using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public static StoreManager instance;
    public Text MoneyUI;
    public Store[] StoreItemUI;
    public ShopItem[] ShopItems;
    public GameObject StorePanel;
    public GameObject[] ItemPrefab;
    public List<ItemData> purchasedItems = new List<ItemData>();

    private GameManager gameManager;
    private List<GameObject> spawnedItems = new List<GameObject>();

    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-6.3f, -1.14f);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(6.3f, -1.14f);
    [SerializeField] private int minObjects = 1, maxObjects = 4;
    [SerializeField] private float minSpawnDistance = 50f;

    private void Start()
    {

        gameManager = GameManager.instance;
        if (gameManager == null)
        {
            Debug.LogError("GameManager instance not found!");
            return;
        }

        UpdateMoneyUI();

        if (StorePanel == null)
        {
            Debug.LogError("StorePanel not assigned in StoreManager!");
            return;
        }

        if (ItemPrefab == null)
        {
            Debug.LogError("ItemPrefab not assigned in StoreManager!");
            return;
        }

        SpawnItemsInPanel();
        LoadStoreUI();
        CheckPurchasable();
    }

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

    private void SpawnItemsInPanel()
    {
        int totalToSpawn = Mathf.Min(maxObjects, ShopItems.Length);
        HashSet<int> spawnedIndices = new HashSet<int>();

        for (int i = 0; i < totalToSpawn; i++)
        {
            int randomIndex;
            do
            {
                randomIndex = Random.Range(0, ShopItems.Length);
            } while (spawnedIndices.Contains(randomIndex));

            spawnedIndices.Add(randomIndex);
            int prefabIndex = Mathf.Min(randomIndex, ItemPrefab.Length - 1);

            GameObject spawnedItem = Instantiate(ItemPrefab[prefabIndex], StorePanel.transform);
            RectTransform rectTransform = spawnedItem.GetComponent<RectTransform>();

            if (rectTransform != null)
            {
                Vector2 spawnPoint;
                int attempts = 0;
                do
                {
                    spawnPoint = new Vector2(
                        Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                        Random.Range(spawnAreaMin.y, spawnAreaMax.y)
                    );
                    attempts++;
                } while (IsPositionOccupied(spawnPoint) && attempts < 100);

                if (attempts >= 100)
                {
                    Debug.LogWarning("Failed to find a valid spawn point after 100 attempts.");
                    Destroy(spawnedItem);
                    continue;
                }

                rectTransform.anchoredPosition = spawnPoint;
                spawnedItems.Add(spawnedItem);
            }
        }
    }

    private bool IsPositionOccupied(Vector2 position)
    {
        foreach (var item in spawnedItems)
        {
            RectTransform rectTransform = item.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                if (Vector2.Distance(rectTransform.anchoredPosition, position) < minSpawnDistance)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void LoadStoreUI()
    {
        if (StoreItemUI == null || ShopItems == null || StoreItemUI.Length != ShopItems.Length)
        {
            Debug.LogError("StoreItemUI or ShopItems is null or length mismatch!");
            return;
        }

        for (int i = 0; i < StoreItemUI.Length; i++)
        {
            if (StoreItemUI[i] != null && ShopItems[i] != null)
            {
                StoreItemUI[i].titletext.text = ShopItems[i].title;
                StoreItemUI[i].desciciptiontext.text = ShopItems[i].description;
                StoreItemUI[i].pricetext.text = ShopItems[i].price.ToString();
            }
        }
    }

    private void UpdateMoneyUI()
    {
        if (MoneyUI != null)
        {
            MoneyUI.text = gameManager.goldAmount.ToString();
        }
    }

    public void CheckPurchasable()
    {
        foreach (GameObject item in spawnedItems)
        {
            if (item != null)
            {
                Button button = item.GetComponent<Button>();
                if (button != null)
                {
                    Text[] texts = item.GetComponentsInChildren<Text>();
                    if (texts.Length < 2)
                    {
                        Debug.LogError("Không tìm thấy Text UI chứa giá!");
                        continue;
                    }

                    string priceTextStr = texts[1].text.Trim();
                    Debug.Log($"Kiểm tra giá trị text: '{priceTextStr}'"); 
                    if (string.IsNullOrEmpty(priceTextStr))
                    {
                        Debug.LogError("Chuỗi giá rỗng hoặc không hợp lệ!");
                        continue;
                    }

                    if (string.IsNullOrEmpty(priceTextStr))
                    {
                        Debug.LogError("Chuỗi giá rỗng hoặc không hợp lệ!");
                        continue;
                    }

                    priceTextStr = priceTextStr.Replace(",", "").Replace("$", "");

                    if (int.TryParse(priceTextStr, out int price))
                    {
                        button.interactable = (ScoreControl.instance.GetCurrentScore() >= price);
                    }
                    else
                    {
                        Debug.LogError($"⚠ Không thể chuyển đổi giá '{priceTextStr}' thành số nguyên!");
                    }
                }
            }
        }
    }



    public void BuyItem(int index)
    {
        if (index < 0 || index >= ShopItems.Length)
        {
            Debug.LogError("Invalid item index: " + index);
            return;
        }

        int price = ShopItems[index].price;

        if (ScoreControl.instance.SpendScore(price))
        {
            Debug.Log("Bought item: " + ShopItems[index].title);

            GameObject itemToRemove = spawnedItems.Find(item => item != null &&
                item.GetComponentsInChildren<Text>()[0].text == ShopItems[index].title);

            if (itemToRemove != null)
            {
                spawnedItems.Remove(itemToRemove);
                Destroy(itemToRemove);
            }

            CheckPurchasable();
        }
        else
        {
            Debug.Log("Not enough score to buy item: " + ShopItems[index].title);
        }
    }

    public void _StartGameButton()
    {
        SceneManager.LoadScene("GamePlay");
    }
}
