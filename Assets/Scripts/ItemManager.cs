// File: ItemManager.cs
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [SerializeField] private GameObject[] itemsUIPrefab; 
    [SerializeField] private Transform itemsContainer;  

    private List<ItemData> ownedItems = new List<ItemData>(); 
    private Dictionary<string, GameObject> itemButtons = new Dictionary<string, GameObject>(); 

    private bool isHaveItems = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        if (itemsContainer == null)
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                itemsContainer = canvas.transform;
            }
            else
            {
                return;
            }
        }
    }

    public void AddItem(string itemName, Sprite itemIcon = null, System.Action onUse = null)
    {
        if (itemName == "Dynamite" || itemName == "Streng")
        {
            ItemData item = ownedItems.Find(i => i.itemName == itemName);
            if (item != null)
            {
                item.quantity++;
            }
            else
            {
                item = new ItemData(itemName, 1, itemIcon, onUse);
                ownedItems.Add(item);
            }
            ShowItemButton(item);
        }
        else
        {
            onUse?.Invoke();
        }
    }

    private void ShowItemButton(ItemData item)
    {
        GameObject prefab = null;
        foreach (var p in itemsUIPrefab)
        {
            if (p.name == item.itemName + "Use") 
            {
                prefab = p;
                break;
            }
        }

        if (prefab == null)
        {
            Debug.LogWarning($"Không tìm thấy prefab UI cho {item.itemName}Use!");
            return;
        }

        if (itemButtons.ContainsKey(item.itemName))
        {
            UpdateButtonUI(item);
        }
        else
        {
            GameObject buttonObj = Instantiate(prefab, itemsContainer);
            itemButtons[item.itemName] = buttonObj;

            Image iconImage = buttonObj.transform.Find("ItemIcon")?.GetComponent<Image>();
            Text quantityText = buttonObj.transform.Find("QuantityText")?.GetComponent<Text>();
            Button useButton = buttonObj.GetComponent<Button>();

            if (iconImage != null && item.itemIcon != null) iconImage.sprite = item.itemIcon;
            if (quantityText != null) quantityText.text = "x" + item.quantity;
            if (useButton != null)
            {
                useButton.onClick.RemoveAllListeners();
                useButton.onClick.AddListener(() => UseItem(item.itemName));
            }
            else
            {
                Debug.LogWarning($"Không tìm thấy Button component trên {item.itemName}Use!");
            }
        }
    }

    private void UpdateButtonUI(ItemData item)
    {
        GameObject buttonObj = itemButtons[item.itemName];
        Text quantityText = buttonObj.transform.Find("QuantityText")?.GetComponent<Text>();
        if (quantityText != null) quantityText.text = "x" + item.quantity;
    }
    public void ClearAllItems()
    {
        
        foreach (var button in itemButtons.Values)
        {
            Destroy(button);
        }
        itemButtons.Clear();
        ownedItems.Clear();
    }
    public void UseItem(string itemName)
    {
        ItemData item = ownedItems.Find(i => i.itemName == itemName);
        if (item != null && item.quantity > 0)
        {
            bool useSuccess = false;
            if (item.onUse != null)
            {
                var previousQuantity = item.quantity;
                item.onUse.Invoke();
                if (item.itemName == "Dynamite")
                {
                    useSuccess = Pod.instance.UseDynamite();
                }
                else if (item.itemName == "Streng")
                {
                    useSuccess = true;
                    Pod.instance.UseStreng();
                }
            }
            if (useSuccess)
            {
                item.quantity--;
                if (item.quantity <= 0)
                {
                    ownedItems.Remove(item);
                    if (itemButtons.ContainsKey(itemName))
                    {
                        Destroy(itemButtons[itemName]);
                        itemButtons.Remove(itemName);
                    }
                }
                else
                {
                    UpdateButtonUI(item);
                }
            }
        }
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}