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

    public Button yesbutt;
    public Button nobutt;
    public Button pauseButton;
    public Button musicButton;
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

    public int lastAdLevel = -2;
    public GameObject adsRewardButtonPrefab;
    private GameObject adsRewardButtonInstance; 
    public int currentLevelIndex;


    private Canvas uiCanvas;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        lastAdLevel = PlayerPrefs.GetInt("LastAdLevel", -2);
    }

    private void SetupCanvas()
    {
        // Tìm Canvas chính trong scene
        uiCanvas = FindObjectOfType<Canvas>();
        if (uiCanvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            uiCanvas = canvasObj.AddComponent<Canvas>();
            uiCanvas.renderMode = RenderMode.ScreenSpaceCamera;
            uiCanvas.worldCamera = Camera.main;
            if (uiCanvas.worldCamera == null)
            {
            }
        }
    }

    void Start()
    {
        InitializeButtons();
        UpdateLevelIndex();
        if (currentLevelIndex == 1)
        {
            lastAdLevel = -2;
            PlayerPrefs.SetInt("LastAdLevel", lastAdLevel);
            PlayerPrefs.Save();
        }
        UpdateAdsRewardButton();
    }

    private void UpdateLevelIndex()
    {
        currentLevelIndex = GameManager.instance != null ? GameManager.instance.currentLevel : 1;
        }

    public void InitializeButtons()
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
        else
        {
            Debug.LogWarning("gameNotificationPanel là null!");
        }
        if (pauseButton != null)
        {
            pauseButton.onClick.RemoveAllListeners();
            pauseButton.onClick.AddListener(() => HandleButtonClick("PauseButton"));
        }
        if (musicButton != null)
        {
            musicButton.onClick.RemoveAllListeners();
            musicButton.onClick.AddListener(() => HandleButtonClick("MusicButton"));
        }
    }

    void HandleButtonClick(string buttonName)
    {
        Debug.Log($"Nút được nhấn: {buttonName}");
        if (buttonName == "YesButt")
        {
            GameManager.instance?.PlayAgainButton();
        }
        else if (buttonName == "NoButt")
        {
            GameManager.instance?.HomeButton();
        }
        else if (buttonName == "StopButt")
        {
            Time.timeScale = 0;
            if (menuGamePanel != null) menuGamePanel.SetActive(true);
        }
        else if (buttonName == "SoundToggleButton")
        {
            AudioListener.volume = AudioListener.volume > 0 ? 0 : 1;
        }
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
            if (dynamiteText != null)
            {
                dynamiteText.gameObject.SetActive(!string.IsNullOrEmpty(dynamiteText.text) && int.TryParse(dynamiteText.text, out int count) && count > 0);
            }
            else
            {
                dynamiteText?.gameObject.SetActive(false);
            }
            scoreText = GameObject.Find("Score")?.GetComponent<Text>();
            targetScoreText = GameObject.Find("target")?.GetComponent<Text>();
            notificationText = GameObject.Find("NotiText")?.GetComponent<Text>();
            highScoreText = GameObject.Find("SL")?.GetComponent<Text>();
            gameNotificationPanel = GameObject.Find("Notifice");
            menuGamePanel = GameObject.Find("MenuPause");
            missionPanel = GameObject.Find("NotifMission");
            missionTargetText = GameObject.Find("MissiontargetText")?.GetComponent<Text>();

            // Xóa các UIManager trùng lặp
            UIManager[] uiManagers = FindObjectsOfType<UIManager>();
            if (uiManagers.Length > 1)
            {
                for (int i = 1; i < uiManagers.Length; i++)
                {
                    Destroy(uiManagers[i].gameObject);
                }
            }

            UpdateLevelIndex();
            if (currentLevelIndex == 1)
            {
                lastAdLevel = -2;
                PlayerPrefs.SetInt("LastAdLevel", lastAdLevel);
                PlayerPrefs.Save();
                Debug.Log($"OnSceneLoaded - Reset lastAdLevel to -2 tại level 1");
            }
            Debug.Log($"OnSceneLoaded - Current Level: {currentLevelIndex}, Last Ad Level: {lastAdLevel}, GameManager Level: {(GameManager.instance != null ? GameManager.instance.currentLevel : -1)}");
            UpdateAdsRewardButton();

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
        else if (scene.name == "MainMenu")
        {
            ClearUIReferences();
            HidePanels();
            if (uiCanvas != null)
            {
                uiCanvas.enabled = false;
            }
            // Hủy adsRewardButton nếu tồn tại
            if (adsRewardButtonInstance != null)
            {
                Destroy(adsRewardButtonInstance);
                adsRewardButtonInstance = null;
                Debug.Log("Hủy adsRewardButtonInstance trong MainMenu");
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
                if (yesbutt != null) yesbutt.gameObject.SetActive(true);
                if (nobutt != null) nobutt.gameObject.SetActive(true);
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

    public void ClearUIReferences()
    {
        HidePanels();
        if (uiCanvas != null && !uiCanvas.Equals(null))
        {
            uiCanvas.enabled = false;
        }
        timeText = null;
        numberText = null;
        notificationText = null;
        scoreText = null;
        targetScoreText = null;
        highScoreText = null;
        dynamiteText = null;
        gameNotificationPanel = null;
        menuGamePanel = null;
        missionPanel = null;
        missionTargetText = null;
        popupParent = null;
    }

    public void HidePanels()
    {
        if (menuGamePanel != null) menuGamePanel.SetActive(false);
        if (gameNotificationPanel != null) gameNotificationPanel.SetActive(false);
        if (missionPanel != null) missionPanel.SetActive(false);
    }

    public void HideButton()
    {
        if (yesbutt != null) yesbutt.gameObject.SetActive(false);
        if (nobutt != null) nobutt.gameObject.SetActive(false);
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

    private bool CanUseAdsReward()
    {
        UpdateLevelIndex();
        int currentLevel = currentLevelIndex;
        bool canUse = lastAdLevel == -2 || currentLevel >= lastAdLevel + 2;
        return canUse;
    }

    public void UpdateAdsRewardButton()
    {
        UpdateLevelIndex();
        bool canUse = CanUseAdsReward();
       
        if (canUse)
        {
            if (adsRewardButtonInstance == null && adsRewardButtonPrefab != null)
            {
                if (uiCanvas == null)
                {
                    SetupCanvas();
                }
                if (uiCanvas == null)
                {
                    Debug.LogError("uiCanvas vẫn là null sau khi gọi SetupCanvas!");
                    return;
                }
                adsRewardButtonInstance = Instantiate(adsRewardButtonPrefab, uiCanvas.transform);
                adsRewardButtonInstance.name = "AdsRewardButtonInstance";
                Debug.Log($"Instantiate adsRewardButtonInstance tại level {currentLevelIndex}, Parent: {adsRewardButtonInstance.transform.parent.name}");

                Button buttonComponent = adsRewardButtonInstance.GetComponent<Button>();
                if (buttonComponent != null)
                {
                    buttonComponent.onClick.RemoveAllListeners();
                    buttonComponent.onClick.AddListener(OnClickAdsReward);
                    buttonComponent.interactable = true;
                    Debug.Log($"Gán OnClick cho adsRewardButtonInstance, interactable: {buttonComponent.interactable}");
                }
                else
                {
                    Debug.LogError("adsRewardButtonPrefab không có Button component!");
                }
            }
            else if (adsRewardButtonInstance != null)
            {
                adsRewardButtonInstance.SetActive(true);
                Debug.Log($"Kích hoạt adsRewardButtonInstance tại level {currentLevelIndex}, Parent: {adsRewardButtonInstance.transform.parent.name}");
            }
            else
            {
                Debug.LogError("adsRewardButtonPrefab là null! Vui lòng gán prefab trong Inspector.");
            }
        }
        else
        {
            if (adsRewardButtonInstance != null)
            {
                Destroy(adsRewardButtonInstance);
                adsRewardButtonInstance = null;
                Debug.Log($"Hủy adsRewardButtonInstance tại level {currentLevelIndex}");
            }
        }
    }

    public void OnClickAdsReward()
    {
        if (adsRewardButtonInstance == null)
        {
            Debug.LogError("adsRewardButtonInstance là null khi nhấn!");
            UpdateAdsRewardButton(); 
            return;
        }

        if (!CanUseAdsReward())
        {
            Debug.Log($"AdsRewardButton không thể sử dụng đến level {lastAdLevel + 2}");
            return;
        }

        UpdateLevelIndex();
        lastAdLevel = currentLevelIndex;
        PlayerPrefs.SetInt("LastAdLevel", lastAdLevel);
        PlayerPrefs.Save();
        if (adsRewardButtonInstance != null)
        {
            Destroy(adsRewardButtonInstance);
            adsRewardButtonInstance = null;
        }

        GoogleAdsManager adsManager = FindObjectOfType<GoogleAdsManager>();
        if (adsManager != null)
        {
            adsManager.ShowRewardedAd();
        }
        else
        {
            Debug.LogError("GoogleAdsManager không tìm thấy!");
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
        if (adsRewardButtonInstance != null)
        {
            Destroy(adsRewardButtonInstance);
            adsRewardButtonInstance = null;
        }
    }
}