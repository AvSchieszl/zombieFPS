using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


//TODO!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
//Make Minimap Icon inactive and only activate ones zombie chases


public class ZombieController : MonoBehaviour
{
    [SerializeField] float stoppingDistance = 2.5f;
    [SerializeField] float zombieViewDistance = 10f;
    [SerializeField] float forgetPlayerDistance = 20f;
    [SerializeField] float removeDeadBodyDistance = 20f;
    [SerializeField] float walkingSpeed = 1f;
    [SerializeField] float runningSpeed = 5f;
    [SerializeField] int attackDamage = 5;
    [SerializeField] public int health = 100;

    [SerializeField] AudioSource[] attackSounds;
    Animator anim;
    NavMeshAgent agent;
    GameObject target;

    public enum STATE { IDLE, WANDER, CHASE, ATTACK, DEAD };
    public STATE state = STATE.IDLE;

    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<RemoveDeadBody>().enabled = false;
        anim = this.GetComponent<Animator>();
        agent = this.GetComponent<NavMeshAgent>();

        target = GameObject.FindWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case STATE.IDLE:
                ChasePlayerOrWander();
                break;
            case STATE.WANDER:
                WanderAround();
                break;
            case STATE.CHASE:
                ChasePlayer();
                break;
            case STATE.ATTACK:
                AttackPlayer();
                break;
            case STATE.DEAD:
                if (DistanceToPlayer() > removeDeadBodyDistance)
                    this.GetComponent<RemoveDeadBody>().enabled = true;
                break;
        }
    }

    void ChasePlayerOrWander()
    {
        if (CanSeePlayer())
            state = STATE.CHASE;
        else if (Random.Range(0, 5000) < 10) //chance of switching from idle to wander
        {
            state = STATE.WANDER;
        }
    }
    void WanderAround()
    {
        if (!agent.hasPath)
        {
            float newX = this.transform.position.x + Random.Range(-5, 5);
            float newZ = this.transform.position.z + Random.Range(-5, 5);
            float newY = Terrain.activeTerrain.SampleHeight(new Vector3(newX, 0, newZ));
            Vector3 destination = new Vector3(newX, newY, newZ);
            agent.SetDestination(destination);
            agent.stoppingDistance = 0;
            TurnOffAnimation();
            agent.speed = walkingSpeed;
            anim.SetBool("isWalking", true);
        }
        if (CanSeePlayer()) state = STATE.CHASE;
        else if (Random.Range(0, 5000) < 5) //chance of switching from wander to idle
        {
            state = STATE.IDLE;
            TurnOffAnimation();
            agent.ResetPath();
        }
    }
    void ChasePlayer()
    {
        if (GameState.gameOver)
        {
            agent.ResetPath();
            TurnOffAnimation();
            state = STATE.WANDER;
            return;
        }
        else
        {
            agent.SetDestination(target.transform.position);
            agent.stoppingDistance = stoppingDistance;
            TurnOffAnimation();
            agent.speed = runningSpeed;
            anim.SetBool("isRunning", true);

            if (agent.remainingDistance <= stoppingDistance && !agent.pathPending)
            {
                state = STATE.ATTACK;
            }

            if (ForgetPlayer())
            {
                state = STATE.WANDER;
                agent.ResetPath();
            }
        }
    }
    void AttackPlayer()
    {
        if (GameState.gameOver)
        {
            agent.ResetPath(); //makes Zombies walk away immediately after game over
            TurnOffAnimation();
            state = STATE.WANDER;
            return;
        }
        TurnOffAnimation();
        anim.SetBool("isAttacking", true);


        Vector3 targetPostition = new Vector3( //lock lookAt on Y
            target.transform.position.x,
            this.transform.position.y,
            target.transform.position.z);
        this.transform.LookAt(targetPostition);

        if (DistanceToPlayer() > stoppingDistance + 1.5f)
            state = STATE.CHASE;

    }
    float DistanceToPlayer()
    {
        if (GameState.gameOver) return Mathf.Infinity;
        return Vector3.Distance(target.transform.position, this.transform.position);
    }
    bool CanSeePlayer()
    {
        if (DistanceToPlayer() + Random.Range(0, 5) < zombieViewDistance)
        {
            return true;
        }
        return false;
    }

    bool ForgetPlayer()
    {
        if (DistanceToPlayer() > forgetPlayerDistance)
        {
            return true;
        }
        return false;
    }

    public void KillZombie()
    {
        TurnOffAnimation();
        this.GetComponentInChildren<SpriteRenderer>().enabled = false;
        agent.enabled = false;
        state = STATE.DEAD;
    }
    public void DealDamage()
    {
        if (target != null)
        {
            target.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
            PlayAttackSounds();
        }
        else
        {
            state = STATE.WANDER;
        }


    }
    void PlayAttackSounds()
    {
        AudioSource audioSource = new AudioSource();
        int i = Random.Range(1, attackSounds.Length);

        audioSource = attackSounds[i];
        audioSource.Play();
        attackSounds[i] = attackSounds[0];
        attackSounds[0] = audioSource;
    }
    public void TurnOffAnimation()
    {
        anim.SetBool("isWalking", false);
        anim.SetBool("isAttacking", false);
        anim.SetBool("isRunning", false);
    }
}
