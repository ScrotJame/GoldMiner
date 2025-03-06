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

    private int Money;
    private GameControll gameController;
    private List<GameObject> spawnedItems = new List<GameObject>();

    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-6.3f, -1.14f);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(6.3f, -1.14f);
    [SerializeField] private int minObjects = 1, maxObjects = 4;
    [SerializeField] private float minSpawnDistance = 50f; // Minimum distance between items

    private void Start()
    {
        gameController = GameControll.instance;
        if (gameController == null)
        {
            Debug.LogError("GameControll.instance not found in the scene!");
            return;
        }

        if (gameController.scoreManager != null)
        {
            Money = gameController.scoreManager.GetCurrentScore();
            MoneyUI.text = Money.ToString();
        }
        else
        {
            Debug.LogError("ScoreControl not assigned in GameControll!");
            return;
        }

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
            Debug.Log("StoreManager instance initialized.");
        }
        else if (instance != this)
        {
            Debug.Log("Duplicate StoreManager found, destroying: " + gameObject.name);
            Destroy(gameObject);
            return;
        }

        if (purchasedItems == null)
        {
            purchasedItems = new List<ItemData>();
        }
        LoadPlayerData(); // Tải danh sách vật phẩm đã mua từ PlayerPrefs (nếu có)
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
        Debug.Log("Saved purchased items: " + purchasedItems.Count);
    }

    // Tải dữ liệu từ PlayerPrefs
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
        Debug.Log("Loaded purchased items: " + itemCount);
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
            if (ItemPrefab[prefabIndex] == null)
            {
                Debug.LogError($"ItemPrefab[{prefabIndex}] is null!");
                continue;
            }

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

                Text[] texts = spawnedItem.GetComponentsInChildren<Text>();
                if (texts.Length >= 3)
                {
                    texts[0].text = ShopItems[randomIndex].title;
                    texts[1].text = ShopItems[randomIndex].description;
                    texts[2].text = ShopItems[randomIndex].price.ToString();
                }

                Button button = spawnedItem.GetComponent<Button>();
                if (button != null)
                {
                    int index = randomIndex;
                    button.onClick.AddListener(() => BuyItem(index));
                }

                spawnedItems.Add(spawnedItem);
            }
            else
            {
                Debug.LogError("ItemPrefab must have a RectTransform component!");
                Destroy(spawnedItem);
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

    public void CheckPurchasable()
    {
        foreach (GameObject item in spawnedItems)
        {
            if (item != null)
            {
                Button button = item.GetComponent<Button>();
                if (button != null)
                {
                    Text priceText = item.GetComponentsInChildren<Text>()[1];
                    int price = int.Parse(priceText.text);
                    button.interactable = (Money >= price);
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

        if (Money >= ShopItems[index].price)
        {
            Money -= ShopItems[index].price;
            MoneyUI.text = Money.ToString();
            gameController.scoreManager.SpendScore(ShopItems[index].price);

            ItemData purchasedItem = new ItemData
            {
                id = index,
                nameItem = ShopItems[index].title,
                price = ShopItems[index].price,
                currentStack = 1,
                maxStack = 99,
                isStackable = true
            };

            ///gameController.AddPurchasedItem(purchasedItem);
            Debug.Log("Bought item: " + ShopItems[index].title);

            GameObject itemToRemove = spawnedItems.Find(item => item != null && item.GetComponentsInChildren<Text>()[0].text == ShopItems[index].title);
            if (itemToRemove != null)
            {
                spawnedItems.Remove(itemToRemove);
                Destroy(itemToRemove);
            }

            CheckPurchasable();
        }
        else
        {
            Debug.Log("Not enough money to buy item: " + ShopItems[index].title);
        }
    }

    public void NextMissionButton()
    {
        SceneManager.LoadScene("GamePlay");
    }
}
