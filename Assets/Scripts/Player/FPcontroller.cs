using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPcontroller : MonoBehaviour
{
    [Header("Configurations")]
    [SerializeField] GameObject cam;
    [SerializeField] Animator anim;
    [SerializeField] GameObject protagonistPrefab;
    [SerializeField] GameObject flashLight;

    //Refactor: put in AudioClass
    [Header("Audio Sources")]
    [SerializeField] AudioSource jumpingAudio;
    [SerializeField] AudioSource landingAudio;
    [SerializeField] AudioSource[] footstepsAudio;
    [SerializeField] float footstepAudioRepeatTime = 0.4f;

    [Header("Movement Variables")]
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float sprintSpeed = 10f;
    [SerializeField] float jumpForce = 300f;
    public float mouseSensitivity = 2f; //might change back to private

    Rigidbody rb;
    CapsuleCollider capsule;
    Quaternion cameraRot;
    Quaternion characterRot;
    private float _minimumX = -90f;
    private float _maximumX = 89.99f;

    private Vector3 _newVelocity;

    bool playingWalkingAudio = false;
    bool previoslyGrounded = true;

    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        capsule = this.GetComponent<CapsuleCollider>();

        cameraRot = cam.transform.localRotation;
        characterRot = this.transform.localRotation;

        GameState.gameOver = false;
        GameState.canShoot = true;

        //lock cursor:
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        CameraControl();
        HandleJump();
        MovementInput();
        MovementAnimAndSounds();
        ToggleFlashlight();
        WeaponZoom();
    }
    void FixedUpdate()
    {
        //move player:
        if (IsGrounded())
            rb.velocity = transform.TransformDirection(_newVelocity);// * (1 / Time.timeScale);//need to fix
    }

    //test
    // bool grounded;
    // private void CheckGrounded()
    // {
    //     grounded = false;
    //     float capsuleHeight = Mathf.Max(capsule.radius * 2f, capsule.height);
    //     Vector3 capsuleBottom = transform.TransformPoint(capsule.center - Vector3.up * capsuleHeight / 2f);
    //     float radius = transform.TransformVector(capsule.radius, 0f, 0f).magnitude;
    //     Ray ray = new Ray(capsuleBottom + transform.up * .01f, -transform.up);
    //     RaycastHit hit;
    //     if (Physics.Raycast(ray, out hit, radius * 5f))
    //     {
    //         float normalAngle = Vector3.Angle(hit.normal, transform.up);
    //         if (normalAngle < 60f)
    //         {
    //             float maxDist = radius / Mathf.Cos(Mathf.Deg2Rad * normalAngle) - radius + .02f;
    //             if (hit.distance < maxDist)
    //                 grounded = true;
    //         }
    //     }
    // }
    //test end

    private void CameraControl()
    {
        float xRot = Input.GetAxis("Mouse Y") * mouseSensitivity;
        float yRot = Input.GetAxis("Mouse X") * mouseSensitivity;

        cameraRot *= Quaternion.Euler(-xRot, 0, 0);
        characterRot *= Quaternion.Euler(0, yRot, 0);

        cameraRot = ClampRotationAroundXAxis(cameraRot);

        this.transform.localRotation = characterRot;
        cam.transform.localRotation = cameraRot;
    }
    void MovementInput()
    {
        float speed;
        _newVelocity = Vector3.up * rb.velocity.y; //equals: new Vector3(0f, rb.velocity.y, 0f)
        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = sprintSpeed;
            GameState.canShoot = false;
        }
        else
        {
            speed = walkSpeed;
            GameState.canShoot = true;
        }
        _newVelocity.x = Input.GetAxisRaw("Horizontal") * speed;
        _newVelocity.z = Input.GetAxisRaw("Vertical") * speed;

    }

    void HandleJump()
    {
        bool grounded = IsGrounded();
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            jumpingAudio.Play();
            rb.AddForce(new Vector3(0, jumpForce, 0));
            if (anim.GetBool("walking") || anim.GetBool("running"))
            {
                anim.SetBool("walking", false);
                anim.SetBool("running", false);
                CancelInvoke(nameof(PlayFootstepAudio));
                playingWalkingAudio = false;
            }

        }
        else if (!previoslyGrounded && grounded)
        {
            landingAudio.Play();
        }
        previoslyGrounded = grounded;
    }

    bool isWalking()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            return true;
        else
            return false;
    }
    void MovementAnimAndSounds()
    {
        if (isWalking() && !Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetBool("running", false);
            if (!anim.GetBool("walking") && IsGrounded())
            {
                anim.SetBool("walking", true);

                CancelInvoke(nameof(PlayFootstepAudio));
                InvokeRepeating(nameof(PlayFootstepAudio), 0, footstepAudioRepeatTime);
            }
        }
        else if (_newVelocity != Vector3.zero && Input.GetKey(KeyCode.LeftShift))
        {
            anim.SetBool("walking", false);
            if (!anim.GetBool("running") && IsGrounded())
            {
                anim.SetBool("running", true);

                CancelInvoke(nameof(PlayFootstepAudio));
                InvokeRepeating(nameof(PlayFootstepAudio), 0, footstepAudioRepeatTime * (walkSpeed / sprintSpeed));
            }
        }
        else if (anim.GetBool("walking") || anim.GetBool("running"))
        {
            CancelInvoke(nameof(PlayFootstepAudio));
            anim.SetBool("walking", false);
            anim.SetBool("running", false);
            playingWalkingAudio = false;
        }
    }

    Quaternion ClampRotationAroundXAxis(Quaternion q)
    {
        q.x /= q.w;
        q.y /= q.w;
        q.z /= q.w;
        q.w = 1.0f;

        float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan(q.x);
        angleX = Mathf.Clamp(angleX, _minimumX, _maximumX);
        q.x = Mathf.Tan(0.5f * Mathf.Deg2Rad * angleX);

        return q;
    }

    bool IsGrounded()
    {
        // RaycastHit hitInfo;
        // if (Physics.SphereCast(transform.position, capsule.radius, Vector3.down, out hitInfo,
        //     (capsule.height / 2f) - capsule.radius + 0.1f))
        // {
        //     return true;
        // }
        // return false;
        float capsuleHeight = Mathf.Max(capsule.radius * 2f, capsule.height);
        Vector3 capsuleBottom = transform.TransformPoint(capsule.center - Vector3.up * capsuleHeight / 2f);
        float radius = transform.TransformVector(capsule.radius, 0f, 0f).magnitude;
        Ray ray = new Ray(capsuleBottom + transform.up * .01f, -transform.up);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, radius * 5f))
        {
            float normalAngle = Vector3.Angle(hit.normal, transform.up);
            if (normalAngle < 90)
            {
                float maxDist = radius / Mathf.Cos(Mathf.Deg2Rad * normalAngle) - radius + .02f;
                if (hit.distance < maxDist)
                    return true;
            }
        }
        return false;
    }
    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == "Limb" || col.gameObject.tag == "Enemy")
        {
            Physics.IgnoreCollision(col.collider, capsule);
        }
        else if (IsGrounded())
        {
            if (anim.GetBool("walking") && !playingWalkingAudio)
                InvokeRepeating(nameof(PlayFootstepAudio), 0f, footstepAudioRepeatTime);
        }
    }

    public void PlayFootstepAudio()
    {
        AudioSource audioSource = new AudioSource();
        int i = Random.Range(1, footstepsAudio.Length);

        audioSource = footstepsAudio[i];
        audioSource.Play();
        footstepsAudio[i] = footstepsAudio[0];
        footstepsAudio[0] = audioSource;
        playingWalkingAudio = true;
    }

    // for refactoring (Make a play random sound from array Function for walk and hurt sounds):
    public void PlayRandomSoundFromArray(AudioSource[] soundArray)
    {
        AudioSource audioSource = new AudioSource();
        int i = Random.Range(1, soundArray.Length);

        audioSource = soundArray[i];
        audioSource.Play();
        soundArray[i] = soundArray[0];
        soundArray[0] = audioSource;
    }

    //MIGHT CHANGE LATER
    private void ToggleFlashlight()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (flashLight.activeSelf)
                flashLight.SetActive(false);
            else
                flashLight.SetActive(true);
        }
    }

    private void WeaponZoom()
    {
        if (Input.GetMouseButton(1))
        {
            Camera.main.fieldOfView = 55;
            mouseSensitivity = 1.2f;

            anim.SetBool("aiming", true);
        }
        else
        {
            Camera.main.fieldOfView = 75;
            mouseSensitivity = 3.5f;

            anim.SetBool("aiming", false);
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Destination")
        {
            GameState.gameOver = true;
            Vector3 position = new Vector3(this.transform.position.x,
                                Terrain.activeTerrain.SampleHeight(this.transform.position),
                                this.transform.position.z);
            GameObject protagonist = Instantiate(protagonistPrefab, position, this.transform.rotation);
            protagonist.GetComponent<Animator>().SetTrigger("Dance");
            Destroy(this.gameObject);
        }
    }
}