using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Text _Time, Number, _Notif, BestScoreTxt;
    public int currentLevel = 1;
    public int goldAmount = 0;

    private int initialTime = 60;
    private int timeLeft;
    private Coroutine countdownCoroutine;
    [SerializeField]
    private GameObject _MenuGamePanel, _GameNotifPanel, _MissionPanel;
    private Text _MissionTargetText;

    private ScoreControl scoreManager;
    private Spawner spawner;
    private Pod pod;
    private int _basetime;
    private int _nextScore;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(_MissionPanel);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        InitializeLevel();
        StartCountdown();
        ActivePanel();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GamePlay")
        {
            _Time = GameObject.Find("Time")?.GetComponent<Text>();
            Number = GameObject.Find("SL")?.GetComponent<Text>();
            _Notif = GameObject.Find("NotiText")?.GetComponent<Text>();
            _MissionTargetText = GameObject.Find("MissiontargetText")?.GetComponent<Text>();

            _MissionPanel = GameObject.Find("NotifMission");
            _GameNotifPanel = GameObject.Find("Notifice");
            _MenuGamePanel = GameObject.Find("MenuPause");

            scoreManager = FindObjectOfType<ScoreControl>();
            spawner = FindObjectOfType<Spawner>();
            pod = FindObjectOfType<Pod>();

            if (scoreManager == null) Debug.LogError("ScoreControl not found in scene!");
        }
    }

    private void InitializeLevel()
    {
        _MissionPanel.SetActive(true);
        timeLeft = initialTime;
        _SetTime(timeLeft);
        if (scoreManager != null && currentLevel == 1)
            scoreManager.InitializeScore(0, 10);
        _SetNumber();
        StartCoroutine(HideMissionPanelAfterDelay(3f)); 
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
            if (_Time != null)
                _Time.text = timeLeft.ToString("F0");
            else
                yield break;

            yield return new WaitForSeconds(1f);
            timeLeft--;
        }
        _Time.text = "0";
        CheckMissionComplete(scoreManager?.GetCurrentScore() ?? 0, timeLeft);
    }

    private void NextMission(int score)
    {
        Time.timeScale = 0;
        _basetime = timeLeft;
        currentLevel ++;
        _basetime += 15;
        if (score > 10000)
        {
            _nextScore = score / 4 + (scoreManager?.GetTargetScore() ?? 0);
        }
        else
        {
            _nextScore = score / 2 + (scoreManager?.GetTargetScore() ?? 0);
        }
        if (_MissionPanel != null)
        {
            _MissionPanel.SetActive(true);
            if (_MissionTargetText != null)
            {
                _MissionTargetText.text = $" {_nextScore}";
            }
            StartCoroutine(HideMissionPanelAndContinue(3f));
        }
        else
        {
            Debug.LogError("_MissionPanel is null!");
        }
    }

    private IEnumerator HideMissionPanelAndContinue(float delay)
    {
        yield return new WaitForSecondsRealtime(delay); 
        _MissionPanel?.SetActive(false);
        ResetGameForNewLevel();
    }

    private void CheckMissionComplete(int score, int time)
    {
        if (time <= 0 && score >= (scoreManager?.GetTargetScore() ?? 0))
        {
            _Notif.text = "Mission Complete!";
            NextMission(score); 
        }
        else if (time <= 0)
        {
            _Notif.text = "Mission Failed! \n Do you want to try again?";
            scoreManager?.InitializeScore(0);
            Time.timeScale = 0;

            if (_GameNotifPanel != null) _GameNotifPanel.SetActive(true);
        }
    }

    private void StopCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }
    }

    private void ResetGameForNewLevel()
    {
        ActivePanel();
        spawner?.ResetLevel();
        initialTime += 15;
        _SetTime(initialTime); if (scoreManager != null)
            scoreManager.InitializeScore(scoreManager.GetCurrentScore(), _nextScore);
        _SetNumber();
        Time.timeScale = 1;
        StartCountdown();
    }

    public void _HomeButton() => SceneManager.LoadScene("MainMenu");
    public void _PlayAgainButton() => SceneManager.LoadScene("GamePlay");
    public void _PlayButton() { ActivePanel(); Time.timeScale = 1; StartCountdown(); }
    public void _ContinueButton() { ActivePanel(); Time.timeScale = 1; StartCountdown(); }
    public void _StopButton() { Time.timeScale = 0; _MenuGamePanel?.SetActive(true); StopCountdown(); }

    private void _SetTime(int time) { timeLeft = time; if (_Time != null) _Time.text = time.ToString(); }
    private void _SetNumber() { if (Number != null) Number.text = "0"; }
    public void _getNumber(int number) { if (Number != null) Number.text = number.ToString(); }

    private void ActivePanel()
    {
        _MenuGamePanel?.SetActive(false);
        _GameNotifPanel?.SetActive(false);
        _MissionPanel?.SetActive(false);
    }

    private IEnumerator HideMissionPanelAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        _MissionPanel?.SetActive(false);
    }
}