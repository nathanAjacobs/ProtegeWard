using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;


public class UIController : MonoBehaviour {

    private CurrentScore theCurrentScore;
    public Transform levelID;
    public Text scoreText;

    private OptionsValues optionsVals;

    private RawImage healthBar;

    private GameObject player;

    private PlayerController pc;

    private int pointsWhenLoaded;

    private Vector2 sizeAtStart;

    private float pauseInput; 
    private float lastPauseInput = 0;

    private bool isPaused = false;

    private GameObject hud;
    private GameObject pauseMenuPanel;

    private GameObject pauseMenu;
    private GameObject options;

    private GameObject menuConfirm;
    private GameObject quitConfirm;

    private GameObject credits;
    private GameObject highScores;

    private GameObject winMenu;
    private GameObject loseMenu;

    private GameObject howToPlayMenu;

    private bool gameLost = false;
    private bool gameWon = false;

    private bool inOptions = false;
    private bool inHowToPlay = false;
    private bool inMenuConfirm = false;
    private bool inQuitConfirm = false;
    private bool inHighScores = false;
    private bool inCredits = false;
    

    private float horizontalInput = 0;
    private float verticalInput = 0;
    private float lastHorizontalInput = 0;
    private float lastVerticalInput = 0;

    private int pauseMenuIndex = 0;
    private int optionsMenuIndex = 0;
    private int howToPlayMenuIndex = 0;
    private int menuConfirmIndex = 0;
    private int quitConfirmIndex = 0;
    private int winMenuIndex = 0;
    private int loseMenuIndex = 0;
    private int highScoresIndex = 0;
    private int creditsIndex = 0;

    private bool inLevelOne = false;
    private bool inLevelTwo = false;
    private bool inLevelThree = false;

    public AudioSource musicSource;
    public Slider mouseSensSlider;
    public Slider controllerSensSlider;
    public Slider masterSlider;
    public Slider musicSlider;
    public Slider sfxSlider;

    public AudioMixer audioMixer;

    // Use this for initialization
    void Start () {
        healthBar = FindChildObjectWithName("health bar").GetComponent<RawImage>();
        sizeAtStart = healthBar.GetComponent<RectTransform>().sizeDelta;
        player = GameObject.FindWithTag("Player");
        pc = player.GetComponent<PlayerController>();
        hud = FindChildObjectWithName("HUD");
        pauseMenuPanel = FindChildObjectWithName("Pause Menu Panel");

        pauseMenu = FindChildObjectWithName("Pause Menu");
        options = FindChildObjectWithName("Options Menu");

        menuConfirm = FindChildObjectWithName("Menu Confirm");
        quitConfirm = FindChildObjectWithName("Quit Confirm");
        credits = FindChildObjectWithName("Credits Menu");
        highScores = FindChildObjectWithName("High Scores Menu");
        winMenu = FindChildObjectWithName("Win Menu");
        loseMenu = FindChildObjectWithName("Lose Menu");
        howToPlayMenu = FindChildObjectWithName("How to Play Menu");

        optionsVals = GameObject.FindWithTag("Options").GetComponent<OptionsValues>();
        theCurrentScore = GameObject.FindWithTag("Scores").GetComponent<CurrentScore>();

        pointsWhenLoaded = theCurrentScore.points;

        mouseSensSlider.value = optionsVals.mouseSens;
        controllerSensSlider.value = optionsVals.controllerSens;
        masterSlider.value = optionsVals.masterVolume;
        musicSlider.value = optionsVals.musicVolume;
        sfxSlider.value = optionsVals.sfxVolume;



        hud.SetActive(true);
        pauseMenuPanel.SetActive(false);

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        musicSource.Play();

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
	
	// Update is called once per frame
	void Update () {
        RectTransform rt = healthBar.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2 (sizeAtStart.x * (pc.GetPlayerHealth() / 100.0f), sizeAtStart.y);

        pauseInput = Input.GetAxisRaw("Cancel");

        

        if(pauseInput > 0 && pauseInput != lastPauseInput && !isPaused && !gameLost && !gameWon)
        {
            Pause();
        }
        else if(pauseInput > 0 && pauseInput != lastPauseInput && isPaused && !gameLost && !gameWon)
        {
            Resume();
        }

        lastPauseInput = pauseInput;

        if(isPaused)
        {
            /*if(Input.GetAxisRaw("Mouse Y") != 0 || Input.GetAxisRaw("Mouse X") != 0)
            {
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
            }*/
            horizontalInput = Input.GetAxisRaw("HorizontalController");
            verticalInput = Input.GetAxisRaw("VerticalController");
            
            if(inOptions)
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
            else if(inHowToPlay)
            {
                if (lastVerticalInput == 0)
                {
                    if (verticalInput < 0)
                    {
                        howToPlayMenuIndex++;
                        if (howToPlayMenuIndex == 1)
                            howToPlayMenuIndex = 0;
                    }
                    else if (verticalInput > 0)
                    {
                        howToPlayMenuIndex--;
                        if (howToPlayMenuIndex == -1)
                            howToPlayMenuIndex = 0;
                    }
                }

                switch (howToPlayMenuIndex)
                {
                    case 0:
                        GetChildObject(howToPlayMenu.transform, "Back").GetComponent<Button>().Select();
                        break;
                }
            }
            else if (inMenuConfirm)
            {
                if (lastVerticalInput == 0)
                {
                    if (verticalInput < 0)
                    {
                        menuConfirmIndex++;
                        if (menuConfirmIndex == 2)
                            menuConfirmIndex = 0;
                    }
                    else if (verticalInput > 0)
                    {
                        menuConfirmIndex--;
                        if (menuConfirmIndex == -1)
                            menuConfirmIndex = 1;
                    }
                }

                switch (menuConfirmIndex)
                {
                    case 0:
                        GetChildObject(menuConfirm.transform, "Yes").GetComponent<Button>().Select();
                        break;
                    case 1:
                        GetChildObject(menuConfirm.transform, "No").GetComponent<Button>().Select();
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
                        pauseMenuIndex++;
                        if (pauseMenuIndex == 5)
                            pauseMenuIndex = 0;
                    }
                    else if (verticalInput > 0)
                    {
                        pauseMenuIndex--;
                        if (pauseMenuIndex == -1)
                            pauseMenuIndex = 4;
                    }
                }

                switch (pauseMenuIndex)
                {
                    case 0:
                        GetChildObject(pauseMenu.transform, "Resume").GetComponent<Button>().Select();
                        break;
                    case 1:
                        GetChildObject(pauseMenu.transform, "Options").GetComponent<Button>().Select();
                        break;
                    case 2:
                        GetChildObject(pauseMenu.transform, "How To Play").GetComponent<Button>().Select();
                        break;
                    case 3:
                        GetChildObject(pauseMenu.transform, "Main Menu").GetComponent<Button>().Select();
                        break;
                    case 4:
                        GetChildObject(pauseMenu.transform, "Quit").GetComponent<Button>().Select();
                        break;
                }
            }

            lastVerticalInput = verticalInput;
            //Debug.Log(GameObject.Find("EventSystem").GetComponent<EventSystem>().currentSelectedGameObject);

        }
        else if(gameWon)
        {
            horizontalInput = Input.GetAxisRaw("HorizontalController");
            verticalInput = Input.GetAxisRaw("VerticalController");

            /*if (Input.GetAxisRaw("Mouse Y") != 0 || Input.GetAxisRaw("Mouse X") != 0)
            {
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
            }*/

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
            else if (inHowToPlay)
            {
                if (lastVerticalInput == 0)
                {
                    if (verticalInput < 0)
                    {
                        howToPlayMenuIndex++;
                        if (howToPlayMenuIndex == 1)
                            howToPlayMenuIndex = 0;
                    }
                    else if (verticalInput > 0)
                    {
                        howToPlayMenuIndex--;
                        if (howToPlayMenuIndex == -1)
                            howToPlayMenuIndex = 0;
                    }
                }

                switch (howToPlayMenuIndex)
                {
                    case 0:
                        GetChildObject(howToPlayMenu.transform, "Back").GetComponent<Button>().Select();
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
            else if (inMenuConfirm)
            {
                if (lastVerticalInput == 0)
                {
                    if (verticalInput < 0)
                    {
                        menuConfirmIndex++;
                        if (menuConfirmIndex == 2)
                            menuConfirmIndex = 0;
                    }
                    else if (verticalInput > 0)
                    {
                        menuConfirmIndex--;
                        if (menuConfirmIndex == -1)
                            menuConfirmIndex = 1;
                    }
                }

                switch (menuConfirmIndex)
                {
                    case 0:
                        GetChildObject(menuConfirm.transform, "Yes").GetComponent<Button>().Select();
                        break;
                    case 1:
                        GetChildObject(menuConfirm.transform, "No").GetComponent<Button>().Select();
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
                        winMenuIndex++;
                        if (winMenuIndex == 7)
                            winMenuIndex = 0;
                    }
                    else if (verticalInput > 0)
                    {
                        winMenuIndex--;
                        if (winMenuIndex == -1)
                            winMenuIndex = 6;
                    }
                }

                switch (winMenuIndex)
                {
                    case 0:
                        GetChildObject(winMenu.transform, "Restart").GetComponent<Button>().Select();
                        break;
                    case 1:
                        GetChildObject(winMenu.transform, "Options").GetComponent<Button>().Select();
                        break;
                    case 2:
                        GetChildObject(winMenu.transform, "How To Play").GetComponent<Button>().Select();
                        break;
                    case 3:
                        GetChildObject(winMenu.transform, "High Scores").GetComponent<Button>().Select();
                        break;
                    case 4:
                        GetChildObject(winMenu.transform, "Credits").GetComponent<Button>().Select();
                        break;
                    case 5:
                        GetChildObject(winMenu.transform, "Main Menu").GetComponent<Button>().Select();
                        break;
                    case 6:
                        GetChildObject(winMenu.transform, "Quit").GetComponent<Button>().Select();
                        break;
                }
            }

            lastVerticalInput = verticalInput;

            

        }
        else if (gameLost)
        {
            horizontalInput = Input.GetAxisRaw("HorizontalController");
            verticalInput = Input.GetAxisRaw("VerticalController");

            /*if (Input.GetAxisRaw("Mouse Y") != 0 || Input.GetAxisRaw("Mouse X") != 0)
            {
                GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
            }*/

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
            else if (inHowToPlay)
            {
                if (lastVerticalInput == 0)
                {
                    if (verticalInput < 0)
                    {
                        howToPlayMenuIndex++;
                        if (howToPlayMenuIndex == 1)
                            howToPlayMenuIndex = 0;
                    }
                    else if (verticalInput > 0)
                    {
                        howToPlayMenuIndex--;
                        if (howToPlayMenuIndex == -1)
                            howToPlayMenuIndex = 0;
                    }
                }

                switch (howToPlayMenuIndex)
                {
                    case 0:
                        GetChildObject(howToPlayMenu.transform, "Back").GetComponent<Button>().Select();
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
            else if (inMenuConfirm)
            {
                if (lastVerticalInput == 0)
                {
                    if (verticalInput < 0)
                    {
                        menuConfirmIndex++;
                        if (menuConfirmIndex == 2)
                            menuConfirmIndex = 0;
                    }
                    else if (verticalInput > 0)
                    {
                        menuConfirmIndex--;
                        if (menuConfirmIndex == -1)
                            menuConfirmIndex = 1;
                    }
                }

                switch (menuConfirmIndex)
                {
                    case 0:
                        GetChildObject(menuConfirm.transform, "Yes").GetComponent<Button>().Select();
                        break;
                    case 1:
                        GetChildObject(menuConfirm.transform, "No").GetComponent<Button>().Select();
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
                        loseMenuIndex++;
                        if (loseMenuIndex == 7)
                            loseMenuIndex = 0;
                    }
                    else if (verticalInput > 0)
                    {
                        loseMenuIndex--;
                        if (loseMenuIndex == -1)
                            loseMenuIndex = 6;
                    }
                }

                switch (loseMenuIndex)
                {
                    case 0:
                        GetChildObject(loseMenu.transform, "Restart").GetComponent<Button>().Select();
                        break;
                    case 1:
                        GetChildObject(loseMenu.transform, "Options").GetComponent<Button>().Select();
                        break;
                    case 2:
                        GetChildObject(loseMenu.transform, "How To Play").GetComponent<Button>().Select();
                        break;
                    case 3:
                        GetChildObject(loseMenu.transform, "High Scores").GetComponent<Button>().Select();
                        break;
                    case 4:
                        GetChildObject(loseMenu.transform, "Credits").GetComponent<Button>().Select();
                        break;
                    case 5:
                        GetChildObject(loseMenu.transform, "Main Menu").GetComponent<Button>().Select();
                        break;
                    case 6:
                        GetChildObject(loseMenu.transform, "Quit").GetComponent<Button>().Select();
                        break;
                }
            }

            lastVerticalInput = verticalInput;

        }
        else
        {
            scoreText.text = theCurrentScore.points.ToString();
        }

        horizontalInput = 0;
        verticalInput = 0;
    }

    public void Pause()
    {
        musicSource.Pause();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        inOptions = false;
        inHowToPlay = false;
        inMenuConfirm = false;
        inQuitConfirm = false;
        hud.SetActive(false);
        pauseMenuPanel.SetActive(true);
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Resume()
    {
        musicSource.UnPause();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenuIndex = 0;
        GameObject.Find("EventSystem").GetComponent<EventSystem>().SetSelectedGameObject(null);
        hud.SetActive(true);
        pauseMenuPanel.SetActive(false);
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        options.SetActive(false);
        howToPlayMenu.SetActive(false);
        menuConfirm.SetActive(false);
        quitConfirm.SetActive(false);
    }

    public void ToOptions()
    {
        optionsMenuIndex = 0;
        inOptions = true;
        pauseMenu.SetActive(false);
        loseMenu.SetActive(false);
        winMenu.SetActive(false);
        options.SetActive(true);
    }

    public void BackFromOptions()
    {
        inOptions = false;
        options.SetActive(false);
        if (gameLost)
            loseMenu.SetActive(true);
        else if (gameWon)
            winMenu.SetActive(true);
        else
            pauseMenu.SetActive(true);
    }

    public void ToMenuConfirm()
    {
        menuConfirmIndex = 0;
        inMenuConfirm = true;
        pauseMenu.SetActive(false);
        loseMenu.SetActive(false);
        winMenu.SetActive(false);
        menuConfirm.SetActive(true);
    }

    public void BackFromMenuConfirm()
    {
        inMenuConfirm = false;
        menuConfirm.SetActive(false);
        if (gameLost)
            loseMenu.SetActive(true);
        else if (gameWon)
            winMenu.SetActive(true);
        else
            pauseMenu.SetActive(true);
    }

    public void ToQuitConfirm()
    {
        quitConfirmIndex = 0;
        inQuitConfirm = true;
        pauseMenu.SetActive(false);
        loseMenu.SetActive(false);
        winMenu.SetActive(false);
        quitConfirm.SetActive(true);
    }

    public void BackFromQuitConfirm()
    {
        inQuitConfirm = false;
        quitConfirm.SetActive(false);
        if (gameLost)
            loseMenu.SetActive(true);
        else if (gameWon)
            winMenu.SetActive(true);
        else
            pauseMenu.SetActive(true);
    }

    public void ToCredits()
    {
        creditsIndex = 0;
        inCredits = true;
        pauseMenu.SetActive(false);
        loseMenu.SetActive(false);
        winMenu.SetActive(false);
        credits.SetActive(true);
    }

    public void BackFromCredits()
    {
        inCredits = false;
        credits.SetActive(false);
        if (gameLost)
            loseMenu.SetActive(true);
        else if (gameWon)
            winMenu.SetActive(true);
        else
            pauseMenu.SetActive(true);
    }

    public void ToHighScores()
    {
        highScoresIndex = 0;
        inHighScores = true;
        pauseMenu.SetActive(false);
        loseMenu.SetActive(false);
        winMenu.SetActive(false);
        highScores.SetActive(true);
    }

    public void BackFromHighScores()
    {
        inHighScores = false;
        highScores.SetActive(false);
        if (gameLost)
            loseMenu.SetActive(true);
        else if (gameWon)
            winMenu.SetActive(true);
        else
            pauseMenu.SetActive(true);
    }

    public void ToHowToPlay()
    {
        howToPlayMenuIndex = 0;
        inHowToPlay = true;
        pauseMenu.SetActive(false);
        loseMenu.SetActive(false);
        winMenu.SetActive(false);
        howToPlayMenu.SetActive(true);
    }

    public void BackHowToPlay()
    {
        inHowToPlay = false;
        howToPlayMenu.SetActive(false);
        if (gameLost)
            loseMenu.SetActive(true);
        else if (gameWon)
            winMenu.SetActive(true);
        else
            pauseMenu.SetActive(true);
    }

    public void LoadMenu()
    {
        theCurrentScore.points = 0;
        SceneManager.LoadScene("Main Menu");
    }

    public void Quit()
    {
        theCurrentScore.points = 0;
        Application.Quit();
    }

    public void Restart()
    {
        if (inLevelOne)
        {
            theCurrentScore.points = 0;
            SceneManager.LoadScene("Level 1");
        }
        else if (inLevelTwo)
        {
            theCurrentScore.points = pointsWhenLoaded;
            SceneManager.LoadScene("Level 2");
        }
        else if (gameWon)
        {
            theCurrentScore.points = 0;
            SceneManager.LoadScene("Level 1");
        }
        else if (!gameWon && inLevelThree)
        {
            theCurrentScore.points = pointsWhenLoaded;
            SceneManager.LoadScene("Level 3");
        }

        Time.timeScale = 1f;
    }

    public void Lost()
    {
        musicSource.Stop();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameLost = true;
        hud.SetActive(false);
        Time.timeScale = 0f;
        pauseMenu.SetActive(false);
        pauseMenuPanel.SetActive(true);
        loseMenu.SetActive(true);
    }

    public void BeatLevelOne()
    {
        musicSource.Stop();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Loading Screen 1");
    }

    public void BeatLevelTwo()
    {
        musicSource.Stop();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadScene("Loading Screen 2");
    }

    public void Won()
    {
        musicSource.Stop();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        gameWon = true;
        hud.SetActive(false);
        Time.timeScale = 0f;
        pauseMenu.SetActive(false);
        pauseMenuPanel.SetActive(true);
        winMenu.SetActive(true);
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public bool IsGameWon()
    {
        return gameWon;
    }

    public bool IsGameLost()
    {
        return gameLost;
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
}
