using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    //UI gameplay
    public Text timeText;
    public Text numberText;
    public Text notificationText;
    public Text scoreText;
    public Text targetScoreText;
    public Text highScoreText;

    //UI Notification
    public GameObject gameNotificationPanel;
    public GameObject menuGamePanel;
    public GameObject missionPanel;
    public GameObject storePanel;
    public Text missionTargetText;
    private Coroutine missionPanelCoroutine;

    //UI store
    [SerializeField] private GameObject shopUIPrefab;
    private GameObject shopUIInstance;
    public Text goldText;
    public Button dynamiteButton;
    public Button luckButton;
    public Button drugButton;
    public Button strengthButton;
    public Button continueButton;
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
        }
    }
    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GamePlay")
        {
            gameObject.SetActive(true);
            timeText = GameObject.Find("Time")?.GetComponent<Text>();
            numberText = GameObject.Find("SL")?.GetComponent<Text>();
            scoreText = GameObject.Find("Score")?.GetComponent<Text>();
            targetScoreText = GameObject.Find("target")?.GetComponent<Text>();
            notificationText = GameObject.Find("NotiText")?.GetComponent<Text>();
            gameNotificationPanel = GameObject.Find("Notifice");
            menuGamePanel = GameObject.Find("MenuPause");
            missionPanel = GameObject.Find("NotifMission");
            missionTargetText = GameObject.Find("MissiontargetText").GetComponentInChildren<Text>();
            if (missionPanel != null)
            {
                missionPanel.SetActive(true);
                StartCoroutine(HideMissionPanelAfterDelay(3f));
            }

            goldText = GameObject.Find("ScoreStore")?.GetComponent<Text>();
            dynamiteButton = GameObject.Find("Dynamite")?.GetComponent<Button>();
            drugButton = GameObject.Find("Drug")?.GetComponent<Button>();
            luckButton = GameObject.Find("Luck")?.GetComponent<Button>();
            strengthButton = GameObject.Find("Strength")?.GetComponent<Button>();
            continueButton = GameObject.Find("ContinueButton")?.GetComponent<Button>();
        }
    }
    public void UpdateTime(int time)
    {
        if (timeText != null) timeText.text = time.ToString();
    }
    public void _ShowShop()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("GameManager.instance is null!");
            return;
        }

        if (shopUIInstance == null && shopUIPrefab != null)
        {
            shopUIInstance = Instantiate(shopUIPrefab, transform); 
            storePanel = shopUIInstance.transform.Find("ShopPanel")?.gameObject;

            if (storePanel != null)
            {
                goldText = storePanel.transform.Find("ScoreStore")?.GetComponent<Text>();
                dynamiteButton = storePanel.transform.Find("Dynamite")?.GetComponent<Button>();
                drugButton = storePanel.transform.Find("Drug")?.GetComponent<Button>();
                luckButton = storePanel.transform.Find("Luck")?.GetComponent<Button>();
                strengthButton = storePanel.transform.Find("Strength")?.GetComponent<Button>();
                continueButton = storePanel.transform.Find("ContinueButton")?.GetComponent<Button>();

                if (dynamiteButton != null) dynamiteButton.onClick.AddListener(BuyDynamite);
                if (drugButton != null) drugButton.onClick.AddListener(BuyDrug);
                if (luckButton != null) luckButton.onClick.AddListener(BuyLuck);
                if (strengthButton != null) strengthButton.onClick.AddListener(BuyStrength);
                if (continueButton != null) continueButton.onClick.AddListener(CloseShop);
            }
            else
            {
                Debug.LogError("ShopPanel not found in ShopUI prefab!");
            }
        }

        if (storePanel != null)
        {
            storePanel.SetActive(true);
            GameManager.instance.PauseGameForShop();  
        }
        else
        {
            Debug.LogError("storePanel is null!");
        }

    }

    private void BuyStrength()
    {
        throw new NotImplementedException();
    }

    private void BuyLuck()
    {
        throw new NotImplementedException();
    }

    private void BuyDrug()
    {
        throw new NotImplementedException();
    }

    private void BuyDynamite()
    {
        throw new NotImplementedException();
    }

    private void CloseShop()
    {
        if (storePanel != null)
        {
            storePanel.SetActive(false);
            GameManager.instance.ResetGameForNewLevel();
        }
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
            gameNotificationPanel?.SetActive(true);
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
        missionPanel?.SetActive(false);
    }

    public void HidePanels()
    {
        menuGamePanel?.SetActive(false);
        gameNotificationPanel?.SetActive(false);
        missionPanel?.SetActive(false);
    }
    public void UpdateScoreUI(int score)
    {
        if (scoreText != null)
            scoreText.text = "" + score;
    }

    public void UpdateTargetScoreUI(int targetScore)
    {
        if (targetScoreText != null)
            targetScoreText.text = " " + targetScore;
    }

    public void UpdateHighScoreUI(int highScore)
    {
        if (highScoreText != null)
            highScoreText.text = "High Score: " + highScore;
    }
}