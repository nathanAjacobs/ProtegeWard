using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerMotor))]
[RequireComponent(typeof(FieldOfView))]
public class PlayerController : MonoBehaviour
{
    public Transform levelID;

    private bool inLevelOne = false;
    private bool inLevelTwo = false;
    private bool inLevelThree = false;

    private OptionsValues optionsVals;
    public float speed = 5f;

    public float runningSpeed = 15f;


    public float lookSensitivity = 3f;
    public float controllerSensitivity = 3f;

    public float jumpSpeed = 5;
    public float downAccl = 0.75f;

    public float cameraLock = 80f;

    public float pushForce = 5.0f;
    public float punchForce = 10f;

    public UIController ui;



    private float health = 100f;

    private Camera cam;


    private float distToGrounded = 0.1f;
    private PlayerMotor motor;

    private LayerMask layerMask;


    private FieldOfView fow;

    private float yVel = 0f;
    private float movX = 0;
    private float movZ = 0;
    private float rotY = 0;
    private float rotX = 0;

    private float pushInput = 0;
    private float freezeInput = 0;
    private float meleeInput = 0;

    private bool pushActivated = false;
    private bool inAttack = false;
    private bool freezeActivated = false;
    private bool notRunning = true;

    private float jumpInput;

    private float lastPushInput = 0;
    private float lastFreezeInput = 0;

    private float overallRotation = 0;

    private GameObject currentTargetHit;

    private bool punchedTarget = false;

    private LayerMask targetMask;

    private Animator animator;

    private bool usingMouse = false;
    private bool usingController = false;

    public AudioSource audioSource;
    public AudioClip runningClip;
    public AudioClip walkingClip;
    public AudioClip swingClip;
    public AudioSource abilitySource;
    public AudioClip punchClip;
    public AudioClip pushClip;
    public AudioClip freezeClip;
    public AudioClip healthClip;
    public AudioSource interactSource;
    public AudioClip gruntClip;
    public AudioSource gruntSource;

    private bool havePlayedSwing = false;


    public LayerMask collisionLayer;
    public Transform playerPosition;
    public GameObject camRotator;
    public GameObject newCamPos;
    private float defaultDist;
    private bool colliding = false;
    private Vector3[] adjustedCameraClipPoints;
    private Vector3[] desiredCameraClipPoints;
    private float adjustmentDistance = 0;

    public Text[] highScores;

    private List<int> highScoreValues = new List<int>();
    private bool alreadyAdded = false;
    private CurrentScore theCurrentScore;

    private void Start()
    {
        motor = GetComponent<PlayerMotor>();
        animator = GetComponent<Animator>();
        fow = GetComponent<FieldOfView>();
        cam = GetComponentInChildren<Camera>();
        layerMask = ~0;
        targetMask = LayerMask.GetMask("Targets");
        adjustedCameraClipPoints = new Vector3[5];
        desiredCameraClipPoints = new Vector3[5];

        optionsVals = GameObject.FindWithTag("Options").GetComponent<OptionsValues>();

        lookSensitivity = optionsVals.mouseSens;
        controllerSensitivity = optionsVals.controllerSens;

        defaultDist = cam.transform.localPosition.z;
        UpdateCameraClipPoints(cam.transform.position, cam.transform.rotation, ref adjustedCameraClipPoints);

        theCurrentScore = GameObject.FindWithTag("Scores").GetComponent<CurrentScore>();
        GetScores();
        //PlayerPrefs.DeleteAll();

        if (levelID.position.x == -70)
        {
            inLevelOne = true;
        }
        else if (levelID.position.x == 70)
        {
            inLevelTwo = true;
        }
        else if (levelID.position.x == 0)
        {
            inLevelThree = true;
        }
    }

    private void Update()
    {
        lookSensitivity = optionsVals.mouseSens;
        controllerSensitivity = optionsVals.controllerSens;

        if (!ui.IsPaused() && !ui.IsGameWon() && !ui.IsGameLost())
        {
            audioSource.UnPause();
            interactSource.UnPause();
            abilitySource.UnPause();
            //reset animator booleans
            if (animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("jump"))
            {
                animator.SetBool("jump", false);
                overallRotation = 0;
            }

            if (animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("push"))
            {
                animator.SetBool("push", false);
                overallRotation = 0;
            }

            if (animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("freeze"))
            {
                animator.SetBool("freeze", false);
                overallRotation = 0;
            }

            //chack to see if player is running
            if (animator.GetFloat("movX") != 0 || animator.GetFloat("movZ") != 0)
            {
                notRunning = false;
            }
            else
            {
                notRunning = true;
            }

            if (ControllerWasPressed())
            {
                usingController = true;
                usingMouse = false;
            }
            if (MouseWasMoved())
            {
                usingController = false;
                usingMouse = true;
            }


            //jump input
            jumpInput = Input.GetAxis("Jump");


            //get movement input
            //movX += Input.GetAxisRaw("Horizontal");
            //movZ += Input.GetAxisRaw("Vertical");
            movX += Input.GetAxisRaw("Horizontal");
            movZ += Input.GetAxisRaw("Vertical");

            //send input to animator
            animator.SetFloat("movX", Input.GetAxis("Horizontal"));
            animator.SetFloat("movZ", Input.GetAxis("Vertical"));

            //get look input
            rotY += Input.GetAxisRaw("Mouse X");

            rotX += Input.GetAxisRaw("Mouse Y");

            rotX += Input.GetAxisRaw("Right Stick X");

            rotY += Input.GetAxisRaw("Right Stick Y");

            //controllerWasPressed();

            overallRotation += rotY;

            // reset animator turn booleans
            if (animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("turnLeft"))
            {
                animator.SetBool("turnLeft", false);
                overallRotation = 0;
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("turnRight"))
            {
                animator.SetBool("turnRight", false);
                overallRotation = 0;
            }

            // animator call to turn left or right
            if (overallRotation < -90 && movZ == 0 && movX == 0 && animator.GetCurrentAnimatorStateInfo(0).tagHash != Animator.StringToHash("jump")
                                                                && animator.GetCurrentAnimatorStateInfo(0).tagHash != Animator.StringToHash("push")
                                                                && animator.GetCurrentAnimatorStateInfo(0).tagHash != Animator.StringToHash("freeze")
                                                                && animator.GetCurrentAnimatorStateInfo(0).tagHash != Animator.StringToHash("attack"))
            {
                animator.SetBool("turnLeft", true);

            }
            else if (overallRotation > 90 && movZ == 0 && movX == 0 && animator.GetCurrentAnimatorStateInfo(0).tagHash != Animator.StringToHash("jump")
                                                                    && animator.GetCurrentAnimatorStateInfo(0).tagHash != Animator.StringToHash("push")
                                                                    && animator.GetCurrentAnimatorStateInfo(0).tagHash != Animator.StringToHash("freeze")
                                                                    && animator.GetCurrentAnimatorStateInfo(0).tagHash != Animator.StringToHash("attack"))
            {
                animator.SetBool("turnRight", true);
            }

            // if paused clear move and look values
            if (ui.IsPaused())
            {
                movX = 0;
                movZ = 0;
                rotY = 0;
                rotX = 0;
            }

            // get ability inputs
            pushInput = Input.GetAxisRaw("Ability1");
            freezeInput = Input.GetAxisRaw("Ability2");
            meleeInput = Input.GetAxisRaw("Melee");

            //Debug.Log(inLevelOne);

            // activate player animation for push
            if (!inLevelOne && freezeInput == 0 && meleeInput == 0 && jumpInput == 0 && lastPushInput <= 0 && pushInput > 0 && ((animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("movement") && animator.GetAnimatorTransitionInfo(0).duration == 0) || (
                                                                                                                  animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("turnLeft") ||
                                                                                                                  animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("turnRight") ||
                                                                                                                  animator.GetAnimatorTransitionInfo(0).userNameHash == Animator.StringToHash("turnRight") ||
                                                                                                                  animator.GetAnimatorTransitionInfo(0).userNameHash == Animator.StringToHash("turnLeft"))))
            {
                animator.SetBool("push", true);
                pushActivated = true;
            }
            if (inLevelThree && pushInput == 0 && meleeInput <= 0 && jumpInput == 0 && lastFreezeInput >= 0 && freezeInput < 0 && ((animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("movement") && animator.GetAnimatorTransitionInfo(0).duration == 0) || (
                                                                                                                    animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("turnLeft") ||
                                                                                                                    animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("turnRight") ||
                                                                                                                    animator.GetAnimatorTransitionInfo(0).userNameHash == Animator.StringToHash("turnRight") ||
                                                                                                                    animator.GetAnimatorTransitionInfo(0).userNameHash == Animator.StringToHash("turnLeft"))))
            {
                
                animator.SetBool("freeze", true);
                freezeActivated = true;
            }
            if (freezeInput >= 0 && pushInput == 0 && jumpInput == 0 && meleeInput > 0 && ((animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("movement") && animator.GetAnimatorTransitionInfo(0).duration == 0) || (
                                                                                            animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("turnLeft") ||
                                                                                            animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("turnRight") ||
                                                                                            animator.GetAnimatorTransitionInfo(0).userNameHash == Animator.StringToHash("turnRight") ||
                                                                                            animator.GetAnimatorTransitionInfo(0).userNameHash == Animator.StringToHash("turnLeft"))))
            {
                animator.SetBool("attack", true);
            }

            if ((animator.GetCurrentAnimatorStateInfo(0).tagHash != Animator.StringToHash("movement") && animator.GetCurrentAnimatorStateInfo(0).tagHash != Animator.StringToHash("turnLeft") && 
                                                                                                         animator.GetCurrentAnimatorStateInfo(0).tagHash != Animator.StringToHash("turnRight")) || animator.GetAnimatorTransitionInfo(0).duration != 0)
            {
                jumpInput = 0;
            }


            // check if in attack animation
            if (animator.GetCurrentAnimatorStateInfo(0).tagHash == Animator.StringToHash("attack"))
            {
                inAttack = true;
            }
            else
            {
                inAttack = false;
            }

            // temporary key to win the game
            //if (Input.GetKeyDown("m"))
            //{
            //    ui.Won();
            //}
            lastPushInput = pushInput;
            lastFreezeInput = freezeInput;
        }
        else
        {
            audioSource.Pause();
            interactSource.Pause();
            abilitySource.Pause();
        }

        if ((ui.IsGameLost() || ui.IsGameWon()) && !alreadyAdded)
        {
            // Debug.Log(points);
            InsertScore(theCurrentScore.points);
            /*foreach (int i in highScoreValues)
            {
               // Debug.Log(i);
            }*/
            PopulateScores();
            UpdateCanvasScore();
            alreadyAdded = true;
        }

    }

    private void FixedUpdate()
    {
        Vector3 movHorizontal = transform.right * movX;
        Vector3 movVertical = transform.forward * movZ;
        Vector3 velocity = Vector3.zero;

        // set velocity accordingly depending on if running or not
        //(movVertical + movHorizontal).normalized == transform.forward /*&& notRunning == false*/
        if (notRunning)
        {
            audioSource.Stop();
            velocity = Vector3.zero;
        }
        else if (movZ >= 0.9f && movX > -0.6f && movX < 0.6f && !notRunning)
        {
            velocity = (movHorizontal + movVertical).normalized * runningSpeed;
            
            audioSource.loop = true;
            if (audioSource.isPlaying && audioSource.clip.Equals(walkingClip))
            {
                audioSource.Stop();
                audioSource.clip = runningClip;
                audioSource.Play();
            }
            else if (audioSource.isPlaying && audioSource.clip == runningClip)
            {

            }
            else
            {
                audioSource.clip = runningClip;
                audioSource.Play();
            }
        }
        else if(movX != 0 || movZ != 0)
        {
            //Debug.Log("Heyyy");
            audioSource.loop = true;
            if (audioSource.isPlaying && audioSource.clip == runningClip)
            {
                audioSource.Stop();
                audioSource.clip = walkingClip;
                audioSource.Play();
            }
            else if(audioSource.isPlaying && audioSource.clip == walkingClip)
            {
                ;
            }
            else
            {
                //Debug.Log(walkingClip);
                //Debug.Log(audioSource.clip);
                audioSource.clip = walkingClip;
                
                audioSource.Play();
            }
            velocity = (movHorizontal + movVertical).normalized * speed;
        }



        // jump logic

        if (jumpInput >= 0.1 && Grounded())
        {
            yVel = jumpSpeed;
            animator.SetBool("jump", true);
        }
        else if (yVel <= 0 && Grounded())
        {
            yVel = 0;
        }
        else
        {
            yVel = yVel - downAccl * Time.fixedDeltaTime;
        }

        if(yVel != 0)
        {
            audioSource.Stop();
        }

        //add y velocity to velocity vector and make move call
        Vector3 upVector = new Vector3(0, yVel, 0);
        velocity += upVector;
        motor.Move(velocity);


        //get y rotation and make rotate call
        Vector3 rotation = Vector3.zero;

        if(usingMouse)
        {
            rotation = new Vector3(0f, rotY, 0f) * lookSensitivity;
        }
        else if (usingController)
        {
            rotation = new Vector3(0f, rotY, 0f) * controllerSensitivity;
        }

        Quaternion oldCamRot = camRotator.transform.rotation;

        UpdateCameraClipPoints(cam.transform.position, cam.transform.rotation, ref adjustedCameraClipPoints);

        motor.Rotate(rotation);


        // camera lock code
        float rotationOfCamera = cam.transform.rotation.eulerAngles.x;

        if (usingMouse)
        {
            if (rotationOfCamera + -1 * rotX * lookSensitivity < 360 - cameraLock && rotationOfCamera + -1 * rotX * lookSensitivity > cameraLock)
            {
                rotX = 0f;
            }
        }
        else if (usingController)
        {
            if (rotationOfCamera + -1 * rotX * controllerSensitivity < 360 - cameraLock && rotationOfCamera + -1 * rotX * controllerSensitivity > cameraLock)
            {
                rotX = 0f;
            }
        }

        Vector3 cameraRotation = Vector3.zero;

        if (usingMouse)
        {
            cameraRotation = new Vector3(rotX, 0f, 0f) * lookSensitivity;
        }
        else if (usingController)
        {
            cameraRotation = new Vector3(rotX, 0f, 0f) * controllerSensitivity;
        }

        //rotate camera
        
        motor.RotateCamera(cameraRotation);

        
        UpdateCameraClipPoints(cam.transform.position, cam.transform.rotation, ref desiredCameraClipPoints);

        for(int i = 0; i < 5; i++)
        {
            Debug.DrawLine(cam.transform.position, adjustedCameraClipPoints[i], Color.green);
        }

        Debug.DrawLine(cam.transform.position, playerPosition.position + new Vector3(0, 1, 0), Color.red);

        CheckColliding(playerPosition.position + new Vector3(0, 1, 0));
        adjustmentDistance = GetAdjustedCameraDistance(cam.transform.position);

        if (colliding)
        {
            float newZ = 0f;
            float distToPlayer = Vector3.Distance(playerPosition.position + new Vector3(0, 1, 0), cam.transform.position);
            newZ = distToPlayer - adjustmentDistance;
            if (newZ > -0.516f)
            {
                adjustmentDistance = -0.516f;
            }
            //Debug.Log(newZ);
            newCamPos.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, cam.transform.localPosition.y + newZ);
            cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, newCamPos.transform.localPosition, 0.5f);
        }
        else
        {
            //Debug.Log(defaultDist);
            newCamPos.transform.localPosition = new Vector3(cam.transform.localPosition.x, cam.transform.localPosition.y, defaultDist);
            UpdateCameraClipPoints(newCamPos.transform.position, cam.transform.rotation, ref desiredCameraClipPoints);
            if(!CollisionDetectedAtClipPoints(desiredCameraClipPoints, playerPosition.position))
                cam.transform.localPosition = Vector3.Lerp(cam.transform.localPosition, newCamPos.transform.localPosition, 0.5f);
        }

        movX = 0;
        movZ = 0;
        rotX = 0;
        rotY = 0;

        /*foreach (Transform t in fow.visibleTargets)
        {
            EnemyController ec = t.GetComponent<EnemyController>();
            EnemyRangedController erc = t.GetComponent<EnemyRangedController>();

            Rigidbody r = t.GetComponent<Rigidbody>();
            Vector3 dirToTarget = (r.transform.position - transform.position).normalized;
            
            Debug.DrawRay(transform.position, dirToTarget, Color.yellow);
            
        }*/

        // freeze code
        if (freezeActivated)
        {
            overallRotation = 0;
            if (audioSource.isPlaying && (audioSource.clip == runningClip || audioSource.clip == walkingClip))
            {
                audioSource.Stop();
            }
            if (abilitySource.isPlaying && abilitySource.clip == freezeClip)
            {

            }
            else
            {
                abilitySource.clip = freezeClip;
                abilitySource.loop = false;
                abilitySource.Play();
            }
            RaycastHit hit;
            if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100, targetMask))
            {
                EnemyController ec = hit.transform.GetComponent<EnemyController>();
                if (ec != null)
                    ec.SetIsFrozen(true);

                EnemyRangedController erc = hit.transform.GetComponent<EnemyRangedController>();
                if (erc != null)
                    erc.SetIsFrozen(true);
            }

            freezeActivated = false;
        }
        lastFreezeInput = freezeInput;

        // push code
        if (pushActivated)
        {
            overallRotation = 0;
            if (audioSource.isPlaying && (audioSource.clip == runningClip || audioSource.clip == walkingClip))
            {
                audioSource.Stop();
            }
            if (abilitySource.isPlaying && abilitySource.clip == pushClip)
            {

            }
            else
            {
                abilitySource.clip = pushClip;
                abilitySource.loop = false;
                abilitySource.Play();
            }
            foreach (Transform t in fow.visibleTargets)
            {
                EnemyController ec = t.GetComponent<EnemyController>();
                EnemyRangedController erc = t.GetComponent<EnemyRangedController>();
                if (ec != null)
                {
                    ec.SetIsInPush(true);
                    ec.updateComponents();
                }
                if (erc != null)
                {
                    erc.SetIsInPush(true);
                    erc.updateComponents();
                }

                Rigidbody r = t.GetComponent<Rigidbody>();

                r.isKinematic = false;

                Vector3 dirToTarget = (r.transform.position - transform.position).normalized;   
                
                r.AddForce(dirToTarget * pushForce, ForceMode.Impulse);
            }
            pushActivated = false;
        }
        lastPushInput = pushInput;

        // punch code
        if (inAttack)
        {
            overallRotation = 0;
            animator.SetBool("attack", false);

            if(audioSource.isPlaying && (audioSource.clip == runningClip || audioSource.clip == walkingClip))
            {
                audioSource.Stop();
            }
            if(abilitySource.isPlaying && abilitySource.clip == swingClip)
            {

            }
            else if(!havePlayedSwing)
            {
                abilitySource.clip = swingClip;
                abilitySource.loop = false;
                abilitySource.Play();
                havePlayedSwing = true;
            }
            if (punchedTarget)
            {
                theCurrentScore.points += 10;
                
                Rigidbody rb = currentTargetHit.GetComponent<Rigidbody>();


                EnemyController ec = currentTargetHit.GetComponent<EnemyController>();
                EnemyRangedController erc = currentTargetHit.GetComponent<EnemyRangedController>();
                if (ec != null)
                {
                    currentTargetHit.GetComponent<EnemyController>().SetWasPunched(true);
                    ec.cj.connectedBody = rb;
                    //ec.cj.connectedAnchor = new Vector3(0, 0.8358577f, -0.04685646f);
                    ec.agent.enabled = false;
                }
                if (erc != null)
                {
                    currentTargetHit.GetComponent<EnemyRangedController>().SetWasPunched(true);
                }

                
                rb.isKinematic = false;
                rb.gameObject.GetComponent<CapsuleCollider>().isTrigger = true;
                currentTargetHit.GetComponent<NavMeshAgent>().enabled = false;


                Rigidbody[] rbs = currentTargetHit.GetComponentsInChildren<Rigidbody>();
                int i = 0;
                foreach (Rigidbody rig in rbs)
                {
                    if (i != 0)
                        rig.isKinematic = false;
                    i++;
                }

                /*Collider[] ragdolls = currentTargetHit.GetComponentsInChildren<Collider>();
                i = 0;
                foreach (Collider c in ragdolls)
                {
                    if (i != 0)
                        c.isTrigger = false;
                    i++;
                }*/


                Vector3 force = cam.transform.forward;
                

                if(ec != null && ec.GetIsFrozen())
                {
                    force = new Vector3(force.x, 1f, force.z);
                }
                else
                {
                    force = new Vector3(force.x, 0.25f, force.z);
                }
                rb.AddForce(force * punchForce, ForceMode.VelocityChange);

                if (ec != null)
                {
                    currentTargetHit.GetComponent<EnemyController>().SetIsDead(true);
                }
                if (erc != null)
                {
                    currentTargetHit.GetComponent<EnemyRangedController>().SetIsDead(true);
                }

                abilitySource.clip = punchClip;
                abilitySource.Play();

                punchedTarget = false;
            }
        }

        

        else
        {
            havePlayedSwing = false;
            
        }

    }

    private bool Grounded()
    {
        return Physics.Raycast(new Vector3(transform.position.x, transform.position.y + 0.05f, transform.position.z), Vector3.down, distToGrounded, layerMask);
    }

    public void AddHealth(float amountToAdd)
    {
        if(health != 100)
        {
            interactSource.Stop();
            interactSource.clip = healthClip;
            interactSource.Play();
        }
        
        health += amountToAdd;
        if (health > 100)
        {
            health = 100;
        }
    }

    public void RemoveHealth(float amountToRemove)
    {
        gruntSource.Stop();
        gruntSource.clip = gruntClip;
        gruntSource.Play();

        health -= amountToRemove;
        if (health < 1)
        {
            health = 0;
            Die();
        }

    }

    public float GetPlayerHealth()
    {
        return health;
    }

    private void Die()
    {
        ui.Lost();
    }

    public void FootR()
    {

    }

    public void FootL()
    {

    }

    public void Hit()
    {
        
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 2.5f, targetMask))
        {
            //Debug.Log("Heyy");
            currentTargetHit = hit.collider.gameObject;

            //Debug.Log(currentTargetHit.name);

            punchedTarget = true;
        }
    }

    public void SetPunchedTarget(bool b)
    {
        punchedTarget = b;
    }

    public void SetCurrentTargetHit(GameObject g)
    {
        currentTargetHit = g;
    }

    private bool ControllerWasPressed()
    {
        bool pressed = false;

        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKey("joystick button " + i))
            {
                pressed = true;
            }
        }

        if (Input.GetAxisRaw("HorizontalController") != 0 || Input.GetAxisRaw("VerticalController") != 0)
        {
            pressed = true;
        }
        if (Input.GetAxisRaw("Right Stick X") != 0 || Input.GetAxisRaw("Right Stick Y") != 0)
        {
            pressed = true;
        }
        if (Input.GetAxisRaw("Triggers") != 0)
        {
            pressed = true;
        }
        return pressed;
    }

    private bool MouseWasMoved()
    {
        if (Input.GetAxisRaw("Mouse X") != 0 || Input.GetAxisRaw("Mouse Y") != 0)
        {
            return true;
        }

        return false;
    }

    private void UpdateCameraClipPoints(Vector3 cameraPosition, Quaternion atRotation, ref Vector3[] intoArray)
    {
        intoArray = new Vector3[5];

        float z = cam.nearClipPlane;
        float x = Mathf.Tan(cam.fieldOfView) * z;
        float y = x / cam.aspect;

        intoArray[0] = (atRotation * new Vector3(-x, y, z)) + cameraPosition;
        intoArray[1] = (atRotation * new Vector3(x, y, z)) + cameraPosition;
        intoArray[2] = (atRotation * new Vector3(-x, -y, z)) + cameraPosition;
        intoArray[3] = (atRotation * new Vector3(x, -y, z)) + cameraPosition;

        intoArray[4] = cameraPosition;

    }

    private bool CollisionDetectedAtClipPoints(Vector3[] clipPoints, Vector3 fromPosition)
    {
        for(int i = 0; i < clipPoints.Length; i++)
        {
            Ray ray = new Ray(fromPosition, clipPoints[i] - fromPosition);
            float distance = Vector3.Distance(clipPoints[i], fromPosition);
            if(Physics.Raycast(ray, distance, collisionLayer))
            {
                return true;
            }
        }

        return false;
    }

    private float GetAdjustedCameraDistance(Vector3 fromPosition)
    {
        float distance = -1;

        for(int i = 0; i < desiredCameraClipPoints.Length; i++)
        {
            Ray ray = new Ray(fromPosition, desiredCameraClipPoints[i] - fromPosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
            {
                if(distance == -1)
                {
                    distance = hit.distance;
                }
                else
                {
                    if (hit.distance < distance)
                        distance = hit.distance;
                }
            }
        }


        if (distance == -1)
            return 0;
        else
            return distance;
    }

    private void CheckColliding(Vector3 targetPosition)
    {
        if(CollisionDetectedAtClipPoints(desiredCameraClipPoints, targetPosition))
        {
            colliding = true;
        }
        else
        {
            colliding = false;
        }
    }

    public void UpdateCanvasScore()
    {
        int i = 0;

        foreach (int f in highScoreValues)
        {
            //Debug.Log(i);
            highScores[i].text = f.ToString();
            highScores[i].enabled = true;
            i++;
        }
    }

    private void InsertScore(int points)
    {
        int spotToInsert = -1;

        for (int i = 0; i < 8; i++)
        {
            //Debug.Log(highScoreValues[i]);
            if (points > highScoreValues[i])
            {
                spotToInsert = i;
                //Debug.Log(i);
                break;
            }
        }

        if (spotToInsert != -1)
        {
            highScoreValues.Insert(spotToInsert, points);
            highScoreValues.RemoveAt(8);
        }
    }

    private void PopulateScores()
    {

        for (int i = 0; i < 8; i++)
        {
            switch (i)
            {
                case 0:
                    PlayerPrefs.SetInt("Score0", highScoreValues[i]);
                    break;
                case 1:
                    PlayerPrefs.SetInt("Score1", highScoreValues[i]);
                    break;
                case 2:
                    PlayerPrefs.SetInt("Score2", highScoreValues[i]);
                    break;
                case 3:
                    PlayerPrefs.SetInt("Score3", highScoreValues[i]);
                    break;
                case 4:
                    PlayerPrefs.SetInt("Score4", highScoreValues[i]);
                    break;
                case 5:
                    PlayerPrefs.SetInt("Score5", highScoreValues[i]);
                    break;
                case 6:
                    PlayerPrefs.SetInt("Score6", highScoreValues[i]);
                    break;
                case 7:
                    PlayerPrefs.SetInt("Score7", highScoreValues[i]);
                    break;
            }
        }
    }

    private void GetScores()
    {
        highScoreValues.Clear();
        for (int i = 0; i < 8; i++)
        {
            switch (i)
            {
                case 0:
                    highScoreValues.Add(PlayerPrefs.GetInt("Score0"));
                    break;
                case 1:
                    highScoreValues.Add(PlayerPrefs.GetInt("Score1"));
                    break;
                case 2:
                    highScoreValues.Add(PlayerPrefs.GetInt("Score2"));
                    break;
                case 3:
                    highScoreValues.Add(PlayerPrefs.GetInt("Score3"));
                    break;
                case 4:
                    highScoreValues.Add(PlayerPrefs.GetInt("Score4"));
                    break;
                case 5:
                    highScoreValues.Add(PlayerPrefs.GetInt("Score5"));
                    break;
                case 6:
                    highScoreValues.Add(PlayerPrefs.GetInt("Score6"));
                    break;
                case 7:
                    highScoreValues.Add(PlayerPrefs.GetInt("Score7"));
                    break;
            }
        }
    }

    public void Swing()
    {

    }
}
