using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameControll : MonoBehaviour
{
    public static GameControll instance;

    public Text _Time, Number, _Notif, BestScoreTxt;
    private int timeLeft;
    private Coroutine countdownCoroutine;
    [SerializeField] GameObject _MenuGamePanel, _GameNotifPanel;
    public List<ItemData> purchasedItems = new List<ItemData>();

    public bool isAlive = true;
    public int count = 0;
    public Spawner spawner;
    public Store gameStore;
    public ScoreControl scoreManager;
    public StoreManager storeManager;
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
        if (purchasedItems == null)
        {
            purchasedItems = new List<ItemData>();
            Debug.Log("purchasedItems was null, initialized in Awake.");
        }

        if (scoreManager == null)
        {
            scoreManager = FindObjectOfType<ScoreControl>();
            if (scoreManager == null)
            {
                Debug.LogError("ScoreControl not found in the scene!");
            }
        }

        scoreManager.LoadPlayerData();
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
        StartCountdown();
        if (_MenuGamePanel != null) _MenuGamePanel.SetActive(false);
        if (_GameNotifPanel != null) _GameNotifPanel.SetActive(false);
    }

    public bool SpendMoney(int amount)
    {
        return scoreManager.SpendScore(amount);
    }

    public void AddScore(int score)
    {
        scoreManager.AddScore(score);
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
    public void UseItem(string itemName)
    {
        ItemData item = purchasedItems.Find(i => i.nameItem == itemName && !i.isEquipped);
        if (item != null)
        {
            switch (item.nameItem)
            {
                case "Speed Boost":
                    scrollSpeedBoost += 1f;
                    item.isEquipped = true;
                    Debug.Log("Used Speed Boost: " + scrollSpeedBoost);
                    break;
                case "Extra Time":
                    timeLeft += 10;
                    item.isEquipped = true;
                    _SetTime(timeLeft);
                    Debug.Log("Used Extra Time: " + timeLeft);
                    break;
                default:
                    Debug.Log("No effect defined for: " + itemName);
                    break;
            }
        }
        else
        {
            Debug.Log("Item not found or already used: " + itemName);
        }
    }
    IEnumerator Countdown()
    {
        while (timeLeft > 0)
        {
            _Time.text = timeLeft.ToString("F0");
            yield return new WaitForSeconds(1f);
            timeLeft--;
        }
        _Time.text = "0";
        CheckMissionComplete(scoreManager.GetCurrentScore(), timeLeft);
    }

    private void InitializeLevel()
    {
        timeLeft = 10;
        _SetTime(timeLeft);
        scoreManager.InitializeScore(0, 1); // Khởi tạo điểm số và mục tiêu
        _SetNumber();
    }

    private void _SetTime(int time)
    {
        timeLeft = time;
        if (_Time != null)
        {
            _Time.text = time.ToString();
        }
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
        if (time <= 0 && score >= scoreManager.GetTargetScore())
        {
            _Notif.text = "Mission Complete!";
            StopCountdown();
            Time.timeScale = 0;
            AdvanceToNextLevel(score);
            //SceneManager.LoadScene("Store");
            SceneManager.LoadScene("GamePlay");
        }
        else if (time <= 0 && score < scoreManager.GetTargetScore())
        {
            _Notif.text = "Mission Failed! \n Do you want to try again?";
            scoreManager.InitializeScore(0); // Reset điểm số
            Time.timeScale = 0;
            if (_GameNotifPanel != null) _GameNotifPanel.SetActive(true);
        }
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

        timeLeft = 60;
        int newTargetScore = scoreManager.GetTargetScore() + score / 2;
        timeLeft += 30;
        scoreManager.SetTargetScore(newTargetScore);
        _SetTime(timeLeft);

        spawner.ResetLevel();
    }

    // Load player data from PlayerPrefs
    private void LoadPlayerData()
    {
        int itemCount = PlayerPrefs.GetInt("PurchasedItemCount", 0);
        purchasedItems.Clear();
        for (int i = 0; i < itemCount; i++)
        {
            ItemData item = new ItemData
            {
                id = PlayerPrefs.GetInt($"Item_{i}_id"),
                nameItem = PlayerPrefs.GetString($"Item_{i}_name", ""),
                price = PlayerPrefs.GetInt($"Item_{i}_price", 0),
                currentStack = PlayerPrefs.GetInt($"Item_{i}_stack", 1),
                maxStack = PlayerPrefs.GetInt($"Item_{i}_maxStack", 99),
                isStackable = PlayerPrefs.GetInt($"Item_{i}_stackable", 1) == 1
            };
            purchasedItems.Add(item);
        }
        Debug.Log("Loaded: Score = " + scoreManager.GetCurrentScore() + ", Items = " + itemCount);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SavePlayerData();
        if (scene.name == "Store")
        {
            GameObject storeObj = GameObject.Find("Store");
            if (storeObj != null)
            {
                gameStore = storeObj.GetComponent<Store>();
                if (gameStore != null)
                {
                    Debug.Log("GameControll - Assigned gameStore from StoreScene");
                }
                else
                {
                    Debug.LogWarning("Store script not found on 'Store' GameObject in StoreScene");
                }
            }
            else
            {
                Debug.LogWarning("Could not find 'Store' GameObject in StoreScene");
            }
        }
    }
    public void AddPurchasedItem(ItemData item)
    {
        purchasedItems.Add(item);
        Debug.Log("Item added to GameControll: " + item.nameItem + ", Price: " + item.price);
        SavePlayerData();
    }


    private void SavePlayerData()
    {
        PlayerPrefs.SetInt("PurchasedItemCount", purchasedItems.Count);
        for (int i = 0; i < purchasedItems.Count; i++)
        {
            PlayerPrefs.SetInt($"Item_{i}_id", purchasedItems[i].id);
            PlayerPrefs.SetString($"Item_{i}_name", purchasedItems[i].nameItem);
            PlayerPrefs.SetInt($"Item_{i}_price", purchasedItems[i].price);
            PlayerPrefs.SetInt($"Item_{i}_stack", purchasedItems[i].currentStack);
            PlayerPrefs.SetInt($"Item_{i}_maxStack", purchasedItems[i].maxStack);
            PlayerPrefs.SetInt($"Item_{i}_stackable", purchasedItems[i].isStackable ? 1 : 0);
        }
        PlayerPrefs.Save();
        Debug.Log("Saved: Score = " + scoreManager.GetCurrentScore() + ", Items = " + purchasedItems.Count);
    }
    public void _HomeButton()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void _PlayAgainButton()
    {
        SceneManager.LoadScene("GamePlay");
    }

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
