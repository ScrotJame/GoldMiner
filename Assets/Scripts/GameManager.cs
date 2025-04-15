using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int currentLevel = 1;
    public int goldAmount = 0;

    private int initialTime = 15;
    public int timeLeft;
    private int pendingScore;
    private bool shouldStartNextMission = false;
    private bool returningFromShop = false;

    private Coroutine countdownCoroutine;

    private Spawner spawner;
    private Pod pod;
    public int _nextScore;
    AudioManager audioManager;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        audioManager = GameObject.FindGameObjectWithTag("Audio")?.GetComponent<AudioManager>();
        Debug.Log($"GameManager Awake - Current Level: {currentLevel}");
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        StopAllCoroutines();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);

        UIManager.instance?.HidePanels();
        spawner = FindObjectOfType<Spawner>();
        pod = FindObjectOfType<Pod>();

        if (spawner == null)
        {
            Debug.LogError("Spawner not found in the scene! Cannot proceed.");
            return;
        }
        if (pod == null)
        {
            Debug.LogError("Pod not found in the scene! Cannot proceed.");
            return;
        }

        InitializeLevel();

        if (shouldStartNextMission)
        {
            NextMission(pendingScore);
            shouldStartNextMission = false;
        }
    }

    private void InitializeLevel()
    {
        timeLeft = initialTime;
        UIManager.instance?.UpdateTime(timeLeft);
        UIManager.instance?.UpdateNumber(0);

        if (ScoreControl.instance == null)
        {
            Debug.LogError("ScoreControl instance is null! Attempting to find or create one.");
            ScoreControl.instance = FindObjectOfType<ScoreControl>();
            if (ScoreControl.instance == null)
            {
                Debug.LogError("Could not find or create ScoreControl! Game cannot proceed.");
                return;
            }
        }

        if (ScoreControl.instance.GetTargetScore() == 0)
        {
            ScoreControl.instance.SetTargetScore(1000);
        }

        StartCountdown();
    }

    private void Start()
    {
        if (currentLevel == 1)
        {
            ScoreControl.instance?.StartNewGame();
        }
    }

    private void StartCountdown()
    {
        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
        countdownCoroutine = StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        while (timeLeft > 0)
        {
            UIManager.instance?.UpdateTime(timeLeft);
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }
        UIManager.instance?.UpdateTime(0);
        CheckMissionComplete(ScoreControl.instance?.GetCurrentScore() ?? 0, timeLeft);
    }

    public void TriggerNextMission(int score)
    {
        shouldStartNextMission = true;
        pendingScore = score;
    }

    public void NextMission(int score)
    {
        Time.timeScale = 0;
        currentLevel++;
        initialTime = 15 + (currentLevel * 6);
        timeLeft = initialTime;
        _nextScore = (score > 10000) ? score / 4 + (ScoreControl.instance?.GetTargetScore() ?? 0) : score / 2 + (ScoreControl.instance?.GetTargetScore() ?? 0);
        ScoreControl.instance?.SetTargetScore(_nextScore);
        UIManager.instance?.ShowMissionPanel(_nextScore);
        StartCoroutine(HideMissionPanelAndContinue(3f));
        pod?.StopMovement();
        returningFromShop = false;
        UIManager.instance?.UpdateAdsRewardButton();
    }

    private IEnumerator HideMissionPanelAndContinue(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        ResetGameForNewLevel();
    }

    private void CheckMissionComplete(int score, int time)
    {
        if (time <= 0)
        {
            // Xử lý vàng hoặc vật phẩm đang gắp
            if (pod != null && pod._state == Pod.StateMoc._rewind && pod._transformPostion != null)
            {
                // Hủy vật phẩm mà không cộng điểm
                GoldBase gold = pod._transformPostion.GetComponent<GoldBase>();
                Mouse mouse = pod._transformPostion.GetComponent<Mouse>();
                BlindBox blindBox = pod._transformPostion.GetComponent<BlindBox>();

                if (gold != null || mouse != null || blindBox != null)
                {
                    Destroy(pod._transformPostion.gameObject);
                    Debug.Log($"Destroyed item {pod._transformPostion.name} due to time out, no points awarded");
                }

                pod._transformPostion = null;
                if (LinePod.instance != null)
                {
                    LinePod.instance.ReleaseGold();
                }
                if (pod._animHook != null)
                {
                   pod._animHook.Play("hook");
                }
            }

            pod?.ResetHookPosition();
            if (returningFromShop)
            {
                returningFromShop = false;
                Debug.Log("CheckMissionComplete: Returning from shop, skipping mission check.");
                return;
            }

            int targetScore = ScoreControl.instance?.GetTargetScore() ?? 0;
            if (score >= targetScore)
            {
                UIManager.instance?.HideButton();
                UIManager.instance?.ShowNotification("Mission Complete!");
                StartCoroutine(WaitBeforeNextMission(score));
                pod?.StopMovement();
            }
            else
            {
                UIManager.instance?.InitializeButtons();
                audioManager.background.Stop();
                audioManager.PlaySFX(audioManager.lost);
                ItemManager.Instance.ClearAllItems();
                UIManager.instance?.ShowNotification("Mission Failed! \n Do you want to try again?");
                ScoreControl.instance?.StartNewGame();
                pod?.StopMovement();
            }
        }
    }
    private IEnumerator WaitBeforeNextMission(int score)
    {
        yield return new WaitForSeconds(2f);
        UIManager.instance?.HidePanels();
        Time.timeScale = 0;
        GameObject nextMissionButton = GameObject.Find("NextMissonbutt");
        if (nextMissionButton != null)
        {
            nextMissionButton.SetActive(false);
        }
        GameObject NoButt = GameObject.Find("NoButt");
        if (NoButt != null)
        {
            NoButt.SetActive(false);
        }

        GoogleAdsManager adsManager = FindObjectOfType<GoogleAdsManager>();
        if (adsManager != null && adsManager.IsBannerAdLoaded() && currentLevel % 2 == 0)
        {
            adsManager.onAdClosedGoToShop += () =>
            {
                if (StoreManager.Instance != null)
                {
                    _ControlMusic();
                    StoreManager.Instance.ShowShop();
                }
            };
            adsManager.ShowInterstitialAd();
        }
        else
        {
            if (StoreManager.Instance != null)
            {
                _ControlMusic();
                StoreManager.Instance.ShowShop();
            }
        }
    }

    public void ResetGameForNewLevel()
    {
        spawner?.ResetLevel();
        timeLeft = initialTime;
        UIManager.instance?.UpdateTime(timeLeft);

        if (ScoreControl.instance != null)
        {
            ScoreControl.instance.InitializeScore(ScoreControl.instance.GetCurrentScore(), _nextScore);
            UIManager.instance?.UpdateNumber(ScoreControl.instance.GetCurrentScore());
        }
        else
        {
            Debug.LogWarning("ScoreControl instance is null, cannot initialize score!");
        }

        pod?.ResumeMovement();
        Time.timeScale = 1;
        StartCountdown();
        UIManager.instance?.UpdateAdsRewardButton();
    }

    public void HomeButton()
    {
        UIManager.instance.ClearUIReferences();
        UIManager.instance.HidePanels();
        ItemManager.Instance.ClearAllItems();
        SceneManager.LoadScene("MainMenu");
    }

    public void PlayAgainButton()
    {
        ScoreControl.instance?.PlayAgainGame();
        UIManager.instance?.HidePanels();
    }

    public void PlayButton()
    {
        UIManager.instance?.HidePanels();
        Time.timeScale = 1;
        StartCountdown();
    }

    public void ContinueButton()
    {
        UIManager.instance?.HidePanels();
        Time.timeScale = 1;
        StartCountdown();
    }

    public void StopButton()
    {
        Time.timeScale = 0;
        UIManager.instance?.menuGamePanel?.SetActive(true);
        StopCountdown();
    }

    private void StopCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }
    }

    public void PauseGameForShop()
    {
        pod?.StopMovement();
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }
    }

    public void _ControlMusic()
    {
        audioManager.background.Stop();
    }

    public void ResumeGameAfterShop()
    {
        audioManager.background.Play();
        if (pod == null || pod.Equals(null))
        {
            Debug.LogWarning("Pod is null in ResumeGameAfterShop!");
            return;
        }

        pod?.ResumeMovement();
        pod.ResetHookPosition();
        UIManager.instance?.HidePanels();
        Time.timeScale = 1;
        timeLeft = initialTime;
        UIManager.instance?.UpdateTime(timeLeft);
        returningFromShop = true;

        if (ScoreControl.instance != null)
        {
            NextMission(ScoreControl.instance.GetCurrentScore());
        }
        else
        {
            Debug.LogError("ScoreControl.instance is null in ResumeGameAfterShop!");
            return;
        }
    }

    public void CollectGold(int amount)
    {
        goldAmount += amount;
        ScoreControl.instance?.AddScore(amount);
    }

    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
        StopAllCoroutines();
    }

    public void AddTime(float additionalTime)
    {
        timeLeft += (int)additionalTime;
        UIManager.instance?.UpdateTime(timeLeft);
    }
}