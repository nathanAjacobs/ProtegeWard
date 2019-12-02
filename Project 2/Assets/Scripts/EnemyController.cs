using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour {

    public NavMeshAgent agent;

    public ConfigurableJoint cj;

    public Transform[] points;

    public float meleeDamage = 25f;

    public float attackRadius = 3f;

    public float freezeTime = 3f;

    private Rigidbody rb;

    private bool isInPush;
    private bool isFrozen;
    private bool wasPunched;

    private bool inTrigger;

    private FieldOfView fow;
    private Animator animator;

    private float frozenTimer = 0f;

    private int currentPoint;
    private float pointRadius = 1f;

    private GameObject currentTargetHit;
    private bool punchedTarget = false;
    private bool playerWasHit = false;

    private bool lastIsFrozen;
    private bool lastIsInPush;
    private bool isDead = false;

    public AudioSource enemySource;
    public AudioClip runningClip;
    public AudioClip swingClip;

    public UIController ui;

    // Use this for initialization
    void Start () {
        currentPoint = 0;
        agent = GetComponent<NavMeshAgent>();
        agent.SetDestination(points[0].position);
        isInPush = false;
        rb = GetComponent<Rigidbody>();
        wasPunched = false;
        rb.freezeRotation = false;
        animator = GetComponent<Animator>();
        fow = GetComponent<FieldOfView>();
        lastIsFrozen = false;
        lastIsInPush = false;

        /*Rigidbody[] rbs = GetComponentsInChildren<Rigidbody>();
        int i = 0;
        foreach (Rigidbody rig in rbs)
        {
            if (i != 0)
                rig.isKinematic = true;
            i++;
        }*/


        Collider[] ragdolls = GetComponentsInChildren<Collider>();
        int i = 0;
        foreach (Collider c in ragdolls)
        {
            if (i != 0)
                c.isTrigger = true;
            i++;
        }
    }

    private void Update()
    {
        if (!ui.IsPaused() && !ui.IsGameWon() && !ui.IsGameLost())
        {
            enemySource.UnPause();
        }
        else
        {
            enemySource.Pause();
        }
    }

    private void FixedUpdate()
    {
        
        // destroy this enemy if dead and rigid body comes to a stop
        if (isDead && Vector3.Magnitude(rb.velocity) <= 0.1)
        {
                Object.Destroy(this.gameObject);
        }

        // reset animator punch bool to false if in the punch state
        if (animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("punch"))
        {
            animator.SetBool("punch", false);
        }


        // update animator freeze and push bools
        animator.SetBool("inPush", isInPush);
        animator.SetBool("freeze", isFrozen);

        // if velocity is not zero, set animator isMoving to true
        if (agent.velocity != Vector3.zero)
        {
            animator.SetBool("isMoving", true);
        }

        // if the Rigidbody's velocity is zero, then enemy is not in push state
        if (rb != null && Vector3.Magnitude(rb.velocity) < 2.5)
        {
            isInPush = false;
        }

        // if the enemy is frozen and the enemy just got pushed, set isFrozen to false
        if (isFrozen && isInPush && lastIsInPush == false)
        {
            isFrozen = false;
        }
        lastIsInPush = isInPush;

        // if the enemy is in push state and just got frozen, set isInPush to false
        if (isInPush && lastIsFrozen == false && isFrozen)
        {
            isInPush = false;
        }
        lastIsFrozen = isFrozen;


        if (!wasPunched)
        {
            // if the enemy has not been punched, this code executes

            if (isFrozen)
            {
                enemySource.Stop();
                // if the enemy is frozen this code executes

                // increment freeze timer
                frozenTimer += Time.fixedDeltaTime;

                // turn off NavMesh Agent so enemy can no longer move
                agent.enabled = false;

                if (frozenTimer >= freezeTime)
                {
                    isFrozen = false;

                    // reset frozen timer becuase no longer frozen
                    frozenTimer = 0;
                }

            }
            else if (isInPush)
            {
                enemySource.Stop();
                // if the enemy is in push state this code executes

                // reset frozen timer becuase no longer frozen
                frozenTimer = 0;

                // freeze rotation, turn off NavMeshAgent, isKinematic to false (so enemy can be pushed)
                //updateComponents();

            }
            else
            {
                // if the enemy is not frozen or in push state, this code executes

                // reset frozen timer becuase no longer frozen
                frozenTimer = 0;

                // turn on NavMeshAgent, set Rigidbody isKinematic to true, unfreeze Rigidbody rotation
                agent.enabled = true;
                rb.isKinematic = true;
                rb.freezeRotation = false;

                // calculate distance to current NavMesh point
                float distToPoint = Vector3.Distance(transform.position, points[currentPoint].position);

                // if the enemy is close enough to the point then we consider it at the point
                if (distToPoint < pointRadius)
                {
                    // set currentPoint to the next point in the array
                    currentPoint++;

                    // if currentPoint is out of bounds reset to zero
                    if (currentPoint == points.Length)
                    {
                        currentPoint = 0;
                    }
                }

                // if no player in view just set destination to current point
                if (fow.visibleTargets.Count == 0)
                {
                    agent.SetDestination(points[currentPoint].position);
                }
                else
                {
                    // if the player is a visible target to the enemy this code executes

                    foreach (Transform t in fow.visibleTargets)
                    {
                        // calculate distance between enemy and player
                        float distance = Vector3.Distance(transform.position, t.position);

                        // set distination of enemy to the player
                        agent.SetDestination(t.position);

                        // fire punch animation if the enemy is within attack radius
                        if (distance <= attackRadius)
                        {
                            animator.SetBool("punch", true);
                        }
                    }
                }

                if (animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("punch"))
                {
                    if (enemySource.isPlaying && enemySource.clip == swingClip)
                    {

                    }
                    else if(enemySource.isPlaying && enemySource.clip == runningClip)
                    {
                        enemySource.Stop();
                    }
                    else
                    {
                        //enemySource.clip = swingClip;
                        //enemySource.loop = false;
                        //enemySource.Play();
                    }

                    // if the enemy is in the punch animation this code executes

                    // if the player was punched and has not been hit during the current animation, remove health from player and set playerWasHit to true
                }
                else
                {
                    if (enemySource.isPlaying && enemySource.clip == runningClip)
                    {

                    }
                    else if (enemySource.isPlaying && enemySource.clip != swingClip)
                    {
                        enemySource.clip = runningClip;
                        enemySource.loop = true;
                        enemySource.Play();
                    }
                    // if the enemy is not in the punch animation this code executes

                    // the player cannot be punched when the enemy is not in the punch animation so the bools are set to false
                    punchedTarget = false;
                    playerWasHit = false;
                }

            }
        }
        else
        {
            // the player was punched
            enemySource.Stop();
            //animator.SetTrigger("die");
            
            agent.enabled = false;
            animator.enabled = false;
            isDead = true;

            Collider[] ragdolls = GetComponentsInChildren<Collider>();
            int i = 0;
            foreach (Collider c in ragdolls)
            {
                if (i != 0)
                    c.isTrigger = false;
                i++;
            }

            //cj.connectedBody = rb;


        }
    }

    public void updateComponents()
    {
        rb.freezeRotation = true;
        agent.enabled = false;
        rb.isKinematic = false;
    }

    public void SetIsFrozen(bool b)
    {
        isFrozen = b;
    }

    public bool GetIsFrozen()
    {
        return isFrozen;
    }

    public void SetIsInPush(bool b)
    {
        isInPush = b;
    }

    public void SetPunchedTarget(bool b)
    {
        punchedTarget = b;
    }

    public void SetWasPunched(bool b)
    {
        wasPunched = b;
    }

    public void SetCurrentTartgetHit(GameObject g)
    {
        currentTargetHit = g;
    }

    public void SetIsDead(bool b)
    {
        isDead = b;
    }

    public void FootR()
    {

    }

    public void FootL()
    {

    }

    public void Swing()
    {
        enemySource.Stop();
        enemySource.loop = false;
        enemySource.clip = swingClip;
        enemySource.Play();
    }

    public void Hit()
    {
        if(inTrigger)
        {
            currentTargetHit.GetComponent<PlayerController>().RemoveHealth(meleeDamage);
        }
    }

    public void SetInTrigger(bool b)
    {
        inTrigger = b;
    }
}
