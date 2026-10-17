using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    // =========================================================
    // PANELS
    // =========================================================

    [Header("Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject youWinPanel;
    [SerializeField] private GameObject youDiedPanel;


    // =========================================================
    // MAIN MENU
    // =========================================================

    [Header("Main Menu")]
    [SerializeField] private TMP_Text levelNumberText;

    [Header("Music Buttons")]
    [SerializeField] private GameObject musicOnButton;
    [SerializeField] private GameObject musicOffButton;


    // =========================================================
    // GAME PANEL - SCORE
    // =========================================================

    [Header("Score")]
    [SerializeField] private TMP_Text allScoreText;
    [SerializeField] private TMP_Text simpleScoreText;
    [SerializeField] private TMP_Text bonusScoreText;


    // =========================================================
    // HEALTH
    // =========================================================

    [Header("Health")]
    [SerializeField] private Image healthBarImage;
    [SerializeField] private TMP_Text healthScoreText;


    // =========================================================
    // POWER BAR
    // =========================================================

    [Header("Power Bar")]
    [SerializeField] private GameObject powerBar;
    [SerializeField] private Image powerBarImage;
    [SerializeField] private TMP_Text powerScoreText;


    // =========================================================
    // PLAYER
    // =========================================================

    [Header("Player")]
    [SerializeField] private PlayerCollect playerCollect;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private PlayerPowerPallet playerPowerPallet;


    // =========================================================
    // LEVEL SETTINGS
    // =========================================================

    [Header("Level Settings")]
    [SerializeField] private int currentLevel = 1;
    [SerializeField] private int targetScore = 30;

    [SerializeField] private int totalSimpleCollectibles = 10;
    [SerializeField] private int totalBonusCollectibles = 2;


    // =========================================================
    // STATE
    // =========================================================

    private bool gameEnded = false;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        Time.timeScale = 1f;

        UpdateLevelText();
        LoadMusicState();

        // Power bar o'yin boshida yashirin
        HidePowerBar();

        ShowMainMenu();
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (gameEnded)
            return;

        UpdateScoreUI();
        UpdateHealthUI();
        UpdatePowerUI();

        CheckWin();
        CheckDeath();
    }


    // =========================================================
    // MAIN MENU
    // =========================================================

    public void StartGame()
    {
        gameEnded = false;

        mainMenuPanel.SetActive(false);
        gamePanel.SetActive(true);
        pausePanel.SetActive(false);
        youWinPanel.SetActive(false);
        youDiedPanel.SetActive(false);

        // Har safar game boshlanganda power bar yashirin
        HidePowerBar();

        Time.timeScale = 1f;
    }


    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);

        gamePanel.SetActive(false);
        pausePanel.SetActive(false);
        youWinPanel.SetActive(false);
        youDiedPanel.SetActive(false);

        Time.timeScale = 0f;
    }


    // =========================================================
    // QUIT
    // =========================================================

    public void QuitGame()
    {
        Debug.Log("Game Quit");

        Application.Quit();
    }


    // =========================================================
    // PAUSE
    // =========================================================

    public void PauseGame()
    {
        if (gameEnded)
            return;

        gamePanel.SetActive(false);
        pausePanel.SetActive(true);

        Time.timeScale = 0f;
    }


    // =========================================================
    // RESUME
    // =========================================================

    public void ResumeGame()
    {
        if (gameEnded)
            return;

        pausePanel.SetActive(false);
        gamePanel.SetActive(true);

        Time.timeScale = 1f;
    }


    // =========================================================
    // RESTART
    // =========================================================

    public void RestartGame()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            SceneManager.GetActiveScene().buildIndex
        );
    }


    // =========================================================
    // NEXT LEVEL
    // =========================================================

    public void NextLevel()
    {
        Time.timeScale = 1f;

        int nextScene =
            SceneManager.GetActiveScene().buildIndex + 1;

        if (nextScene < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.Log("Next level mavjud emas.");
        }
    }


    // =========================================================
    // SCORE UI
    // =========================================================

    private void UpdateScoreUI()
    {
        if (playerCollect == null)
            return;

        int allScore =
            playerCollect.GetScore();

        int simpleScore =
            playerCollect.GetSimpleScore();

        int bonusScore =
            playerCollect.GetBonusScore();

        allScoreText.text =
            allScore + " / " + targetScore;

        simpleScoreText.text =
            simpleScore + " / " +
            totalSimpleCollectibles;

        bonusScoreText.text =
            bonusScore + " / " +
            totalBonusCollectibles;
    }


    // =========================================================
    // HEALTH UI
    // =========================================================

    private void UpdateHealthUI()
    {
        if (playerHealth == null)
            return;

        float currentHealth =
            playerHealth.GetCurrentHealth();

        float maxHealth =
            playerHealth.GetMaxHealth();

        float healthPercent =
            currentHealth / maxHealth;

        healthBarImage.fillAmount =
            healthPercent;

        healthScoreText.text =
            Mathf.RoundToInt(currentHealth) + "%";
    }


    // =========================================================
    // POWER UI
    // =========================================================

    private void UpdatePowerUI()
    {
        if (playerPowerPallet == null)
        {
            HidePowerBar();
            return;
        }

        float remainingTime =
            playerPowerPallet.GetRemainingPowerTime();

        float duration =
            playerPowerPallet.GetPowerDuration();

        // Power aktiv emas
        if (!playerPowerPallet.IsPoweredUp())
        {
            HidePowerBar();
            return;
        }

        // Power aktiv
        ShowPowerBar();

        if (duration > 0f)
        {
            powerBarImage.fillAmount =
                remainingTime / duration;
        }

        powerScoreText.text =
            Mathf.CeilToInt(remainingTime) + "s";
    }


    // =========================================================
    // SHOW POWER BAR
    // =========================================================

    private void ShowPowerBar()
    {
        if (powerBar != null)
        {
            powerBar.SetActive(true);
        }
    }


    // =========================================================
    // HIDE POWER BAR
    // =========================================================

    private void HidePowerBar()
    {
        if (powerBar != null)
        {
            powerBar.SetActive(false);
        }
    }


    // =========================================================
    // WIN
    // =========================================================

    private void CheckWin()
    {
        if (playerCollect == null)
            return;

        if (!gamePanel.activeSelf)
            return;

        int score =
            playerCollect.GetScore();

        if (score >= targetScore)
        {
            ShowWinPanel();
        }
    }


    public void ShowWinPanel()
    {
        gameEnded = true;

        gamePanel.SetActive(false);
        pausePanel.SetActive(false);
        youWinPanel.SetActive(true);

        HidePowerBar();

        Time.timeScale = 0f;
    }


    // =========================================================
    // DEATH
    // =========================================================

    private void CheckDeath()
    {
        if (playerHealth == null)
            return;

        if (!gamePanel.activeSelf)
            return;

        if (playerHealth.GetCurrentHealth() <= 0f)
        {
            ShowDiedPanel();
        }
    }


    public void ShowDiedPanel()
    {
        gameEnded = true;

        gamePanel.SetActive(false);
        pausePanel.SetActive(false);
        youDiedPanel.SetActive(true);

        HidePowerBar();

        Time.timeScale = 0f;
    }


    // =========================================================
    // MUSIC ON
    // =========================================================

    public void TurnMusicOn()
    {
        AudioListener.pause = false;

        PlayerPrefs.SetInt("MusicMuted", 0);
        PlayerPrefs.Save();

        musicOnButton.SetActive(true);
        musicOffButton.SetActive(false);
    }


    // =========================================================
    // MUSIC OFF
    // =========================================================

    public void TurnMusicOff()
    {
        AudioListener.pause = true;

        PlayerPrefs.SetInt("MusicMuted", 1);
        PlayerPrefs.Save();

        musicOnButton.SetActive(false);
        musicOffButton.SetActive(true);
    }


    // =========================================================
    // LOAD MUSIC STATE
    // =========================================================

    private void LoadMusicState()
    {
        int muted =
            PlayerPrefs.GetInt("MusicMuted", 0);

        if (muted == 1)
        {
            AudioListener.pause = true;

            musicOnButton.SetActive(false);
            musicOffButton.SetActive(true);
        }
        else
        {
            AudioListener.pause = false;

            musicOnButton.SetActive(true);
            musicOffButton.SetActive(false);
        }
    }


    // =========================================================
    // LEVEL TEXT
    // =========================================================

    private void UpdateLevelText()
    {
        levelNumberText.text =
            currentLevel.ToString();
    }
}