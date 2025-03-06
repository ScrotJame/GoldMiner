using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControll : MonoBehaviour
{
    public static GameControll instance;

    public Text _Time, Number, _Notif, BestScoreTxt;
    private int initialTime;
    private int timeLeft;
    private Coroutine countdownCoroutine;
    [SerializeField] GameObject _MenuGamePanel, _GameNotifPanel;

    public bool isAlive = true;
    public int count = 0;
    public Spawner spawner;
    public ScoreControl scoreManager;
    private float scrollSpeedBoost;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Debug.Log("GameControll instance initialized.");
        }
        else if (instance != this)
        {
            Debug.Log("Duplicate GameControll found, destroying: " + gameObject.name);
            Destroy(gameObject);
            return;
        }

        if (scoreManager == null)
        {
            scoreManager = FindObjectOfType<ScoreControl>();
            if (scoreManager == null)
                Debug.LogError("ScoreControl not found in the scene!");
        }

        if (spawner == null)
        {
            spawner = FindObjectOfType<Spawner>();
            if (spawner == null)
                Debug.LogError("Spawner not found in the scene!");
        }

        scoreManager?.LoadPlayerData();
        InitializeLevel();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        if (_Time == null || Number == null || _Notif == null)
        {
            Debug.LogError("One or more UI references (_Time, Number, _Notif) are null in Start!");
            return;
        }
        StartCountdown();
        if (_MenuGamePanel != null) _MenuGamePanel.SetActive(false);
        if (_GameNotifPanel != null) _GameNotifPanel.SetActive(false);
    }

    public bool SpendMoney(int amount)
    {
        return scoreManager != null && scoreManager.SpendScore(amount);
    }

    public void AddScore(int score)
    {
        if (scoreManager != null) scoreManager.AddScore(score);
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

    //public void UseItem(string itemName)
    //{
    //    ItemData item = StoreManager.instance.purchasedItems.Find(i => i.nameItem == itemName && !i.isEquipped);
    //    if (item != null)
    //    {
    //        switch (item.nameItem)
    //        {
    //            case "Speed Boost":
    //                scrollSpeedBoost += 1f;
    //                item.isEquipped = true;
    //                Debug.Log("Used Speed Boost: " + scrollSpeedBoost);
    //                break;
    //            case "Extra Time":
    //                timeLeft += 10;
    //                item.isEquipped = true;
    //                _SetTime(timeLeft);
    //                Debug.Log("Used Extra Time: " + timeLeft);
    //                break;
    //            default:
    //                Debug.Log("No effect defined for: " + itemName);
    //                break;
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("Item not found or already used: " + itemName);
    //    }
    //}

    IEnumerator Countdown()
    {
        while (timeLeft > 0)
        {
            if (_Time != null)
                _Time.text = timeLeft.ToString("F0");
            else
            {
                Debug.LogError("_Time is null in Countdown!");
                yield break;
            }
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }
        if (_Time != null) _Time.text = "0";
        if (scoreManager != null)
            CheckMissionComplete(scoreManager.GetCurrentScore(), timeLeft);
    }

    private void InitializeLevel()
    {
        initialTime = 60; 
        timeLeft = initialTime;
        _SetTime(timeLeft);
        if (scoreManager != null)
            scoreManager.InitializeScore(0, 1000);
        _SetNumber();
    }

    private void _SetTime(int time)
    {
        timeLeft = time;
        if (_Time != null)
            _Time.text = time.ToString();
    }

    private void _SetNumber()
    {
        if (Number != null) Number.text = "0";
    }

    public void _getNumber(int number)
    {
        if (Number != null) Number.text = number.ToString();
    }

    private void CheckMissionComplete(int score, int time)
    {
        if (time <= 0 && scoreManager != null && score >= scoreManager.GetTargetScore())
        {
            if (_Notif != null) _Notif.text = "Mission Complete!";
            StopCountdown();
            Time.timeScale = 0;
            AdvanceToNextLevel(score);
            ResetGameForNewLevel(); // Reset trong cùng scene
        }
        else if (time <= 0 && scoreManager != null && score < scoreManager.GetTargetScore())
        {
            if (_Notif != null) _Notif.text = "Mission Failed! \n Do you want to try again?";
            if (scoreManager != null) scoreManager.InitializeScore(0);
            Time.timeScale = 0;
            if (_GameNotifPanel != null) _GameNotifPanel.SetActive(true);
        }
    }

    private void ResetGameForNewLevel()
    {
        if (_MenuGamePanel != null) _MenuGamePanel.SetActive(false);
        if (_GameNotifPanel != null) _GameNotifPanel.SetActive(false);
        if (spawner != null)
            spawner.ResetLevel();
        else
            Debug.LogError("Spawner is null in ResetGameForNewLevel!");
        _SetNumber();
        Time.timeScale = 1;
        StartCountdown();
    }

    private void AdvanceToNextLevel(int score)
    {
        if (scoreManager == null)
        {
            Debug.LogError("scoreManager is null in AdvanceToNextLevel!");
            return;
        }
        if (spawner == null)
        {
            spawner = FindObjectOfType<Spawner>();
            if (spawner == null)
            {
                Debug.LogError("Spawner not found in the scene!");
                return;
            }
        }

        initialTime += 20; 
        timeLeft = initialTime;
        int newTargetScore = (scoreManager.GetTargetScore() + score / 2) +500;
        scoreManager.SetTargetScore(newTargetScore);
        _SetTime(timeLeft);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //if (scene.name == "Store" && StoreManager.instance != null)
        //{
        //    GameObject storeObj = GameObject.Find("Store");
        //    if (storeObj != null)
        //        StoreManager.instance.AssignStore(storeObj.GetComponent<Store>());
        //}
        //else 
        if (scene.name == "GamePlay")
        {
            // Cập nhật lại tham chiếu nếu cần khi vào GamePlay
            spawner = FindObjectOfType<Spawner>();
            scoreManager = FindObjectOfType<ScoreControl>();
            if (spawner == null) Debug.LogError("Spawner not found after scene load!");
            if (scoreManager == null) Debug.LogError("ScoreManager not found after scene load!");
        }
    }

    public void _HomeButton() => SceneManager.LoadScene("MainMenu");
    public void _PlayAgainButton() => SceneManager.LoadScene("GamePlay");
    public void _PlayButton()
    {
        if (_MenuGamePanel != null) _MenuGamePanel.SetActive(false);
        if (_GameNotifPanel != null) _GameNotifPanel.SetActive(false);
        Time.timeScale = 1;
        StartCountdown();
    }
    public void _ContinueButton()
    {
        if (_MenuGamePanel != null) _MenuGamePanel.SetActive(false);
        if (_GameNotifPanel != null) _GameNotifPanel.SetActive(false);
        Time.timeScale = 1;
        StartCountdown();
    }
    public void _StopButton()
    {
        Time.timeScale = 0;
        if (_MenuGamePanel != null) _MenuGamePanel.SetActive(true);
        if (_GameNotifPanel != null) _GameNotifPanel.SetActive(false);
        StopCountdown();
    }
}