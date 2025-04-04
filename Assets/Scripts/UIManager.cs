using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    // UI gameplay
    public Text timeText;
    public Text numberText;
    public Text notificationText;
    public Text scoreText;
    public Text targetScoreText;
    public Text highScoreText;
    public Text dynamiteText;

    // UI Notification
    public GameObject gameNotificationPanel;
    public GameObject menuGamePanel;
    public GameObject missionPanel;
    public GameObject storePanel;
    public Text missionTargetText;
    private Coroutine missionPanelCoroutine;

    // Score popup
    public GameObject scorePopupPrefab;
    public Transform popupParent;

    private Canvas uiCanvas;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Tạo Canvas mới
            SetupCanvas();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void SetupCanvas()
    {
        // Xóa Canvas cũ nếu có
        if (uiCanvas != null)
        {
            Destroy(uiCanvas);
        }

        // Tạo Canvas mới
        uiCanvas = gameObject.GetComponent<Canvas>();
        if (uiCanvas == null)
        {
            uiCanvas = gameObject.AddComponent<Canvas>();
        }
        uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
        uiCanvas.worldCamera = Camera.main; // Gán Main Camera
        if (uiCanvas.worldCamera == null)
        {
            Debug.LogWarning("Không tìm thấy Main Camera khi tạo Canvas!");
        }
    }

    void Start()
    {
        InitializeButtons();
    }

    void InitializeButtons()
    {
        if (gameNotificationPanel != null)
        {
            Button[] buttons = gameNotificationPanel.GetComponentsInChildren<Button>();
            foreach (Button btn in buttons)
            {
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() => HandleButtonClick(btn.name));
                btn.interactable = true;
            }
        }
    }

    void HandleButtonClick(string buttonName)
    {
        Debug.Log("Nút được nhấn: " + buttonName);
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GamePlay")
        {
            SetupCanvas();
            timeText = GameObject.Find("Time")?.GetComponent<Text>();
            dynamiteText = GameObject.Find("CountDynamite")?.GetComponent<Text>();
            scoreText = GameObject.Find("Score")?.GetComponent<Text>();
            targetScoreText = GameObject.Find("target")?.GetComponent<Text>();
            notificationText = GameObject.Find("NotiText")?.GetComponent<Text>();
            highScoreText = GameObject.Find("SL")?.GetComponent<Text>();
            gameNotificationPanel = GameObject.Find("Notifice");
            menuGamePanel = GameObject.Find("MenuPause");
            missionPanel = GameObject.Find("NotifMission");
            missionTargetText = GameObject.Find("MissiontargetText")?.GetComponent<Text>();
            UIManager[] uiManagers = FindObjectsOfType<UIManager>();
            if (uiManagers.Length > 1)
            {
                for (int i = 1; i < uiManagers.Length; i++)
                {
                    Destroy(uiManagers[i].gameObject);
                }
            }
            InitializeButtons();
            if (gameNotificationPanel != null)
            {
                gameNotificationPanel.SetActive(true);
                Canvas panelCanvas = gameNotificationPanel.GetComponent<Canvas>();
                if (panelCanvas != null)
                {
                    panelCanvas.sortingOrder = 10;
                }
            }

            if (missionPanel != null)
            {
                missionPanel.SetActive(true);
                StartCoroutine(HideMissionPanelAfterDelay(3f));
            }
        }
    }

    public void UpdateTime(int time)
    {
        if (timeText != null) timeText.text = time.ToString();
    }

    public void UpdateNumber(int number)
    {
        if (numberText != null) numberText.text = number.ToString();
    }

    public void ShowNotification(string message)
    {
        if (notificationText != null)
        {
            notificationText.text = message;
            if (gameNotificationPanel != null)
            {
                gameNotificationPanel.SetActive(true);
                InitializeButtons();
            }
        }
    }

    public void UpdateDynamiteCount(int count)
    {
        if (dynamiteText != null)
        {
            dynamiteText.text = count.ToString();
            dynamiteText.gameObject.SetActive(count > 0);
        }
    }

    public void ShowMissionPanel(int score)
    {
        if (missionPanel != null)
        {
            missionPanel.SetActive(true);
            if (missionTargetText != null)
                missionTargetText.text = $"{score}";

            if (missionPanelCoroutine != null)
                StopCoroutine(missionPanelCoroutine);

            missionPanelCoroutine = StartCoroutine(HideMissionPanelAfterDelay(3f));
        }
    }

    private IEnumerator HideMissionPanelAfterDelay(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        if (missionPanel != null) missionPanel.SetActive(false);
        missionPanelCoroutine = null;
    }

    public void HidePanels()
    {
        if (menuGamePanel != null) menuGamePanel.SetActive(false);
        if (gameNotificationPanel != null) gameNotificationPanel.SetActive(false);
        if (missionPanel != null) missionPanel.SetActive(false);
    }

    public void UpdateScoreUI(int score)
    {
        if (scoreText != null) scoreText.text = "" + score;
    }

    public void UpdateTargetScoreUI(int targetScore)
    {
        if (targetScoreText != null) targetScoreText.text = " " + targetScore;
    }

    public void UpdateHighScoreUI(int highScore)
    {
        if (highScoreText != null) highScoreText.text = "" + highScore;
    }
}