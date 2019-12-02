using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyRangedController : MonoBehaviour {

    

    public Transform[] points;

    public float fleeRadius = 5f;

    public float bulletSpeed = 100f;

    public float bulletDamage = 10f;

    public float freezeTime = 3f;

    private NavMeshAgent agent;

    private Rigidbody rb;

    private bool isInPush;
    private bool isFrozen;
    private bool wasPunched;
    

    private FieldOfView fow;
    private Animator animator;

    private float frozenTimer = 0f;

    private int currentPoint;
    private float pointRadius = 1f;

    private GameObject bulletStartPoint;

    private bool playerHit = false;

    private bool lastIsFrozen;
    private bool lastIsInPush;
    private bool isDead = false;
    private bool isAtPoint = false;

    public AudioSource enemySource;
    public AudioClip runningClip;
    public AudioClip swingClip;

    // Use this for initialization
    void Start()
    {
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
        bulletStartPoint = FindChildObjectWithTag("bulletStartPoint");
    }

    public void FixedUpdate()
    {
        // destroy this enemy if dead and rigid body comes to a stop
        if (isDead && Vector3.Magnitude(rb.velocity) < 0.7)
        {
            Object.Destroy(this.gameObject);
        }

        // reset animator shoot bool to false
        animator.SetBool("shoot", false);

        // update animator inPush bool
        animator.SetBool("inPush", isInPush);


        // if velocity is not zero, set animator isMoving to true
        if (agent.velocity != Vector3.zero)
        {
            animator.SetBool("isMoving", true);
        }

        // if the Rigidbody's velocity is zero, then enemy is not in push state
        if (rb.velocity == Vector3.zero)
        {
            //isInPush = false;
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

        // if the enemy just got frozen set the animator bool freeze to true
        if(lastIsFrozen == false && isFrozen == true)
        {
            animator.SetBool("freeze", true);
        }
        lastIsFrozen = isFrozen;


        if (!wasPunched)
        {
            // if the enemy has not been punched, this code executes

            if (!isFrozen && !isInPush)
            {
                // if the enemy is not frozen or in push state, this code executes

                //reset frozen timer because not frozen
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
                    animator.SetBool("reachedPoint", true);
                    animator.SetBool("isMoving", false);
                    isAtPoint = true;
                }
                else
                {
                    animator.SetBool("reachedPoint", false);
                    isAtPoint = false;
                }

                // if the enemy is not moving
                if (animator.GetBool("isMoving") == false)
                {
                    foreach (Transform t in fow.visibleTargets)
                    {
                        // get current rotation of enemy
                        Quaternion originalRot = transform.rotation;
                        
                        // look at the player
                        transform.LookAt(t);

                        // new rotation is the rotation after LookAt() call
                        Quaternion newRot = transform.rotation;

                        // set rotation back to original rotation
                        transform.rotation = originalRot;

                        // Lerp between both rotations to smooth rotation
                        transform.rotation = Quaternion.Lerp(transform.rotation, newRot, 15 * Time.fixedDeltaTime);

                        // fire shoot animation which calls Shoot()
                        animator.SetBool("shoot", true);
                    }
                    enemySource.Stop();
                }
                else
                {
                    if (enemySource.isPlaying && enemySource.clip == runningClip)
                    {

                    }
                    else
                    {
                        enemySource.clip = runningClip;
                        enemySource.loop = true;
                        enemySource.Play();
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
                        // calculate distance from enemy to player
                        float distance = Vector3.Distance(transform.position, t.position);

                        if (distance <= fleeRadius)
                        {
                            // if the player is within the fleeRadius this code executes

                            // variable to hold which point is furthest from player
                            float maxDistanceToPlayer = 0;
                            for (int i = 0; i < 4; i++)
                            {
                                Transform tran = points[i];

                                // calculate distance between player and point
                                float dist = Vector3.Distance(tran.position, t.position);

                                // if the distance is greater than previous max distance update max distance variable and current point
                                if (dist > maxDistanceToPlayer)
                                {
                                    maxDistanceToPlayer = dist;
                                    currentPoint = i;
                                }
                            }
                        }

                        // set the destination to current point (furthest point from player)
                        agent.SetDestination(points[currentPoint].position);
                    }
                }
            }
            else if (isFrozen)
            {
                // if the enemy is frozen this code executes

                // update animator bools
                animator.SetBool("isMoving", false);
                animator.SetBool("freeze", true);

                // increment freeze timer
                frozenTimer += Time.fixedDeltaTime;

                // turn off NavMesh Agent so enemy can no longer move
                agent.enabled = false;

                if (frozenTimer >= freezeTime)
                {
                    isFrozen = false;

                    // if the enemy is at a point go straight into shoot animation after freeze
                    if (isAtPoint)
                        animator.SetBool("shoot", true);
                    animator.SetBool("freeze", false);

                    // reset frozen timer becuase no longer frozen
                    frozenTimer = 0;
                }

            }
            else if (isInPush)
            {
                enemySource.Stop();
                // if the enemy is in push state this code executes

                // update animator freeze bool
                animator.SetBool("freeze", false);

                // reset frozen timer becuase no longer frozen
                frozenTimer = 0;

                // freeze rotation, turn off NavMeshAgent, isKinematic to false (so enemy can be pushed)
                updateComponents();
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
        }
    }

    public void updateComponents()
    {
        rb.freezeRotation = true;
        agent.enabled = false;
        rb.isKinematic = false;
    }

    // this method is called at a set time during the shoot animation
    public void Shoot()
    {
        enemySource.clip = swingClip;
        enemySource.loop = false;
        enemySource.Play();
        // spawn a sphere at bulletStartPoints position, scale it down, set collider to be trigger, add BulletController script, add rigidbody
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = bulletStartPoint.transform.position;
        sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        sphere.GetComponent<SphereCollider>().isTrigger = true;
        sphere.AddComponent<BulletController>();
        BulletController bc = sphere.GetComponent<BulletController>();
        sphere.AddComponent<Rigidbody>();
        Rigidbody rb = sphere.GetComponent<Rigidbody>();


        rb.mass = 0.1f;

        // isKinematic set to true, because bullets position is updated through script
        rb.isKinematic = true;
        rb.useGravity = false;

        foreach (Transform t in fow.visibleTargets)
        {
            // set target position to player position
            Vector3 targetPos = t.transform.position;

            // add 1 to y value of position in order to shoot at chest
            targetPos = new Vector3(targetPos.x, targetPos.y + 1f, targetPos.z);

            // calculate direction to player from sphere
            Vector3 dirToTarget = (targetPos - sphere.transform.position).normalized;

            // calculate distance between player and sphere
            float distance = Vector3.Distance(targetPos, sphere.transform.position);

            // set target position to 10 times the distance in the direction of the player
            bc.SetTargetPosition(sphere.transform.position + dirToTarget * distance * 10);
            bc.SetBulletSpeed(bulletSpeed);
            bc.SetBulletDamage(bulletDamage);
        }
    }

    public void SetIsFrozen(bool b)
    {
        isFrozen = b;
    }

    public void SetIsInPush(bool b)
    {
        isInPush = b;
    }

    public void SetWasPunched(bool b)
    {
        wasPunched = b;
    }

    public void SetIsDead(bool b)
    {
        isDead = b;
    }

    // method to find child game object with given tag
    public GameObject FindChildObjectWithTag(string tag)
    {
        Transform parent = transform;
        return GetChildObject(parent, tag);
    }

    // helper method to find child game object with given tag
    public GameObject GetChildObject(Transform parent, string tag)
    {
        if (parent.childCount > 0)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.tag.Equals(tag))
                {
                    return child.gameObject;
                }
                if (child.childCount > 0)
                {
                    GameObject temp = GetChildObject(child, tag);
                    if (temp != null)
                        return temp;
                }
            }
        }
        return null;
    }

    public void FootR()
    {

    }

    public void FootL()
    {

    }

}
