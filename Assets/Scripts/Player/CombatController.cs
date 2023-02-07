using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CombatController : MonoBehaviour
{
    [SerializeField] private Transform shotDirection;
    [SerializeField] private int weaponForce = 9000;
    [SerializeField] private int weaponDamage = 35;
    [SerializeField] private int maxAmmo = 50;
    [SerializeField] private int ammo = 0;
    [SerializeField] private int ammoClip = 0; //remove from editor
    private int ammoClipMax = 10;
    [SerializeField] private int ammoBoxAmount = 10; //remove from editor
    [SerializeField] private TextMeshProUGUI ammoUI;
    [SerializeField] private TextMeshProUGUI ammoClipUI;
    [SerializeField] private AudioSource ammoPickUpAudio;
    [SerializeField] private AudioSource triggerAudio;
    [SerializeField] private AudioSource reloadingAudio;
    [SerializeField] private GameObject shotFX;
    [SerializeField] private Transform shotFXtransform;
    [SerializeField] private TrailRenderer bulletTrail;

    //slow mo:
    [SerializeField] float slowMotionTimeScale = 0.5f;
    [SerializeField] float slowMotionDuration = 10f;
    private float startTimeScale;
    private float startFixedDeltaTime;

    private Animator anim;

    public GameObject reticle;
    public GameObject bloodFX;

    void Start()
    {
        anim = this.GetComponentInChildren<Animator>();
        ammoUI.text = ammo.ToString();
        ammoClipUI.text = ammoClip.ToString();
        GameState.gameOver = false;
        GameState.canShoot = true;

        //slow mo:
        startTimeScale = Time.timeScale;
        startFixedDeltaTime = Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        ToggleBulletTime();
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.F))
            anim.SetBool("armed", !anim.GetBool("armed"));

        if (Input.GetMouseButtonDown(0) && anim.GetBool("armed") && GameState.canShoot)
        {
            ProcessShot();
        }
        if (Input.GetKeyDown(KeyCode.R) && anim.GetBool("armed") && ammoClip < 10)
        {
            Reloading();
        }
    }

    private void ProcessShot()
    {
        if (ammoClip > 0)
        {
            anim.SetTrigger("fire");
            GameObject shootingFX = Instantiate(shotFX, shotFXtransform.position, shotFXtransform.rotation);
            Destroy(shootingFX, 0.5f);
            ProcessHit();
            ammoClip--;
            ammoClipUI.text = ammoClip.ToString();
            GameState.canShoot = false;
        }
        else
        {
            triggerAudio.Play();
        }
    }


    public void ProcessHit()
    {
        RaycastHit hit;
        if (Physics.Raycast(shotDirection.position, shotDirection.forward, out hit, 500))
        {
            GameObject hitObject = hit.collider.gameObject;

            //bullet trail:
            TrailRenderer trail = Instantiate(bulletTrail, shotFXtransform.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(trail, hit));

            if (hitObject.tag == "Enemy")
            {
                GameObject hitLimb = hitObject;
                ZombieHealth zombieHealth = hitLimb.GetComponentInParent<ZombieHealth>();

                SpawnBloodFX(hit);
                zombieHealth.GetHit(hitLimb, weaponDamage, weaponForce);
            }
            else if (hitObject.tag == "Limb")
            {
                GameObject hitLimb = hitObject;

                SpawnBloodFX(hit);
                hitLimb.GetComponentInChildren<Rigidbody>().AddForce(Camera.main.transform.forward * (weaponForce / 2f));
            }
            // else
            // {
            //      TODO
            //     Hit particles
            //for impact fx: Instantiate(ImpactParticles, hit.point, Quaternion.LookRotation(hit.normal));
            // }
        }
    }

    bool isOn = false;
    void ToggleBulletTime()
    {
        //slow mo on:
        if (Input.GetKeyDown(KeyCode.Tab) && !isOn)
        {
            isOn = true;
            Time.timeScale = slowMotionTimeScale;
            Time.fixedDeltaTime = startFixedDeltaTime * slowMotionTimeScale;
        }
        //slow mo off:
        else if (Input.GetKeyDown(KeyCode.Tab) && isOn)
        {
            isOn = false;
            Time.timeScale = startTimeScale;
            Time.fixedDeltaTime = startFixedDeltaTime;
        }
    }
    public void SlowMoOnHeadshot()
    {
        //slow mo on:
        Time.timeScale = slowMotionTimeScale;
        Time.fixedDeltaTime = startFixedDeltaTime * slowMotionTimeScale;
        //slow mo off:
        Invoke(nameof(ResetSlowMotion), slowMotionDuration);
    }
    private void ResetSlowMotion()
    {
        Time.timeScale = startTimeScale;
        Time.fixedDeltaTime = startFixedDeltaTime;
    }

    private void SpawnBloodFX(RaycastHit hit)
    {
        // //make bloodFX spurt duration random
        // ParticleSystem spurt = bloodFX.transform.Find("spurt").GetComponent<ParticleSystem>();
        // var main = spurt.main;
        // float rand = Random.Range(0.01f, 1.5f);
        // main.duration = rand;       //random duration

        GameObject blood = Instantiate(bloodFX, hit.point, Quaternion.LookRotation(hit.normal));
        blood.transform.parent = hit.transform;

        //  Destroy(blood, 2f);
    }

    private IEnumerator SpawnTrail(TrailRenderer trail, RaycastHit hit)
    {
        float time = 0;
        Vector3 startPosition = trail.transform.position;

        while (time < 1)
        {
            trail.transform.position = Vector3.Lerp(startPosition, hit.point, time);
            time += Time.deltaTime / trail.time;

            yield return null;
        }
        trail.transform.position = hit.point;
        Destroy(trail.gameObject, trail.time);
    }
    private void Reloading()
    {
        GameState.canShoot = false;
        reloadingAudio.Play();
        anim.SetTrigger("reloading");
        int ammoNeeded = ammoClipMax - ammoClip;
        //check if ammo needed for clip exceeds available ammo
        int ammoAvailable = ammoNeeded < ammo ? ammoNeeded : ammo;
        ammo -= ammoAvailable;
        ammoUI.text = ammo.ToString();
        ammoClip += ammoAvailable;
        ammoClipUI.text = ammoClip.ToString();
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Ammo" && ammo < maxAmmo)
        {
            ammo = Mathf.Clamp(ammo + ammoBoxAmount, 0, maxAmmo);
            ammoUI.text = ammo.ToString();
            ammoPickUpAudio.Play();
            Destroy(col.gameObject);
        }
    }
}