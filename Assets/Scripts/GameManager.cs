using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public int currentLevel = 1;
    public int goldAmount = 0;

    private int initialTime = 20;
    private int timeLeft;
    private int pendingScore;
    private bool shouldStartNextMission = false;
    private bool returningFromShop = false;

    private Coroutine countdownCoroutine;

    private Spawner spawner;
    private Pod pod;
    public int _nextScore;

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
        initialTime = 20 + (currentLevel * 6); 
        timeLeft = initialTime;
        _nextScore = (score > 10000) ? score / 4 + (ScoreControl.instance?.GetTargetScore() ?? 0) : score / 2 + (ScoreControl.instance?.GetTargetScore() ?? 0);

        Debug.Log($"NextMission: score = {score}, _nextScore = {_nextScore}, initialTime = {initialTime}, currentLevel = {currentLevel}");
        ScoreControl.instance?.SetTargetScore(_nextScore);
        UIManager.instance?.ShowMissionPanel(_nextScore);
        StartCoroutine(HideMissionPanelAndContinue(3f));
        pod?.StopMovement();
        returningFromShop = false;
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
                UIManager.instance?.ShowNotification("Mission Complete!");
                StartCoroutine(WaitBeforeNextMission(score));
                pod?.StopMovement();
            }
            else
            {
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

        if (StoreManager.Instance != null)
        {
            StoreManager.Instance.ShowShop();
        }
        else
        {
            Debug.LogError("WaitBeforeNextMission: StoreManager.Instance is null, cannot show shop!");
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
    }

    public void HomeButton() => SceneManager.LoadScene("MainMenu");
    public void PlayAgainButton() { ScoreControl.instance?.StartNewGame(); SceneManager.LoadScene("GamePlay"); UIManager.instance.HidePanels(); }
    public void PlayButton() { UIManager.instance?.HidePanels(); Time.timeScale = 1; StartCountdown(); }
    public void ContinueButton() { UIManager.instance?.HidePanels(); Time.timeScale = 1; StartCountdown(); }
    public void StopButton() { Time.timeScale = 0; UIManager.instance?.menuGamePanel?.SetActive(true); StopCountdown(); }

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

    public void ResumeGameAfterShop()
    {
        if (pod == null || pod.Equals(null))
        {
        //    pod = FindObjectOfType<Pod>();
            if (pod == null)
            {
                return;
            }
        }

        pod?.ResumeMovement();
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
            return;
        }

        StartCountdown();
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
}