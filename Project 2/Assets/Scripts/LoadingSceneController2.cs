using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingSceneController2 : MonoBehaviour {

    private GameObject mainMenuPanel;

    private GameObject mainMenu;


    private float horizontalInput = 0;
    private float verticalInput = 0;
    private float lastVerticalInput = 0;




    private int mainMenuIndex = 0;


    // Use this for initialization
    void Start()
    {
        mainMenuPanel = FindChildObjectWithName("Main Menu Panel");
        mainMenu = FindChildObjectWithName("Main Menu");

        mainMenuPanel.SetActive(true);
        mainMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("HorizontalController");
        verticalInput = Input.GetAxisRaw("VerticalController");

        if (lastVerticalInput == 0)
        {
            if (verticalInput < 0)
            {
                mainMenuIndex++;
                if (mainMenuIndex == 1)
                    mainMenuIndex = 0;
            }
            else if (verticalInput > 0)
            {
                mainMenuIndex--;
                if (mainMenuIndex == -1)
                    mainMenuIndex = 0;
            }
        }

        switch (mainMenuIndex)
        {
            case 0:
                GetChildObject(mainMenu.transform, "Play").GetComponent<Button>().Select();
                break;
        }

        lastVerticalInput = verticalInput;
    }

    public void Play()
    {
        SceneManager.LoadScene("Level 3");
        Time.timeScale = 1f;
    }

    // method to find child game object with given tag
    public GameObject FindChildObjectWithName(string name)
    {
        Transform parent = transform;
        return GetChildObject(parent, name);
    }

    // helper method to find child game object with given tag
    private GameObject GetChildObject(Transform parent, string name)
    {
        if (parent.childCount > 0)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                Transform child = parent.GetChild(i);
                if (child.name.Equals(name))
                {
                    return child.gameObject;
                }
                if (child.childCount > 0)
                {
                    GameObject temp = GetChildObject(child, name);
                    if (temp != null)
                        return temp;
                }
            }
        }
        return null;
    }
}
