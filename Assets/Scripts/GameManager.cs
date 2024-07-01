using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    // UI Elements
    [SerializeField] Button singleStartBtn, multiStartBtn;
    [SerializeField] Button gameOverQuitBtn, resetBtn;
    [SerializeField] Button settingsBtn, pauseSettingsBtn;
    [SerializeField] Button backBtn, exitBtn, quitBtn;
    [SerializeField] Button resumeBtn;
    [SerializeField] TextMeshProUGUI scoreText1, scoreText2, winText;
    [SerializeField] TextMeshProUGUI survivalBonusText1, survivalBonusText2;
    [SerializeField] TextMeshProUGUI volumeText, sfxText, difficultyText;
    [SerializeField] TextMeshProUGUI invertedStatusText;
    [SerializeField] Slider volumeSlider, sfxSlider;
    [SerializeField] Button leftToggle, rightToggle;
    [SerializeField] Button leftInvertedToggle, rightInvertedToggle;
    // Game Objects & Components
    [SerializeField] GameObject titleScreen, gameScreen, settingsScreen;
    GameObject settingsReference;   // for more accurate transitions from the settings
    GameObject[] obstacles;
    [SerializeField] GameObject difficulty;
    [SerializeField] GameObject player2, p2Screen;
    [SerializeField] GameObject gameOverScreen, pauseScreen;
    [SerializeField] List<GameObject> hearts, hearts2;
    [SerializeField] Material red, blue, black;
    new AudioSource audio;
    Animator player1Animator, player2Animator;
    // Game Values
    int difficultyIndex = 1;
    int volumePct, sfxPct;
    bool isActive = false;
    readonly List<string> difficulties = new();
    // SFX Volume Property
    public float SFXVolume {get; set;}
    // Game Active Property
    public bool GameActive {get; set;}
    // Inverted Status Property
    public int InvertedStatus {get; set;}
    // Player 1 Score Property
    public int P1Score {get; set;}
    // Player 2 Score Property
    public int P2Score {get; set;}
    // Player 1 Lives Property
    public int P1Lives {get; set;}
    // Player 2 Lives Property
    public int P2Lives {get; set;}
    // Start is called before the first frame update
    void Start()
    {
        // Declare Property  & Component Values
        difficulties.Add("Easy");
        difficulties.Add("Medium");
        difficulties.Add("Hard");
        P1Score = P2Score = 0;
        InvertedStatus = 1;
        audio = GameObject.Find("Main Camera").GetComponent<AudioSource>();
        player1Animator = GameObject.Find("Player 1").GetComponent<Animator>();
        player2Animator = GameObject.Find("Player 2").GetComponent<Animator>();
        // Add Button & Slider Event Listeners
        singleStartBtn.onClick.AddListener(() =>
        {
            p2Screen.SetActive(false);
            player2.SetActive(false);
            StartGame();
        });
        multiStartBtn.onClick.AddListener(() =>
        {
            p2Screen.SetActive(true);
            player2.SetActive(true);
            StartGame();
        });
        gameOverQuitBtn.onClick.AddListener(BackToTitle);
        settingsBtn.onClick.AddListener(ToSettings);
        pauseSettingsBtn.onClick.AddListener(ToSettings);
        backBtn.onClick.AddListener(BackFromSettings);
        resumeBtn.onClick.AddListener(PauseGame);
        resetBtn.onClick.AddListener(RestartGame);
        exitBtn.onClick.AddListener(ExitGame);
        quitBtn.onClick.AddListener(BackToTitle);
        volumeSlider.onValueChanged.AddListener(ChangeVolume);
        sfxSlider.onValueChanged.AddListener(ChangeObstacleVolume);
        leftToggle.onClick.AddListener(ReduceDifficulty);
        rightToggle.onClick.AddListener(IncreaseDifficulty);
        leftInvertedToggle.onClick.AddListener(SetInvertedControls);
        rightInvertedToggle.onClick.AddListener(SetInvertedControls);
    }
    // Go to Settings from either the title or pause menu
    void ToSettings()
    {
        if (titleScreen.activeSelf)
        {
            titleScreen.SetActive(false);
            settingsReference = titleScreen;
        }
        else
        {
            pauseScreen.SetActive(false);
            settingsReference = pauseScreen;
        }
        difficulty.SetActive(settingsReference == titleScreen);
        settingsScreen.SetActive(true);
    }
    // Set the Volume by sliding the percentage
    void ChangeVolume(float volume)
    {
        volumePct = Mathf.RoundToInt(volume * 100);
        volumeText.SetText($"{volumePct}%");
        audio.volume = volume;
    }
    // Set the Volume of obstacle hits by sliding the percentage
    void ChangeObstacleVolume(float volume)
    {
        SFXVolume = volume;
        sfxText.SetText($"{Mathf.RoundToInt(SFXVolume * 100)}");
    }
    // Set the difficulty of the game
    void SetGameDifficulty(int index)
    {
        difficultyText.SetText(difficulties[index % 3]);
    }
    // Set the Inverted Controls Status
    void SetInvertedControls()
    {
        isActive = !isActive;
        invertedStatusText.text = (isActive) ? "On" : "Off";
        InvertedStatus = (isActive) ? -1 : 1;
    }
    // Set the game's active status
    void SetGameActive()
    {
        GameActive = !GameActive;
        gameScreen.SetActive(GameActive);
    }
    // Scroll through the difficulty levels to the left
    void ReduceDifficulty()
    {
        difficultyIndex = (difficultyIndex == 0)? 3 : difficultyIndex;
        SetGameDifficulty(--difficultyIndex);
    }
    // Scroll through the difficulty levels to the right
    void IncreaseDifficulty()
    {
        difficultyIndex = (difficultyIndex == difficulties.Count - 1) ? 0 : difficultyIndex;
        SetGameDifficulty(++difficultyIndex);
    }

    // Go back from the Settings to either the title or pause menu
    void BackFromSettings()
    {
        settingsScreen.SetActive(false);
        settingsReference.SetActive(true);
    }
    // Start the game after pressing either the start or restart button
    void StartGame()
    {
        SetMaxLives();
        SetGameActive();
        titleScreen.SetActive(!GameActive);
    }
    /* Pause the game to take a break after pressing spacebar or the resume 
       button */
    void PauseGame()
    {
        SetGameActive();
        pauseScreen.SetActive(!GameActive);
    }
    // Restart the game to go again
    void RestartGame()
    {
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        foreach (GameObject obstacle in obstacles)
        {
            Destroy(obstacle);
        }
        GameObject.Find("Player 1").transform.position = new(-2, 0, 0);
        player1Animator.SetBool("Death_b", false);
        if (player2.activeSelf)
        {
            GameObject.Find("Player 2").transform.position = new(2, 0, 0);
            player2Animator.SetBool("Death_b", false);
        }
        EndGame();
        SetMaxLives();
        P1Score = P2Score = 0;
        scoreText1.SetText($"Score: {P1Score}");
        scoreText2.SetText($"Score: {P2Score}");
    }
    // End the game after no lives left with the game over screen
    public void EndGame()
    {
        GameActive = !GameActive;
        SetWin();
        gameOverScreen.SetActive(!GameActive);
    }
    // Exit out of the game entirely
    void ExitGame()
    {
        Application.Quit();
    }
    // Go back to title screen after pressing the reset button
    void BackToTitle()
    {
        GameActive = !GameActive;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    // Display which player wins before ending the game if multiplayer is active
    void SetWin()
    {
        if (p2Screen.activeSelf)
        {
            if (P2Score > P1Score)
            {
                winText.text = "Player 2 Wins!";
            }
            else if (P1Score > P2Score)
            {
                winText.text = "Player 1 Wins!";
            }
            else
            {
                winText.text = "Game Tied!";
            }
        }
        winText.gameObject.SetActive(p2Screen.activeSelf);
    }
    /* Increase the score by 1 after collecting a flag while displaying the new
       score */
    public void CollectFlag(int player)
    {
        switch (player)
        {
            case 1:
                scoreText1.SetText($"x {++P1Score}");
                break;
            case 2:
                scoreText2.SetText($"x {++P2Score}");
                break;
        }
    }
    // Set the max number of hearts while starting a game
    void SetMaxLives()
    {
        P1Lives = P2Lives = 1 + 2 * (2 - difficulties.IndexOf(difficultyText.text));
        for (int index = 0; index < hearts.Count; index++)
        {
            hearts[index].SetActive(index < P1Lives);
            hearts2[index].SetActive(index < P1Lives);
            hearts[index].GetComponent<Renderer>().material = red;
            hearts2[index].GetComponent<Renderer>().material = blue;
        }
        survivalBonusText1.text = $"Survival Bonus: {P1Lives}";
        survivalBonusText2.text = $"Survival Bonus: {P2Lives}";
    }
    // Update the hearts' materials based on the number of lives remaining
    public void UpdateLives(int player)
    {
        switch (player)
        {
            case 1:
                hearts[P1Lives].GetComponent<Renderer>().material = black;
                survivalBonusText1.text = $"Survival Bonus: {P1Lives}";
                break;
            case 2:
                hearts2[P2Lives].GetComponent<Renderer>().material = black;
                survivalBonusText2.text = $"Survival Bonus: {P2Lives}";
                break;
        }
    }
    // Update is called one per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }
}
