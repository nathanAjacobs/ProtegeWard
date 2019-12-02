using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyController : MonoBehaviour {

    public float pickupRadius = 3f;
    public string keyPadID = "green";
    public AudioSource audioSource;

    private GameObject player;
    private LayerMask keyMask;
    private Text interactionText;
    private Text controllerText;

    private List<KeyPadController> kpcs = new List<KeyPadController>();

    private static List<KeyController> keyList = new List<KeyController>();

    private List<RawImage> uiKeys = new List<RawImage>();

    private Camera cam;

    private bool inRange = false;

    private bool usingController = false;
    private bool usingMouse = false;

    private UIController ui;

	// Use this for initialization
	void Start () {
        player = GameObject.FindWithTag("Player");
        cam = player.GetComponentInChildren<Camera>();
        keyMask = LayerMask.GetMask("Keys");
        GameObject canvas = GameObject.FindWithTag("UICanvas");
        ui = canvas.GetComponent<UIController>();
        interactionText = ui.FindChildObjectWithName("Key Text").GetComponent<Text>();
        controllerText = ui.FindChildObjectWithName("Key Text Controller").GetComponent<Text>();

        GameObject[] keyPads = GameObject.FindGameObjectsWithTag("KeyPad");
        

        keyList.Add(this);

        for(int i = 0; i < keyPads.Length; i++)
        {
            KeyPadController kp = keyPads[i].GetComponent<KeyPadController>();
            if (kp.keyPadID == keyPadID)
            {
                kpcs.Add(kp);
            }
        }

        GameObject[] keysUI = new GameObject[4];

        keysUI[0] = ui.FindChildObjectWithName("blue key");
        keysUI[1] = ui.FindChildObjectWithName("purple key");
        keysUI[2] = ui.FindChildObjectWithName("yellow key");
        keysUI[3] = ui.FindChildObjectWithName("green key");

        //Debug.Log(keysUI.Length);
        for (int i = 0; i < keysUI.Length; i++)
        {
            RawImage image = keysUI[i].GetComponent<RawImage>();
            UIKeyController imageController = image.gameObject.GetComponent<UIKeyController>();
            if (imageController.keyID == keyPadID)
            {
                uiKeys.Add(image);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {

        if(!ui.IsPaused() && !ui.IsGameWon() && !ui.IsGameLost())
        {
            if (ControllerWasPressed())
            {
                usingController = true;
                usingMouse = false;
                interactionText.gameObject.SetActive(false);
            }
            if(MouseWasMoved())
            {
                controllerText.gameObject.SetActive(false);
                usingController = false;
                usingMouse = true;
            }

            //Debug.Log(Vector3.Distance(transform.position, player.transform.position));
            if (Vector3.Distance(transform.position, player.transform.position) < pickupRadius)
            {
                inRange = true;
                RaycastHit hit;
                if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, 3, keyMask))
                {
                    if (this.gameObject.GetInstanceID() == hit.transform.parent.gameObject.GetInstanceID())
                    {
                        if(usingMouse)
                        {
                            interactionText.gameObject.SetActive(true);
                        }
                        else if(usingController)
                        {
                            controllerText.gameObject.SetActive(true);
                        }
                        
                        if (Input.GetAxisRaw("Interact") > 0)
                        {
                            controllerText.gameObject.SetActive(false);
                            interactionText.gameObject.SetActive(false);
                            keyList.Remove(this);
                            audioSource.Play();
                            Object.Destroy(this.gameObject);
                            foreach (RawImage img in uiKeys)
                            {
                                img.gameObject.SetActive(true);
                            }
                            foreach (KeyPadController kpc in kpcs)
                            {
                                kpc.SetHasKey(true);
                            }
                        }
                    }
                }
                else
                {
                    controllerText.gameObject.SetActive(false);
                    interactionText.gameObject.SetActive(false);
                }
            }
            else
            {
                inRange = false;
            }

            bool playerInRange = false;

            foreach (KeyController k in keyList)
            {
                if (k.GetInRange() == true)
                    playerInRange = true;
            }

            if (!playerInRange)
            {
                interactionText.gameObject.SetActive(false);
                controllerText.gameObject.SetActive(false);
            }
        }
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
