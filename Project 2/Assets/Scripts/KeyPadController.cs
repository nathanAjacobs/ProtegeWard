using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;

public class KeyPadController : MonoBehaviour {

    public string keyPadID = "green";
    public string doorID = "green";
    public float useRadius = 3f;

    private GameObject player;
    
    private LayerMask keyPadMask;
    private Text interactionTextHasKey;
    private Text interactionTextNoKey;
    private Text controllerText;

    private bool doorOpened = false;

    private bool hasKey = false;

    private Camera cam;

    private bool inRange = false;

    private Animator anim;


    private GameObject[] kpcs;

    public AudioSource keyPadSource;
    public AudioClip beepClip;

    private bool usingController = false;
    private bool usingMouse = false;

    private UIController ui;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindWithTag("Player");
        cam = player.GetComponentInChildren<Camera>();
        keyPadMask = LayerMask.GetMask("KeyPads");

        GameObject[] doors = GameObject.FindGameObjectsWithTag("Door");

        for(int i = 0; i < doors.Length; i++)
        {
            DoorsController door = doors[i].GetComponent<DoorsController>();
            if(door.doorID == doorID)
            {
                anim = door.GetComponent<Animator>();
            }
        }

        kpcs = GameObject.FindGameObjectsWithTag("KeyPad");

        

        GameObject canvas = GameObject.FindWithTag("UICanvas");
        ui = canvas.GetComponent<UIController>();
        interactionTextHasKey = ui.FindChildObjectWithName("KeyPad Text").GetComponent<Text>();
        interactionTextNoKey = ui.FindChildObjectWithName("KeyPad No Key Text").GetComponent<Text>();
        controllerText = ui.FindChildObjectWithName("KeyPad Text Controller").GetComponent<Text>();
}

    // Update is called once per frame
    void Update()
    {
        if(!ui.IsPaused() && !ui.IsGameWon() && !ui.IsGameLost())
        {

            if (ControllerWasPressed())
            {
                usingController = true;
                usingMouse = false;
                interactionTextHasKey.gameObject.SetActive(false);
            }
            if (MouseWasMoved())
            {
                controllerText.gameObject.SetActive(false);
                usingController = false;
                usingMouse = true;
            }

            if (Vector3.Distance(transform.position, player.transform.position) < useRadius)
            {
                inRange = true;
                RaycastHit hit;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 100, keyPadMask))
                {
                    if (this.gameObject.GetInstanceID() == hit.transform.gameObject.GetInstanceID())
                    {

                        if (hasKey)
                        {
                            if(usingMouse)
                            {
                                interactionTextHasKey.gameObject.SetActive(true);
                            }
                            else if(usingController)
                            {
                                controllerText.gameObject.SetActive(true);
                            }
                            if (Input.GetAxisRaw("Interact") > 0)
                            {
                                keyPadSource.Stop();
                                keyPadSource.clip = beepClip;
                                keyPadSource.Play();
                                interactionTextHasKey.gameObject.SetActive(false);
                                controllerText.gameObject.SetActive(false);
                                OpenDoor();
                            }
                        }
                        else
                        {
                            interactionTextNoKey.gameObject.SetActive(true);
                        }

                    }
                }
                else
                {
                    interactionTextHasKey.gameObject.SetActive(false);
                    interactionTextNoKey.gameObject.SetActive(false);
                    controllerText.gameObject.SetActive(false);
                }
            }
            else
            {
                inRange = false;
            }

            bool playerInRange = false;

            for (int i = 0; i < kpcs.Length; i++)
            {
                if (kpcs[i].GetComponent<KeyPadController>().GetInRange() == true)
                    playerInRange = true;
            }

            if (!playerInRange)
            {
                interactionTextHasKey.gameObject.SetActive(false);
                interactionTextNoKey.gameObject.SetActive(false);
                controllerText.gameObject.SetActive(false);
            }
        }

    }

    private void OpenDoor()
    {
        anim.SetTrigger("Open");
        doorOpened = true;
    }

    public void SetHasKey(bool b)
    {
        hasKey = b;
    }

    public bool GetInRange()
    {
        return inRange;
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
}
