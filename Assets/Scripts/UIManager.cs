using System.Collections;
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
    // SCORE
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
    // POWER
    // =========================================================

    [Header("Power")]
    [SerializeField] private GameObject powerBar;
    [SerializeField] private Image powerBarImage;
    [SerializeField] private TMP_Text powerScoreText;

    // =========================================================
    // PLAYER REFERENCES
    // =========================================================

    [Header("Player References")]
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
    // BACKGROUND MUSIC
    // =========================================================

    [Header("Background Music")]
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioClip backgroundMusicClip;

    // =========================================================
    // UI AUDIO
    // =========================================================

    [Header("UI Audio")]
    [SerializeField] private AudioSource uiAudioSource;
    [SerializeField] private AudioClip buttonClickSound;
    [SerializeField] private AudioClip youWinSound;
    [SerializeField] private AudioClip youDiedSound;

    [Tooltip("Click tovushi eshitilishi uchun sahna yuklashdan oldingi kutish (soniya)")]
    [SerializeField] private float sceneLoadDelay = 0.2f;

    // =========================================================
    // STATE
    // =========================================================

    private bool gameEnded = false;
    private bool musicMuted = false;

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        Time.timeScale = 1f;

        SetupAudio();
        UpdateLevelText();
        LoadMusicState();
        HidePowerBar();
        OpenMainMenu();
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
    // AUDIO SETUP
    // =========================================================

    private void SetupAudio()
    {
        // UI Audio
        if (uiAudioSource != null)
        {
            uiAudioSource.playOnAwake = false;
            uiAudioSource.loop = false;
            uiAudioSource.spatialBlend = 0f;
            uiAudioSource.volume = 1f;
            uiAudioSource.ignoreListenerPause = true;
        }

        // Background Music
        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.playOnAwake = false;
            backgroundMusicSource.loop = true;
            backgroundMusicSource.spatialBlend = 0f;
            backgroundMusicSource.volume = 1f;

            if (backgroundMusicClip != null)
                backgroundMusicSource.clip = backgroundMusicClip;
        }
    }

    // =========================================================
    // MAIN MENU
    // =========================================================

    private void OpenMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (gamePanel != null) gamePanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (youWinPanel != null) youWinPanel.SetActive(false);
        if (youDiedPanel != null) youDiedPanel.SetActive(false);

        HidePowerBar();
        StopBackgroundMusic();

        Time.timeScale = 0f;
    }

    // =========================================================
    // START GAME
    // =========================================================

    public void StartGame()
    {
        PlayButtonClick();

        gameEnded = false;

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(true);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (youWinPanel != null) youWinPanel.SetActive(false);
        if (youDiedPanel != null) youDiedPanel.SetActive(false);

        HidePowerBar();

        Time.timeScale = 1f;

        PlayBackgroundMusic();
    }

    // =========================================================
    // QUIT
    // =========================================================

    public void QuitGame()
    {
        PlayButtonClick();

        Debug.Log("Quit Game");

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

        if (gamePanel != null) gamePanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(true);

        // Stop emas, Pause: Resume'da musiqa davom etadi
        PauseBackgroundMusic();

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

        if (pausePanel != null) pausePanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(true);

        Time.timeScale = 1f;

        PlayBackgroundMusic();
    }

    // =========================================================
    // RESTART
    // =========================================================

    public void RestartGame()
    {
        PlayButtonClick();

        StartCoroutine(
            LoadSceneDelayed(SceneManager.GetActiveScene().buildIndex)
        );
    }

    // =========================================================
    // MAIN MENU BUTTON
    // =========================================================

    public void ShowMainMenu()
    {
        PlayButtonClick();

        gameEnded = false;

        OpenMainMenu();
    }

    // =========================================================
    // NEXT LEVEL
    // =========================================================

    public void NextLevel()
    {
        PlayButtonClick();

        int nextScene = SceneManager.GetActiveScene().buildIndex + 1;

        if (nextScene < SceneManager.sceneCountInBuildSettings)
        {
            StartCoroutine(LoadSceneDelayed(nextScene));
        }
        else
        {
            Debug.Log("Next Level mavjud emas.");
        }
    }

    // =========================================================
    // SCENE LOAD (click tovushi kesilib qolmasligi uchun)
    // =========================================================

    private IEnumerator LoadSceneDelayed(int sceneIndex)
    {
        // timeScale 0 bo'lsa ham ishlaydi
        yield return new WaitForSecondsRealtime(sceneLoadDelay);

        Time.timeScale = 1f;

        SceneManager.LoadScene(sceneIndex);
    }

    // =========================================================
    // SCORE UI
    // =========================================================

    private void UpdateScoreUI()
    {
        if (playerCollect == null)
            return;

        int allScore = playerCollect.GetScore();
        int simpleScore = playerCollect.GetSimpleScore();
        int bonusScore = playerCollect.GetBonusScore();

        if (allScoreText != null)
            allScoreText.text = allScore + " / " + targetScore;

        if (simpleScoreText != null)
            simpleScoreText.text = simpleScore + " / " + totalSimpleCollectibles;

        if (bonusScoreText != null)
            bonusScoreText.text = bonusScore + " / " + totalBonusCollectibles;
    }

    // =========================================================
    // HEALTH UI
    // =========================================================

    private void UpdateHealthUI()
    {
        if (playerHealth == null)
            return;

        float currentHealth = playerHealth.GetCurrentHealth();
        float maxHealth = playerHealth.GetMaxHealth();

        if (maxHealth <= 0f)
            return;

        float healthPercent = currentHealth / maxHealth;

        if (healthBarImage != null)
            healthBarImage.fillAmount = healthPercent;

        if (healthScoreText != null)
            healthScoreText.text = Mathf.RoundToInt(currentHealth) + "%";
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

        if (!playerPowerPallet.IsPoweredUp())
        {
            HidePowerBar();
            return;
        }

        ShowPowerBar();

        float remainingTime = playerPowerPallet.GetRemainingPowerTime();
        float duration = playerPowerPallet.GetPowerDuration();

        if (powerBarImage != null && duration > 0f)
            powerBarImage.fillAmount = remainingTime / duration;

        if (powerScoreText != null)
            powerScoreText.text = Mathf.CeilToInt(remainingTime) + "s";
    }

    private void ShowPowerBar()
    {
        if (powerBar != null)
            powerBar.SetActive(true);
    }

    private void HidePowerBar()
    {
        if (powerBar != null)
            powerBar.SetActive(false);

        if (powerBarImage != null)
            powerBarImage.fillAmount = 0f;

        if (powerScoreText != null)
            powerScoreText.text = "0s";
    }

    // =========================================================
    // WIN
    // =========================================================

    private void CheckWin()
    {
        if (playerCollect == null)
            return;

        if (gamePanel == null || !gamePanel.activeSelf)
            return;

        if (playerCollect.GetScore() >= targetScore)
            ShowWinPanel();
    }

    public void ShowWinPanel()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (youDiedPanel != null) youDiedPanel.SetActive(false);
        if (youWinPanel != null) youWinPanel.SetActive(true);

        HidePowerBar();
        StopBackgroundMusic();

        // Faqat 1 marta
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

        if (gamePanel == null || !gamePanel.activeSelf)
            return;

        if (playerHealth.GetCurrentHealth() <= 0f)
            ShowDiedPanel();
    }

    public void ShowDiedPanel()
    {
        if (gameEnded)
            return;

        gameEnded = true;

        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(false);
        if (pausePanel != null) pausePanel.SetActive(false);
        if (youWinPanel != null) youWinPanel.SetActive(false);
        if (youDiedPanel != null) youDiedPanel.SetActive(true);

        HidePowerBar();
        StopBackgroundMusic();

        // Faqat 1 marta
        PlayYouDiedSound();

        Time.timeScale = 0f;
    }

    // =========================================================
    // UI AUDIO
    // =========================================================

    public void PlayButtonClick()
    {
        PlayUISound(buttonClickSound, "Button Click Sound");
    }

    private void PlayYouWinSound()
    {
        PlayUISound(youWinSound, "You Win Sound");
    }

    private void PlayYouDiedSound()
    {
        PlayUISound(youDiedSound, "You Died Sound");
    }

    private void PlayUISound(AudioClip clip, string clipName)
    {
        if (uiAudioSource == null)
        {
            Debug.LogError("UI Audio Source ulanmagan!");
            return;
        }

        if (clip == null)
        {
            Debug.LogError(clipName + " ulanmagan!");
            return;
        }

        uiAudioSource.PlayOneShot(clip);
    }

    // =========================================================
    // BACKGROUND MUSIC
    // =========================================================

    private void PlayBackgroundMusic()
    {
        if (backgroundMusicSource == null)
        {
            Debug.LogError("Background Music Source ulanmagan!");
            return;
        }

        if (backgroundMusicSource.clip == null)
        {
            Debug.LogError("Background Music Clip ulanmagan!");
            return;
        }

        if (backgroundMusicSource.mute)
            return;

        if (backgroundMusicSource.isPlaying)
            return;

        // Pauza qilingan bo'lsa davom ettiradi
        backgroundMusicSource.UnPause();

        // Stop qilingan bo'lsa boshidan boshlaydi
        if (!backgroundMusicSource.isPlaying)
            backgroundMusicSource.Play();
    }

    private void PauseBackgroundMusic()
    {
        if (backgroundMusicSource != null)
            backgroundMusicSource.Pause();
    }

    private void StopBackgroundMusic()
    {
        if (backgroundMusicSource == null)
            return;

        if (backgroundMusicSource.isPlaying)
            backgroundMusicSource.Stop();
        else
            backgroundMusicSource.Stop(); // pauza holatini ham tozalaydi
    }

    // =========================================================
    // MUSIC ON / OFF
    // =========================================================

    public void ToggleMusic()
    {
        PlayButtonClick();
        SetMusicMuted(!musicMuted);
    }

    private void SetMusicMuted(bool muted)
    {
        musicMuted = muted;

        if (backgroundMusicSource != null)
        {
            backgroundMusicSource.mute = muted;

            if (muted)
                backgroundMusicSource.Stop();
        }

        PlayerPrefs.SetInt("MusicMuted", muted ? 1 : 0);
        PlayerPrefs.Save();

        UpdateMusicButtons();

        // O'yin davomida yoqilsa, darhol chalinsin
        if (!muted &&
            gamePanel != null &&
            gamePanel.activeSelf &&
            Time.timeScale > 0f)
        {
            PlayBackgroundMusic();
        }
    }

    private void UpdateMusicButtons()
    {
        // musicOnButton = "musiqa yoqilgan" ikonkasi (ovoz bor)
        // musicOffButton = "musiqa o'chirilgan" ikonkasi (ovoz yo'q)
        if (musicOnButton != null)
            musicOnButton.SetActive(!musicMuted);

        if (musicOffButton != null)
            musicOffButton.SetActive(musicMuted);
    }

    private void LoadMusicState()
    {
        musicMuted = PlayerPrefs.GetInt("MusicMuted", 0) == 1;

        if (backgroundMusicSource != null)
            backgroundMusicSource.mute = musicMuted;

        UpdateMusicButtons();
    }

    // =========================================================
    // LEVEL TEXT
    // =========================================================

    private void UpdateLevelText()
    {
        if (levelNumberText != null)
            levelNumberText.text = currentLevel.ToString();
    }
}