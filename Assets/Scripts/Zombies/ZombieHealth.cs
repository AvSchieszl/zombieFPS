using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ZombieHealth : MonoBehaviour
{
    [SerializeField] public int health = 100;
    private int startingHealth;
    private bool isDead = false;
    [SerializeField] GameObject head;
    [SerializeField] GameObject headShotFX;

    [Header("Slow motion Death:")]
    [SerializeField] float slowMotionTimeScale = 0.5f;
    [SerializeField] float slowMotionStartTime = 0.5f;
    [SerializeField] float slowMotionDuration = 0.3f;
    private float startTimeScale;
    private float startFixedDeltaTime;

    private ZombieController zombieController;
    private Ragdoll ragdoll;
    private CombatController combatController;

    // Start is called before the first frame update
    void Start()
    {
        zombieController = this.GetComponent<ZombieController>();
        ragdoll = this.GetComponent<Ragdoll>();
        startingHealth = health;


        //slow mo:
        startTimeScale = Time.timeScale;
        startFixedDeltaTime = Time.fixedDeltaTime;
    }

    public void GetHit(GameObject hitLimb, int damage, int weaponForce)
    {
        health = (int)Mathf.Clamp(health - damage, 0, startingHealth);

        if (hitLimb.name == "Head" || hitLimb.name == "Zombie:Head")
        {
            ZombieDeath();
            HeadShot();
        }
        else if (health == 0)
        {
            SeverAllLimbs();
            ZombieDeath();
            hitLimb.GetComponent<Rigidbody>().AddForce((Camera.main.transform.forward + new Vector3(0, 0.25f, 0)) * weaponForce);
            if (hitLimb.GetComponent<DismemberLimb>())
            {
                hitLimb.transform.parent.GetComponentInParent<Rigidbody>().AddForce(Camera.main.transform.forward * weaponForce);
                hitLimb.GetComponent<DismemberLimb>().SevereLimb();
            }
        }
        else
        {
            //hit reaction
        }
    }


    private void ZombieDeath()
    {
        if (!isDead)
        {
            isDead = true;
            ragdoll.RagdollModeOn();
            zombieController.KillZombie();

            //  Invoke(nameof(SlowMoOnDeath), slowMotionStartTime);
        }
    }

    void SlowMoOnDeath()
    {
        Time.timeScale = slowMotionTimeScale;
        Time.fixedDeltaTime = startFixedDeltaTime * slowMotionTimeScale;
        Invoke(nameof(ResetSlowMotion), slowMotionDuration);
    }
    void ResetSlowMotion()
    {
        Time.timeScale = startTimeScale;
        Time.fixedDeltaTime = startFixedDeltaTime;
    }

    private void HeadShot()
    {
        health = 0;
        // if (zombieController.state != ZombieController.STATE.DEAD)
        //     combatController.SlowMoOnHeadshot();

        head.transform.localScale = Vector3.zero;
        GameObject blood = Instantiate(headShotFX, head.transform.position, head.transform.rotation);
        blood.transform.parent = head.transform;

        head.GetComponentInChildren<Rigidbody>().AddForce(Camera.main.transform.forward * 1000f);

        Destroy(blood, 9f);
    }

    private void SeverAllLimbs()
    {
        foreach (DismemberLimb limb in GetComponentsInChildren<DismemberLimb>())
        {
            limb.SevereLimb();
        }
    }
}
