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
    // AUDIO
    // =========================================================

    [Header("Audio Sources")]
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioSource soundEffectSource;


    [Header("Audio Clips")]
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip youWinSound;
    [SerializeField] private AudioClip youDiedSound;


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

        HidePowerBar();

        StopBackgroundMusic();

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

        HidePowerBar();

        Time.timeScale = 1f;

        // Game boshlanganda music
        PlayBackgroundMusic();
    }


    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);

        gamePanel.SetActive(false);
        pausePanel.SetActive(false);
        youWinPanel.SetActive(false);
        youDiedPanel.SetActive(false);

        StopBackgroundMusic();

        Time.timeScale = 0f;
    }


    // =========================================================
    // QUIT
    // =========================================================

    public void QuitGame()
    {
        PlayButtonClick();

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

        PlayButtonClick();

        gamePanel.SetActive(false);
        pausePanel.SetActive(true);

        StopBackgroundMusic();

        Time.timeScale = 0f;
    }


    // =========================================================
    // RESUME
    // =========================================================

    public void ResumeGame()
    {
        if (gameEnded)
            return;

        PlayButtonClick();

        pausePanel.SetActive(false);
        gamePanel.SetActive(true);

        Time.timeScale = 1f;

        PlayBackgroundMusic();
    }


    // =========================================================
    // RESTART
    // =========================================================

    public void RestartGame()
    {
        PlayButtonClick();

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
        PlayButtonClick();

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

        if (!playerPowerPallet.IsPoweredUp())
        {
            HidePowerBar();
            return;
        }

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
    // POWER BAR SHOW / HIDE
    // =========================================================

    private void ShowPowerBar()
    {
        if (powerBar != null)
            powerBar.SetActive(true);
    }


    private void HidePowerBar()
    {
        if (powerBar != null)
            powerBar.SetActive(false);
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
        if (gameEnded)
            return;

        gameEnded = true;

        gamePanel.SetActive(false);
        pausePanel.SetActive(false);
        youWinPanel.SetActive(true);

        HidePowerBar();

        StopBackgroundMusic();

        // You Win audio - faqat bir marta
        PlayYouWinSound();

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
        if (gameEnded)
            return;

        gameEnded = true;

        gamePanel.SetActive(false);
        pausePanel.SetActive(false);
        youDiedPanel.SetActive(true);

        HidePowerBar();

        StopBackgroundMusic();

        // You Died audio - faqat bir marta
        PlayYouDiedSound();

        Time.timeScale = 0f;
    }


    // =========================================================
    // BUTTON CLICK SOUND
    // =========================================================

    public void PlayButtonClick()
    {
        if (soundEffectSource == null)
            return;

        if (buttonClickSound == null)
            return;

        soundEffectSource.PlayOneShot(buttonClickSound);
    }


    // =========================================================
    // YOU WIN SOUND
    // =========================================================

    private void PlayYouWinSound()
    {
        if (soundEffectSource == null)
            return;

        if (youWinSound == null)
            return;

        soundEffectSource.PlayOneShot(youWinSound);
    }


    // =========================================================
    // YOU DIED SOUND
    // =========================================================

    private void PlayYouDiedSound()
    {
        if (soundEffectSource == null)
            return;

        if (youDiedSound == null)
            return;

        soundEffectSource.PlayOneShot(youDiedSound);
    }


    // =========================================================
    // BACKGROUND MUSIC
    // =========================================================

    private void PlayBackgroundMusic()
    {
        if (backgroundMusicSource == null)
            return;

        if (backgroundMusicSource.mute)
            return;

        if (!backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Play();
        }
    }


    private void StopBackgroundMusic()
    {
        if (backgroundMusicSource == null)
            return;

        backgroundMusicSource.Stop();
    }


    // =========================================================
    // MUSIC ON
    // =========================================================

    public void TurnMusicOn()
    {
        PlayButtonClick();

        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.mute = false;
        }

        PlayerPrefs.SetInt("MusicMuted", 0);
        PlayerPrefs.Save();

        musicOnButton.SetActive(true);
        musicOffButton.SetActive(false);

        // Faqat GamePanel ochiq bo'lsa music boshlanadi
        if (gamePanel.activeSelf &&
            Time.timeScale > 0f)
        {
            PlayBackgroundMusic();
        }
    }


    // =========================================================
    // MUSIC OFF
    // =========================================================

    public void TurnMusicOff()
    {
        PlayButtonClick();

        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.mute = true;
            backgroundMusicSource.Stop();
        }

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

        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.mute =
                muted == 1;
        }

        if (muted == 1)
        {
            musicOnButton.SetActive(false);
            musicOffButton.SetActive(true);
        }
        else
        {
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