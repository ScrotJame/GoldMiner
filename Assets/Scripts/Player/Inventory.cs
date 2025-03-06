using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory : MonoBehaviour
{
    public int space = 3;
    public List<ItemData> items = new List<ItemData>();
    [SerializeField] private GameObject inventoryUIPanel;
    [SerializeField] private Transform inventoryContainer;
    [SerializeField] private GameObject inventoryItemPrefab;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        LoadInventory();
    }

    public void Add(ItemData item)
    {
        if (items.Count >= space)
        {
            Debug.Log("Not enough room.");
            return;
        }

        if (item.isStackable)
        {
            ItemData existingItem = items.Find(i => i.id == item.id);
            if (existingItem != null)
            {
                if (existingItem.currentStack + item.currentStack <= existingItem.maxStack)
                {
                    existingItem.currentStack += item.currentStack;
                }
                else
                {
                    int remainingStack = existingItem.maxStack - existingItem.currentStack;
                    existingItem.currentStack = existingItem.maxStack;
                    item.currentStack -= remainingStack;
                    items.Add(item);
                }
                UpdateInventoryUI();
                SaveInventory();
                return;
            }
        }

        items.Add(item);
        UpdateInventoryUI();
        SaveInventory();
    }

    public bool Use(ItemData item)
    {
        bool success = false;
        if (items.Contains(item) )
        {
            success = true;
            item.currentStack--;
            if (item.currentStack <= 0)
            {
                items.Remove(item);
            }
            UpdateInventoryUI();
            SaveInventory();
        }
        return success;
    }

    public void DisplayInventory()
    {
        if (inventoryUIPanel != null)
        {
            inventoryUIPanel.SetActive(true);
            UpdateInventoryUI();
        }
    }

    private void UpdateInventoryUI()
    {
        if (inventoryContainer == null || inventoryItemPrefab == null) return;

        foreach (Transform child in inventoryContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (ItemData item in items)
        {
            GameObject itemObj = Instantiate(inventoryItemPrefab, inventoryContainer);
            Text itemText = itemObj.GetComponentInChildren<Text>();
            if (itemText != null)
            {
                itemText.text = $"{item.nameItem} x{item.currentStack}";
            }
            Button useButton = itemObj.GetComponentInChildren<Button>();
            if (useButton != null)
            {
                useButton.onClick.AddListener(() => Use(item));
            }
        }
    }

    public void SaveInventory()
    {
        
        PlayerPrefs.SetInt("InventoryCount", items.Count);

        for (int i = 0; i < items.Count; i++)
        {
            PlayerPrefs.SetInt($"Item_{i}_id", items[i].id);
            PlayerPrefs.SetString($"Item_{i}_name", items[i].nameItem);
            PlayerPrefs.SetInt($"Item_{i}_price", items[i].price);
            PlayerPrefs.SetInt($"Item_{i}_stack", items[i].currentStack);
            PlayerPrefs.SetInt($"Item_{i}_maxStack", items[i].maxStack);
            PlayerPrefs.SetInt($"Item_{i}_stackable", items[i].isStackable ? 1 : 0);
        }

        PlayerPrefs.Save();
        Debug.Log("Inventory saved with " + items.Count + " items");
    }

    // Tải Inventory từ PlayerPrefs
    private void LoadInventory()
    {
        int itemCount = PlayerPrefs.GetInt("InventoryCount", 0);
        items.Clear();

        for (int i = 0; i < itemCount; i++)
        {
            ItemData item = new ItemData
            {
                id = PlayerPrefs.GetInt($"Item_{i}_id"),
                nameItem = PlayerPrefs.GetString($"Item_{i}_name", ""),
                price = PlayerPrefs.GetInt($"Item_{i}_price", 0),
                currentStack = PlayerPrefs.GetInt($"Item_{i}_stack", 1),
                maxStack = PlayerPrefs.GetInt($"Item_{i}_maxStack", 99),
                isStackable = PlayerPrefs.GetInt($"Item_{i}_stackable", 1) == 1,
            };
            items.Add(item);
        }

        UpdateInventoryUI();
        Debug.Log("Inventory loaded with " + itemCount + " items");
    }
}