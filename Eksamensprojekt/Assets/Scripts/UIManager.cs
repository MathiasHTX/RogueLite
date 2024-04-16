using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public static event Action<bool> isPaused;

    [SerializeField] Image damageVignette;

    [SerializeField] GameObject gameUI;
    [SerializeField] GameObject crossHair;

    [SerializeField] TextMeshProUGUI pressEText;
    [SerializeField] GameObject pressE;

    [Header("Spaceship")]
    [SerializeField] CanvasGroup cantEnterShip;
    [SerializeField] CanvasGroup holdToExit;
    [SerializeField] Image holdToExitCircle;

    [Header("Intro")]
    [SerializeField] Image whiteFade;
    [SerializeField] RectTransform blackBarTop;
    [SerializeField] RectTransform blackBarBottom;

    [Header("Health bar")]
    [SerializeField] Transform healthBar;
    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] Image healthBarFill;
    [SerializeField] Gradient healthBarGradient;
    int startHealth;

    [Header("Enemies killed")]
    [SerializeField] TextMeshProUGUI enemiesKilledText;
    [SerializeField] RectTransform enemiesKilledTransform;

    [Header("New wave")]
    [SerializeField] GameObject newWave;
    [SerializeField] TextMeshProUGUI waveText;
    [SerializeField] RectTransform waveBackground;
    [SerializeField] TextMeshProUGUI currentWaveText;

    [Header("Wave complete")]
    [SerializeField] GameObject waveComplete;
    [SerializeField] RectTransform waveCompletedBackground;
    [SerializeField] TextMeshProUGUI waveCompletedText;

    [Header("Time before wave")]
    bool timerEnabled;
    [SerializeField] RectTransform timeBeforeNewWaveTransform;
    [SerializeField] TextMeshProUGUI timerText;

    [Header("Game over")]
    [SerializeField] GameObject gameOver;
    [SerializeField] RectTransform gameOverBackground;
    [SerializeField] TextMeshProUGUI gameOverText;

    [Header("Pause screen")]
    [SerializeField] RectTransform pauseScreen;
    [SerializeField] GameObject holdMenuMessage;
    [SerializeField] Image holdMenuRing;
    bool paused;

    [Header("Crafting table")]
    [SerializeField] RectTransform craftingTableRect;
    bool craftingTableOpen;


    bool sceneIsHome;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        sceneIsHome = SceneManager.GetActiveScene().buildIndex == 2;

        PlayerMovementAdvanced.onDeath += PlayerMovementAdvanced_onDeath;

        ShipLandingSequence.OnExitShip += ShipLandingSequence_OnExitShip;
        if (!sceneIsHome)
            gameUI.SetActive(false);

        pressE.SetActive(false);
        whiteFade.gameObject.SetActive(true);
        whiteFade.DOFade(0, 2).OnComplete(() => whiteFade.gameObject.SetActive(false));

        if (craftingTableRect != null)
            craftingTableRect.gameObject.SetActive(false);

        holdMenuMessage.SetActive(false);
        crossHair.SetActive(false);
    }

    private void ShipLandingSequence_OnExitShip()
    {
        if(!sceneIsHome)
        {
            LoadGameUI();
        }

        pressE.SetActive(false);

        blackBarBottom.DOAnchorPosY(-20, 1);
        blackBarTop.DOAnchorPosY(20, 1);

        crossHair.SetActive(true);
    }

    void LoadGameUI()
    {
        PlayerMovementAdvanced.onPlayerHit += PlayerMovementAdvanced_onPlayerHit;
        healthBarFill.color = healthBarGradient.Evaluate(1);
        startHealth = PlayerMovementAdvanced.instance.GetStartHealth();
        healthText.text = "HP " + startHealth;
        gameUI.SetActive(true);
    }

    public void ShowPressEText(string message)
    {
        pressEText.text = message;

        pressE.SetActive(true);
    }

    public void HidePressEText()
    {
        pressE.SetActive(false);
    }

    private void PlayerMovementAdvanced_onPlayerHit(int health)
    {
        float fillAmount = (float)health / startHealth;

        float duration = 0.2f;
        healthBarFill.DOFillAmount(fillAmount, duration);
        healthBar.DOShakePosition(duration, 4, 30);
        healthBarFill.DOColor(healthBarGradient.Evaluate(fillAmount), duration);

        healthText.text = "HP " + health;

        damageVignette.DOFade(0.5f, 0.1f).OnComplete(() => damageVignette.DOFade(0f, 0.3f));
    }

    public void UpdateEnemiesKilledUI(int enemiesKilled, int totalEnemies)
    {
        enemiesKilledText.text = enemiesKilled + "/" + totalEnemies;
        enemiesKilledTransform.DOKill();
        enemiesKilledTransform.DOScale(1.1f, 0.1f).SetLoops(2, LoopType.Yoyo);
    }

    public void NewWave(int waveCount)
    {
        currentWaveText.text = "WAVE " + (waveCount + 1);

        // Start values
        waveText.text = "WAVE " + (waveCount + 1);
        waveBackground.sizeDelta = new Vector2(0, 25);
        waveText.DOFade(0, 0);
        newWave.SetActive(true);

        // Appear
        waveBackground.DOSizeDelta(new Vector2(142, 25), 0.5f).SetEase(Ease.InOutCubic);
        waveText.transform.localPosition = new Vector2(8, -3);
        waveText.DOFade(1, 0.5f);
        waveText.transform.DOLocalMoveX(-17, 0.5f).SetEase(Ease.InOutCubic);

        // Disappear
        waveBackground.DOSizeDelta(new Vector2(0, 25), 0.5f).SetEase(Ease.InOutCubic).SetDelay(3);
        waveText.transform.DOLocalMoveX(8, 0.5f).SetEase(Ease.InOutCubic).SetDelay(3);
        waveText.DOFade(0, 0.5f).SetDelay(3).OnComplete(() => newWave.SetActive(false));
    }

    public void WaveComplete()
    {
        // Start values
        waveCompletedBackground.sizeDelta = new Vector2(0, 30);
        waveCompletedText.transform.localPosition = new Vector2(0, -7);
        waveCompletedText.DOFade(0, 0);
        waveComplete.SetActive(true);

        // Appear
        waveCompletedText.DOFade(1, 0.3f).SetDelay(0.3f);
        waveCompletedText.transform.DOLocalMoveY(0, 0.3f).SetDelay(0.3f);
        waveCompletedBackground.DOSizeDelta(new Vector2(180, 30), 0.5f).SetEase(Ease.InOutCubic);

        // Disappear
        waveCompletedText.DOFade(0, 0.3f).SetDelay(3.3f);
        waveCompletedText.transform.DOLocalMoveY(-7, 0.3f).SetDelay(3.3f);
        waveCompletedBackground.DOSizeDelta(new Vector2(0, 30), 0.5f).SetEase(Ease.InOutCubic).SetDelay(3f).OnComplete(() => waveComplete.SetActive(false));
    }

    private void PlayerMovementAdvanced_onDeath()
    {
        GameOver();
    }

    public void GameOver()
    {
        // Start values
        gameOverBackground.sizeDelta = new Vector2(0, 45);
        gameOverText.transform.localPosition = new Vector2(0, -7);
        gameOverText.DOFade(0, 0);
        gameOver.SetActive(true);

        //Appear
        gameOverText.DOFade(1, 0.3f).SetDelay(0.3f);
        gameOverText.transform.DOLocalMoveY(0, 0.3f).SetDelay(0.3f);
        gameOverBackground.DOSizeDelta(new Vector2(256, 45), 0.5f).SetEase(Ease.InOutCubic);

        // Disappear
        gameOverText.DOFade(0, 0.3f).SetDelay(3.3f);
        gameOverText.transform.DOLocalMoveY(-7, 0.3f).SetDelay(3.3f);
        gameOverBackground.DOSizeDelta(new Vector2(0, 45), 0.5f).SetEase(Ease.InOutCubic).SetDelay(3f);

    }

    private void Update()
    {
        if (timerEnabled)
        {
            timerText.text = WaveManager.instance.GetTimer().ToString("F0") + "s";
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    public void OpenTimeBeforeNewWave()
    {
        Debug.Log("OpenTime");
        timerEnabled = true;
        timeBeforeNewWaveTransform.anchoredPosition = new Vector2(120, -35);
        timeBeforeNewWaveTransform.gameObject.SetActive(true);
        timeBeforeNewWaveTransform.DOAnchorPosX(-150, 1).SetDelay(1);
        enemiesKilledTransform.DOAnchorPosX(120, 1).OnComplete(() => enemiesKilledTransform.gameObject.SetActive(false));
    }

    public void OpenEnemiesKilled()
    {
        timerEnabled = false;
        enemiesKilledTransform.anchoredPosition = new Vector2(-32, -35);
        enemiesKilledTransform.gameObject.SetActive(true);
        enemiesKilledTransform.DOAnchorPosX(-150, 1).SetDelay(1);
        timeBeforeNewWaveTransform.DOAnchorPosX(120, 1).OnComplete(() => timeBeforeNewWaveTransform.gameObject.SetActive(false));
    }

    void PauseGame()
    {
        if (!paused && !craftingTableOpen && ShipLandingSequence.instance.HasExitedShip())
        {
            UIAudio.instance.PlayOpenSound();
            pauseScreen.anchoredPosition = new Vector2(-170, 0);
            pauseScreen.DOAnchorPosX(130, 0.3f).SetUpdate(true);
            pauseScreen.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isPaused?.Invoke(true);
            Time.timeScale = 0f;
            paused = true;
        }
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseScreen.DOAnchorPosX(-170, 0.3f).OnComplete(() => pauseScreen.gameObject.SetActive(false));
        isPaused?.Invoke(false);
        paused = false;
        UIAudio.instance.PlayCloseSound();
    }

    public void ShowCantEnterShipUI()
    {
        cantEnterShip.gameObject.SetActive(true);
        cantEnterShip.DOFade(1, 0.2f);
        cantEnterShip.DOFade(0, 0.5f).SetDelay(2).OnComplete(() => cantEnterShip.gameObject.SetActive(false));
    }
    
    public void ShowHoldToLeave()
    {
        holdToExitCircle.fillAmount = 0;
        holdToExit.gameObject.SetActive(true);
        holdToExit.DOFade(1, 0.2f);
        holdToExitCircle.DOFillAmount(1, 2).OnComplete(()=> Spaceship.instance.OpenSpaceship());
    }

    public void HideHoldToLeave()
    {
        holdToExitCircle.DOKill();
        holdToExit.DOFade(0, 0.2f).OnComplete(() => holdToExit.gameObject.SetActive(false));
    }

    public void ShowHoldMenu()
    {
        holdMenuMessage.SetActive(true);
        holdMenuRing.fillAmount = 0;
        holdMenuRing.DOFillAmount(1, 3).SetUpdate(true).OnComplete(() => {
            Time.timeScale = 1;
            SceneManager.LoadScene(0);
        });
    }

    public void HideHoldMenu()
    {
        holdMenuRing.DOKill();
        holdMenuMessage.SetActive(false);
    }

    public void OpenCraftingUI()
    {
        craftingTableRect.localScale = new Vector2(1, 0);
        craftingTableRect.gameObject.SetActive(true);
        craftingTableRect.DOScaleY(1, 0.2f);
        craftingTableOpen = true;
        UIAudio.instance.PlayOpenSound();
    }

    public void CloseCraftingUI()
    {
        UIAudio.instance.PlayCloseSound();
        craftingTableRect.DOScaleY(0, 0.2f).OnComplete(() => {
            craftingTableRect.gameObject.SetActive(false);
            craftingTableOpen = false;
        });
    }
}
