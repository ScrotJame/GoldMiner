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

    private int initialTime = 20;
    private int timeLeft;
    private Coroutine countdownCoroutine;
    private Coroutine missionPanelCoroutine; // Quản lý coroutine cho _MissionPanel
    [SerializeField]
    private GameObject _MenuGamePanel, _GameNotifPanel, _MissionPanel; // Gán qua Inspector
    [SerializeField]
    private GameObject missionPanelPrefab; // Prefab để khởi tạo nếu cần
    private Text _MissionTargetText;

    private ScoreControl scoreManager;
    private Spawner spawner;
    private Pod pod;
    private int _basetime;
    private int _nextScore;
    public LinePod linePod;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            // Chỉ giữ _MissionPanel nếu đã gán trong Inspector
            if (_MissionPanel != null)
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
        // Không gọi InitializeLevel ở đây, sẽ gọi trong OnSceneLoaded
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Dừng các coroutine khi scene thay đổi
        if (countdownCoroutine != null)
        {
            StopCoroutine(countdownCoroutine);
            countdownCoroutine = null;
        }
        if (missionPanelCoroutine != null)
        {
            StopCoroutine(missionPanelCoroutine);
            missionPanelCoroutine = null;
        }

        if (scene.name == "GamePlay")
        {
            // Tìm lại các tham chiếu trong scene
            _Time = GameObject.Find("Time")?.GetComponent<Text>();
            Number = GameObject.Find("SL")?.GetComponent<Text>();
            _Notif = GameObject.Find("NotiText")?.GetComponent<Text>();
            _MissionTargetText = GameObject.Find("MissiontargetText")?.GetComponent<Text>();

            // Khởi tạo _MissionPanel
            if (_MissionPanel == null)
            {
                if (missionPanelPrefab != null)
                {
                    _MissionPanel = Instantiate(missionPanelPrefab);
                    DontDestroyOnLoad(_MissionPanel); // Giữ instance mới
                }
                else
                {
                    _MissionPanel = GameObject.Find("NotifMission");
                    if (_MissionPanel != null && !ReferenceEquals(_MissionPanel, _MissionPanel))
                        DontDestroyOnLoad(_MissionPanel);
                }
            }

            _GameNotifPanel = GameObject.Find("Notifice");
            _MenuGamePanel = GameObject.Find("MenuPause");

            scoreManager = FindObjectOfType<ScoreControl>();
            spawner = FindObjectOfType<Spawner>();
            pod = FindObjectOfType<Pod>();

            if (scoreManager == null) Debug.LogError("ScoreControl not found in scene!");
            if (_MissionPanel == null) Debug.LogError("MissionPanel not found in scene!");

            // Kích hoạt _MissionPanel ngay khi scene "GamePlay" được tải
            if (_MissionPanel != null)
            {
                _MissionPanel.SetActive(true);
                if (_MissionTargetText != null)
                {
                    _MissionTargetText.text = scoreManager?.GetTargetScore().ToString() ?? "0";
                }
                missionPanelCoroutine = StartCoroutine(HideMissionPanelAfterDelay(3f)); // Ẩn sau 3 giây
            }
            else
            {
                Debug.LogError("MissionPanel could not be initialized in GamePlay scene!");
            }

            // Đảm bảo các panel khác không hiển thị
            if (_GameNotifPanel != null && _GameNotifPanel.activeSelf)
                _GameNotifPanel.SetActive(false);
            if (_MenuGamePanel != null && _MenuGamePanel.activeSelf)
                _MenuGamePanel.SetActive(false);

            // Khởi tạo level sau khi MissionPanel được hiển thị
            InitializeLevel();
        }
    }

    private void InitializeLevel()
    {
        timeLeft = initialTime;
        _SetTime(timeLeft);
        if (scoreManager != null && currentLevel == 1)
            scoreManager.InitializeScore(0, 10);
        _SetNumber();
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
        currentLevel++;
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
            missionPanelCoroutine = StartCoroutine(HideMissionPanelAndContinue(3f));
        }
        else
        {
            Debug.LogError("_MissionPanel is null!");
        }
    }

    private IEnumerator HideMissionPanelAndContinue(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (_MissionPanel != null)
        {
            _MissionPanel.SetActive(false);
        }
        else
        {
            Debug.LogWarning("MissionPanel is null or has been destroyed in HideMissionPanelAndContinue!");
        }
        ResetGameForNewLevel();
    }

    private void CheckMissionComplete(int score, int time)
    {
        if (time <= 0)
        {
            pod?.ResetHookPosition();

            if (score >= (scoreManager?.GetTargetScore() ?? 0))
            {
                _Notif.text = "Mission Complete!";
                SceneManager.LoadScene("Store");
                NextMission(score);
            }
            else
            {
                _Notif.text = "Mission Failed! \n Do you want to try again?";
                scoreManager?.InitializeScore(0);
                Time.timeScale = 0;
                _GameNotifPanel?.SetActive(true);
            }
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
        _SetTime(initialTime);
        if (scoreManager != null)
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
        if (_MenuGamePanel != null && _MenuGamePanel.activeSelf)
            _MenuGamePanel.SetActive(false);
        if (_GameNotifPanel != null && _GameNotifPanel.activeSelf)
            _GameNotifPanel.SetActive(false);
        if (_MissionPanel != null && _MissionPanel.activeSelf)
            _MissionPanel.SetActive(false);
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
            Debug.LogWarning("MissionPanel is null or has been destroyed in HideMissionPanelAfterDelay!");
        }
    }
}