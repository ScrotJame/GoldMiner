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

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        if (missionPanelCoroutine != null)
        {
            StopCoroutine(missionPanelCoroutine);
            missionPanelCoroutine = null;
        }
    }
    void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
        if (missionPanelCoroutine != null)
        {
            StopCoroutine(missionPanelCoroutine);
            missionPanelCoroutine = null;
        }
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GamePlay")
        {
            gameObject.SetActive(true);
            timeText = GameObject.Find("Time")?.GetComponent<Text>();
            dynamiteText = GameObject.Find("CountDynamite")?.GetComponent<Text>();
            if (dynamiteText != null && string.IsNullOrEmpty(dynamiteText.text))
            {
                dynamiteText.gameObject.SetActive(!string.IsNullOrEmpty(dynamiteText.text) && int.Parse(dynamiteText.text) > 0);
            }
            else
            {
                dynamiteText.gameObject.SetActive(false);
            }
            scoreText = GameObject.Find("Score")?.GetComponent<Text>();
            targetScoreText = GameObject.Find("target")?.GetComponent<Text>();
            notificationText = GameObject.Find("NotiText")?.GetComponent<Text>();
            highScoreText = GameObject.Find("SL")?.GetComponent<Text>();
            gameNotificationPanel = GameObject.Find("Notifice");
            menuGamePanel = GameObject.Find("MenuPause");
            missionPanel = GameObject.Find("NotifMission");
            missionTargetText = GameObject.Find("MissiontargetText")?.GetComponentInChildren<Text>();
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
            gameNotificationPanel?.SetActive(true);
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
        missionPanel?.SetActive(false);
        missionPanelCoroutine = null;
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
            highScoreText.text = "" + highScore;
    }
}
