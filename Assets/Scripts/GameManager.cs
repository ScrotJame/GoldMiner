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

    private Coroutine countdownCoroutine;

    private Spawner spawner;
    private Pod pod;
    public  int _nextScore;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);

        if (scene.name == "GamePlay")
        {
            UIManager.instance.HidePanels();
            spawner = FindObjectOfType<Spawner>();
            pod = FindObjectOfType<Pod>();

            InitializeLevel();

            if (shouldStartNextMission)
            {
                NextMission(pendingScore);
                shouldStartNextMission = false;
            }
        }
    }

    private void InitializeLevel()
    {
        timeLeft = initialTime;
        UIManager.instance.UpdateTime(timeLeft);
        UIManager.instance.UpdateNumber(0);
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
            UIManager.instance.UpdateTime(timeLeft);
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }

        UIManager.instance.UpdateTime(0);
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

        int baseTime = timeLeft + 15;
        _nextScore = (score > 10000) ? score / 4 + (ScoreControl.instance?.GetTargetScore() ?? 0) : score / 2 + (ScoreControl.instance?.GetTargetScore() ?? 0);

        ScoreControl.instance?.SetTargetScore(_nextScore);
        UIManager.instance.ShowMissionPanel(_nextScore);

        StartCoroutine(HideMissionPanelAndContinue(3f));
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

            if (score >= (ScoreControl.instance?.GetTargetScore() ?? 0))
            {
                UIManager.instance.ShowNotification("Mission Complete!");
                SceneManager.LoadScene("Store");
                NextMission(score);
            }
            else
            {
                UIManager.instance.ShowNotification("Mission Failed! \n Do you want to try again?");
                ScoreControl.instance?.StartNewGame();
                Time.timeScale = 0;
            }
        }
    }

    private void ResetGameForNewLevel()
    {
        spawner?.ResetLevel();
        initialTime += 15;
        UIManager.instance.UpdateTime(initialTime);
        ScoreControl.instance?.InitializeScore(ScoreControl.instance.GetCurrentScore(), _nextScore);
        UIManager.instance.UpdateNumber(0);
        Time.timeScale = 1;
        StartCountdown();
    }

    public void HomeButton() => SceneManager.LoadScene("MainMenu");
    public void PlayAgainButton() => SceneManager.LoadScene("GamePlay");
    public void PlayButton() { UIManager.instance.HidePanels(); Time.timeScale = 1; StartCountdown(); }
    public void ContinueButton() { UIManager.instance.HidePanels(); Time.timeScale = 1; StartCountdown(); }
    public void StopButton() { Time.timeScale = 0; UIManager.instance.menuGamePanel?.SetActive(true); StopCountdown(); }

    private void StopCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }
    }
}
