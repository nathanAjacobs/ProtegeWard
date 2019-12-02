using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenuController : MonoBehaviour {

    private GameObject mainMenuPanel;

    private GameObject mainMenu;
    private GameObject options;
    private List<int> highScoreValues = new List<int>();
    private GameObject quitConfirm;
    private GameObject credits;
    private GameObject highScores;

    private float horizontalInput = 0;
    private float verticalInput = 0;
    private float lastVerticalInput = 0;


    private bool inOptions = false;
    private bool inHighScores = false;
    private bool inCredits = false;
    private bool inQuitConfirm = false;

    private int mainMenuIndex = 0;
    private int optionsMenuIndex = 0;
    private int creditsIndex = 0;
    private int quitConfirmIndex = 0;
    private int highScoresIndex = 0;

    public Slider mouseSensSlider;
    public Slider controllerSensSlider;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    public Text[] highScoresText;

    public AudioMixer audioMixer;
    private OptionsValues optionsVals;

    // Use this for initialization
    void Start()
    {
        mainMenuPanel = FindChildObjectWithName("Main Menu Panel");
        mainMenu = FindChildObjectWithName("Main Menu");
        options = FindChildObjectWithName("Options Menu");
        quitConfirm = FindChildObjectWithName("Quit Confirm");
        credits = FindChildObjectWithName("Credits Menu");
        highScores = FindChildObjectWithName("High Scores Menu");
        mainMenuPanel.SetActive(true);
        mainMenu.SetActive(true);

        optionsVals = GameObject.FindWithTag("Options").GetComponent<OptionsValues>();

        mouseSensSlider.value = optionsVals.mouseSens;
        controllerSensSlider.value = optionsVals.controllerSens;
        masterSlider.value = optionsVals.masterVolume;
        musicSlider.value = optionsVals.musicVolume;
        sfxSlider.value = optionsVals.sfxVolume;

        GetScores();
        UpdateCanvasScore();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("HorizontalController");
        verticalInput = Input.GetAxisRaw("VerticalController");

        if (inOptions)
        {
            if (lastVerticalInput == 0)
            {
                if (verticalInput < 0)
                {
                    optionsMenuIndex++;
                    if (optionsMenuIndex == 1)
                        optionsMenuIndex = 0;
                }
                else if (verticalInput > 0)
                {
                    optionsMenuIndex--;
                    if (optionsMenuIndex == -1)
                        optionsMenuIndex = 0;
                }
            }

            switch (optionsMenuIndex)
            {
                case 0:
                    GetChildObject(options.transform, "Back").GetComponent<Button>().Select();
                    break;
            }
        }
        else if (inHighScores)
        {
            if (lastVerticalInput == 0)
            {
                if (verticalInput < 0)
                {
                    highScoresIndex++;
                    if (highScoresIndex == 1)
                        highScoresIndex = 0;
                }
                else if (verticalInput > 0)
                {
                    highScoresIndex--;
                    if (highScoresIndex == -1)
                        highScoresIndex = 0;
                }
            }

            switch (highScoresIndex)
            {
                case 0:
                    GetChildObject(highScores.transform, "Back").GetComponent<Button>().Select();
                    break;
            }
        }
        else if (inCredits)
        {
            if (lastVerticalInput == 0)
            {
                if (verticalInput < 0)
                {
                    creditsIndex++;
                    if (creditsIndex == 1)
                        creditsIndex = 0;
                }
                else if (verticalInput > 0)
                {
                    creditsIndex--;
                    if (creditsIndex == -1)
                        creditsIndex = 0;
                }
            }

            switch (creditsIndex)
            {
                case 0:
                    GetChildObject(credits.transform, "Back").GetComponent<Button>().Select();
                    break;
            }
        }
        else if (inQuitConfirm)
        {
            if (lastVerticalInput == 0)
            {
                if (verticalInput < 0)
                {
                    quitConfirmIndex++;
                    if (quitConfirmIndex == 2)
                        quitConfirmIndex = 0;
                }
                else if (verticalInput > 0)
                {
                    quitConfirmIndex--;
                    if (quitConfirmIndex == -1)
                        quitConfirmIndex = 1;
                }
            }

            switch (quitConfirmIndex)
            {
                case 0:
                    GetChildObject(quitConfirm.transform, "Yes").GetComponent<Button>().Select();
                    break;
                case 1:
                    GetChildObject(quitConfirm.transform, "No").GetComponent<Button>().Select();
                    break;
            }
        }
        else
        {
            if (lastVerticalInput == 0)
            {
                if (verticalInput < 0)
                {
                    mainMenuIndex++;
                    if (mainMenuIndex == 5)
                        mainMenuIndex = 0;
                }
                else if (verticalInput > 0)
                {
                    mainMenuIndex--;
                    if (mainMenuIndex == -1)
                        mainMenuIndex = 4;
                }
            }

            switch (mainMenuIndex)
            {
                case 0:
                    GetChildObject(mainMenu.transform, "Play").GetComponent<Button>().Select();
                    break;
                case 1:
                    GetChildObject(mainMenu.transform, "Options").GetComponent<Button>().Select();
                    break;
                case 2:
                    GetChildObject(mainMenu.transform, "High Scores").GetComponent<Button>().Select();
                    break;
                case 3:
                    GetChildObject(mainMenu.transform, "Credits").GetComponent<Button>().Select();
                    break;
                case 4:
                    GetChildObject(mainMenu.transform, "Quit").GetComponent<Button>().Select();
                    break;
            }
        }

        lastVerticalInput = verticalInput;
    }

    public void Play()
    {
        SceneManager.LoadScene("Loading Screen");
        Time.timeScale = 1f;
    }

    public void ToOptions()
    {
        optionsMenuIndex = 0;
        inOptions = true;
        mainMenu.SetActive(false);
        options.SetActive(true);
    }

    public void BackFromOptions()
    {
        inOptions = false;
        options.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ToCredits()
    {
        inCredits = true;
        creditsIndex = 0;
        mainMenu.SetActive(false);
        credits.SetActive(true);
    }

    public void BackFromCredits()
    {
        inCredits = false;
        credits.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ToHighScores()
    {
        highScoresIndex = 0;
        inHighScores = true;
        mainMenu.SetActive(false);
        highScores.SetActive(true);
    }

    public void BackFromHighScores()
    {
        inHighScores = false;
        highScores.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void ToQuitConfirm()
    {
        quitConfirmIndex = 0;
        inQuitConfirm = true;
        mainMenu.SetActive(false);
        quitConfirm.SetActive(true);
    }

    public void BackFromQuitConfirm()
    {
        inQuitConfirm = false;
        quitConfirm.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void Quit()
    {
        Application.Quit();
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

    public void SetMouseSens(float f)
    {
        optionsVals.mouseSens = f;
    }


    public void SetControllerSens(float f)
    {
        optionsVals.controllerSens = f;
    }

    public void SetMasterVolume(float f)
    {
        optionsVals.masterVolume = f;
        audioMixer.SetFloat("master_volume", f);
    }

    public void SetMusicVolume(float f)
    {
        optionsVals.musicVolume = f;
        audioMixer.SetFloat("music_volume", f);
    }

    public void SetSfxVolume(float f)
    {
        optionsVals.sfxVolume = f;
        audioMixer.SetFloat("sfx_volume", f);
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

    public void UpdateCanvasScore()
    {
        int i = 0;

        foreach (int f in highScoreValues)
        {
            //Debug.Log(i);
            highScoresText[i].text = f.ToString();
            highScoresText[i].enabled = true;
            i++;
        }
    }
}
