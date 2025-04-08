﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance;

    [SerializeField] private GameObject[] itemsUIPrefab;
    [SerializeField] private Transform itemsContainer; // Có thể gán trong Inspector

    private List<ItemData> ownedItems = new List<ItemData>();
    private Dictionary<string, GameObject> itemButtons = new Dictionary<string, GameObject>();
    private bool isCanvasFromInspector = false;

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
            return;
        }

        // Kiểm tra xem Canvas có được gán từ Inspector không
        isCanvasFromInspector = itemsContainer != null;
        if (isCanvasFromInspector)
        {
            DontDestroyOnLoad(itemsContainer.root.gameObject); // Bảo vệ Canvas nếu từ Inspector
        }
    }

    private void OnEnable() => SceneManager.sceneLoaded += OnSceneLoaded;
    private void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "GamePlay")
        {
            UpdateItemsContainer(); // Cập nhật Canvas khi scene tải lại
            RestoreItemsUI(); // Khôi phục UI
        }
    }

    void Start()
    {
        UpdateItemsContainer();
        RestoreItemsUI(); // Khôi phục UI khi khởi động
    }

    public void UpdateItemsContainer()
    {
        if (itemsContainer == null || itemsContainer.Equals(null)) // Nếu Canvas bị mất hoặc chưa gán
        {
            Canvas canvas = FindObjectOfType<Canvas>();
            if (canvas != null)
            {
                itemsContainer = canvas.transform;

                // Cấu hình Canvas nếu cần (không bắt buộc nếu Canvas đã được thiết lập trong scene)
                Canvas c = itemsContainer.GetComponent<Canvas>();
                if (c.renderMode != RenderMode.ScreenSpaceCamera)
                {
                    c.renderMode = RenderMode.ScreenSpaceCamera;
                    c.worldCamera = Camera.main;
                    if (c.worldCamera == null)
                    {
                        Debug.LogWarning("Không tìm thấy Main Camera để gán cho Canvas!");
                    }
                }
            }
            else
            {
                Debug.LogError("Không tìm thấy Canvas trong scene! Vui lòng thêm Canvas hoặc gán trong Inspector.");
            }
        }
    }

    public void AddItem(string itemName, Sprite itemIcon = null, System.Action onUse = null)
    {
        UpdateItemsContainer();

        if (itemsContainer == null)
        {
            Debug.LogError("Không tìm thấy Canvas trong scene!");
            return;
        }

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

            if (itemName == "Dynamite" && UIManager.instance != null)
            {
                int dynamiteCount = item.quantity;
                UIManager.instance.UpdateDynamiteCount(dynamiteCount);
            }
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
                item.onUse.Invoke();
            }

            if (item.itemName == "Dynamite")
            {
                useSuccess = Pod.instance.UseDynamite();
            }
            else if (item.itemName == "Streng")
            {
                useSuccess = true;
                Pod.instance.UseStreng();
            }
            else
            {
                useSuccess = true;
            }

            if (useSuccess)
            {
                item.quantity--;

                if (item.itemName == "Dynamite" && UIManager.instance != null)
                {
                    UIManager.instance.UpdateDynamiteCount(item.quantity);
                }

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

    public int GetItemCount(string itemName)
    {
        ItemData item = ownedItems.Find(i => i.itemName == itemName);
        return item != null ? item.quantity : 0;
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    private void RestoreItemsUI()
    {
        foreach (var item in ownedItems)
        {
            ShowItemButton(item);
        }
    }

    public void OnPlayAgain()
    {
        foreach (var button in itemButtons.Values)
        {
            Destroy(button);
        }
        itemButtons.Clear();
        if (!isCanvasFromInspector)
        {
            itemsContainer = null;
        }
        UpdateItemsContainer();
        RestoreItemsUI();
    }
}