using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private List<ItemData> playerItems;
    public GameObject inventoryPanel;
    public GameObject itemPrefab; 
    public Transform itemContainer;
    public Image playerItemSlot;
    private void Start()
    {
        if (StoreManager.instance != null)
        {
            playerItems = StoreManager.instance.purchasedItems;
            ShowInventory();
        }
        else
        {
            Debug.LogError("StoreManager không tồn tại!");
        }
    }

    private void ShowInventory()
    {
        foreach (Transform child in itemContainer)
        {
            Destroy(child.gameObject);
        }

        foreach (var item in playerItems)
        {
            GameObject newItem = Instantiate(itemPrefab, itemContainer);
            Text[] texts = newItem.GetComponentsInChildren<Text>();
            Image itemImage = newItem.GetComponentInChildren<Image>();
            Button useButton = newItem.GetComponentInChildren<Button>();

            if (texts.Length >= 2)
            {
                texts[0].text = item.nameItem;
                texts[1].text = $"Số lượng: {item.currentStack}"; 
            }
            itemImage.sprite = Resources.Load<Sprite>($"Sprites/{item.nameItem}");
            useButton.onClick.AddListener(() => UseItem(item, texts[1], newItem));
        }

        inventoryPanel.SetActive(true); 
    }

    private void UseItem(ItemData item, Text quantityText, GameObject itemUI)
    {
        if (item.currentStack > 0)
        {
            item.currentStack--;
            quantityText.text = $"Số lượng: {item.currentStack}";
            playerItemSlot.sprite = Resources.Load<Sprite>($"Sprites/{item.nameItem}");

            if (item.currentStack <= 0)
            {
                playerItems.Remove(item);
                Destroy(itemUI);
            }

            Debug.Log("Đã sử dụng vật phẩm: " + item.nameItem);
        }
    }
}
