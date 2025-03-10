using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControll : MonoBehaviour
{
    public static GameControll instance;

    public Text _Time, Number, _Notif, BestScoreTxt;
    private int initialTime = 30;
    private int timeLeft;
    private Coroutine countdownCoroutine;
    private GameObject _MenuGamePanel, _GameNotifPanel, _MissionPanel;
    private Text _MissionTargetText;

    public bool isAlive = true;
    public int count = 0;
    public Spawner spawner;
    public ScoreControl scoreManager;
    private Pod pod;

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

        SceneManager.sceneLoaded += OnSceneLoaded;
        InitializeReferences();
    }

    private void Start()
    {
        InitializeLevel();
        StartCountdown();
        ActivePanel();
    }

    private void InitializeReferences()
    {
        scoreManager = FindObjectOfType<ScoreControl>();
        spawner = FindObjectOfType<Spawner>();
        pod = FindObjectOfType<Pod>();

        if (scoreManager == null) Debug.LogError("⚠️ ScoreControl not found in scene!");
        if (spawner == null) Debug.LogError("⚠️ Spawner not found in scene!");
        if (pod == null) Debug.LogError("⚠️ Pod not found in scene!");

        scoreManager?.LoadPlayerData();
    }

    private void InitializeLevel()
    {
        timeLeft = initialTime;
        _SetTime(timeLeft);
        scoreManager?.InitializeScore(0, 10);
        _SetNumber();
    }

    private void StartCountdown()
    {
        if (countdownCoroutine != null) StopCoroutine(countdownCoroutine);
        countdownCoroutine = StartCoroutine(Countdown());
    }

    private void StopCountdown()
    {
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }
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

    private void CheckMissionComplete(int score, int time)
    {
        if (time <= 0 && score >= scoreManager?.GetTargetScore())
        {
            _Notif.text = "Mission Complete!";
            StopCountdown();
            Time.timeScale = 0;
            SceneManager.LoadScene("Store");

            if (_MissionPanel != null)
            {
                _MissionPanel.SetActive(true);
                if (_MissionTargetText != null) _MissionTargetText.text = scoreManager?.GetTargetScore().ToString();
                StartCoroutine(HideMissionPanelAfterDelay(5f));
            }
            else
            {
                Debug.LogWarning("⚠️ _MissionPanel không tồn tại trong Scene!");
            }
        }
        else if (time <= 0)
        {
            _Notif.text = "Mission Failed! \n Do you want to try again?";
            scoreManager?.InitializeScore(0);
            Time.timeScale = 0;

            if (_GameNotifPanel != null) _GameNotifPanel.SetActive(true);
        }
    }

    private IEnumerator HideMissionPanelAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);

        if (_MissionPanel != null)
        {
            _MissionPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("⚠️ Không thể ẩn _MissionPanel vì nó đã bị hủy!");
        }

        ResetGameForNewLevel();
    }

    private void ResetGameForNewLevel()
    {
        ActivePanel();
        spawner?.ResetLevel();
        _SetNumber();
        Time.timeScale = 1;
        StartCountdown();
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

            spawner = FindObjectOfType<Spawner>();
            scoreManager = FindObjectOfType<ScoreControl>();
            pod = FindObjectOfType<Pod>();

            if (_MissionPanel == null) Debug.LogWarning("⚠️ _MissionPanel không tìm thấy trong Scene!");
        }
    }

    public void _HomeButton() => SceneManager.LoadScene("MainMenu");
    public void _PlayAgainButton() => SceneManager.LoadScene("GamePlay");
    public void _PlayButton() { ActivePanel(); Time.timeScale = 1; StartCountdown(); }
    public void _ContinueButton() { ActivePanel(); Time.timeScale = 1; StartCountdown(); }
    public void _StopButton() { Time.timeScale = 0; _MenuGamePanel?.SetActive(true); StopCountdown(); }

    private void _SetTime(int time) { timeLeft = time; if (_Time != null) _Time.text = time.ToString(); }
    private void _SetNumber() { if (Number != null) Number.text = "0"; }
    public void _getNumber(int number) { if (Number != null) Number.text = number.ToString(); }

    void ActivePanel()
    {
        if (_MenuGamePanel != null) _MenuGamePanel.SetActive(false);
        if (_GameNotifPanel != null) _GameNotifPanel.SetActive(false);
        if (_MissionPanel != null) _MissionPanel.SetActive(false);
    }
}
