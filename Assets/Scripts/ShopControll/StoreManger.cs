﻿using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoreManager : MonoBehaviour
{
    public static StoreManager Instance;
    public GameObject shopUIPrefab;
    private GameObject shopInstance;
    public Canvas mainCanvas;

    public Text scoreText;
    public Text dynamitePriceText;
    public Text drugPriceText;
    public Text luckPriceText;
    public Text strengthPriceText;
    public Text bookPriceText;
    public Sprite owner;

    private GameObject itemDynamite;
    private GameObject itemDrug;
    private GameObject itemLuck;
    private GameObject itemStrength;
    private GameObject itemBook;

    private int dynamitePrice = 200; private string dynamiteName = "Dynamite";
    private int drugPrice = 550; private string drugName = "Drug";
    private int luckPrice = 950; private string luckName = "Luck";
    private int strengthPrice = 350; private string strengName = "Streng";
    private int bookPrice = 350; private string bookName = "Book Rock";

    public float hookSpeed = 1f;
    public int upPriceDia = 10;
    public float dynamitePower = 1f;
    public float luckMultiplier = 1f;

    private Button dynamiteButton;
    private Button drugButton;
    private Button luckButton;
    private Button strengthButton;
    private Button bookButton;
    private Button continueButton;

    [SerializeField] private Sprite dynamiteSprite;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        if (mainCanvas == null)
        {
            mainCanvas = FindObjectOfType<Canvas>();
            if (mainCanvas != null)
            {
                DontDestroyOnLoad(mainCanvas.gameObject);
            }
            else
            {
                Debug.LogError("No Canvas found in the scene during Awake!");
            }
        }
    }

    public void BuyLuck()
    {
        if (ScoreControl.instance.SpendScore(luckPrice))
        {
            ItemManager.Instance.AddItem("Luck", null, () =>
            {
                Spawner.instance.ApplyLuckBoost();
            });
            Destroy(itemLuck);
            itemLuck = null;
            UpdateUI();
        }
        else
        {
            Debug.Log("Không đủ vàng để mua Luck!");
        }
    }
    public void BuyDynamite()
    {
        if (ScoreControl.instance.SpendScore(dynamitePrice))
        {
            ItemManager.Instance.AddItem("Dynamite", dynamiteSprite, () =>
            {
                Debug.Log("Dynamite added to inventory");
            });

            int dynamiteCount = ItemManager.Instance.GetItemCount("Dynamite");
            if (UIManager.instance != null)
            {
                UIManager.instance.UpdateDynamiteCount(dynamiteCount);
            }
            Destroy(itemDynamite);
            itemDynamite = null;
            UpdateUI();
        }
        else
        {
            Debug.Log("Không đủ vàng để mua Dynamite!");
        }
    }
    public void BuyDrug()
    {
        if (ScoreControl.instance.SpendScore(drugPrice))
        {
            Debug.Log("Đã mua Drug!");
            ItemManager.Instance.AddItem("Drug", null, () =>
            {
                Spawner.instance.ApplyDrugEffect();
                UIManager.instance?.ShowNotification("Drug đã được kích hoạt! Tăng giá trị Diamond.");
            });
            Destroy(itemDrug);
            itemDrug = null;
            UpdateUI();
        }
        else
        {
            Debug.Log("Không đủ vàng để mua Drug!");
        }
    }
    public void BuyStreng()
    {
        if (ScoreControl.instance.SpendScore(strengthPrice))
        {
            ItemManager.Instance.AddItem("Streng", null, () =>
            {
                Debug.Log("Strength đã được sử dụng");
            });
            Destroy(itemStrength);
            itemStrength = null;
            UpdateUI();
        }
    }
    private void BuyBook()
    {
        if(ScoreControl.instance.SpendScore(bookPrice))
        {
            Debug.Log("Đã mua Book!");
            ItemManager.Instance.AddItem("Book", owner, () =>
            {
                Spawner.instance.ApplyBookEffect();
            });
            Destroy(itemBook);
            itemBook = null;
            UpdateUI();
        }
    }

    public void ShowShop()
    {
        GameManager.instance.PauseGameForShop();

        if (mainCanvas == null || mainCanvas.Equals(null))
        {
            mainCanvas = FindObjectOfType<Canvas>();
            if (mainCanvas == null)   {    return;            }
        }

        if (shopInstance == null)
        {
            shopInstance = Instantiate(shopUIPrefab, mainCanvas.transform);
            shopInstance.transform.localPosition = Vector3.zero;
            shopInstance.transform.localScale = Vector3.one;

            scoreText = shopInstance.transform.Find("ShopUIPanel/Score").GetComponent<Text>();
            dynamitePriceText = shopInstance.transform.Find("ShopUIPanel/ItemDynamite/Price").GetComponent<Text>();
            drugPriceText = shopInstance.transform.Find("ShopUIPanel/ItemDrug/Price").GetComponent<Text>();
            luckPriceText = shopInstance.transform.Find("ShopUIPanel/ItemLuck/Price").GetComponent<Text>();
            strengthPriceText = shopInstance.transform.Find("ShopUIPanel/ItemStreng/Price").GetComponent<Text>();
            bookPriceText = shopInstance.transform.Find("ShopUIPanel/ItemBook/Price").GetComponent<Text>();

            Text dynamiteNameText = shopInstance.transform.Find("ShopUIPanel/ItemDynamite/Name").GetComponent<Text>();
            Text drugNameText = shopInstance.transform.Find("ShopUIPanel/ItemDrug/Name").GetComponent<Text>();
            Text luckNameText = shopInstance.transform.Find("ShopUIPanel/ItemLuck/Name").GetComponent<Text>();
            Text strengthNameText = shopInstance.transform.Find("ShopUIPanel/ItemStreng/Name").GetComponent<Text>();
            Text bookNameText = shopInstance.transform.Find("ShopUIPanel/ItemBook/Name").GetComponent<Text>();

            dynamiteNameText.text = dynamiteName;
            drugNameText.text = drugName;
            luckNameText.text = luckName;
            strengthNameText.text = strengName;
            bookNameText.text = bookName;

            itemDynamite = shopInstance.transform.Find("ShopUIPanel/ItemDynamite").gameObject;
            itemDrug = shopInstance.transform.Find("ShopUIPanel/ItemDrug").gameObject;
            itemLuck = shopInstance.transform.Find("ShopUIPanel/ItemLuck").gameObject;
            itemStrength = shopInstance.transform.Find("ShopUIPanel/ItemStreng").gameObject;
            itemBook = shopInstance.transform.Find("ShopUIPanel/ItemBook").gameObject;

            dynamiteButton = shopInstance.transform.Find("ShopUIPanel/ItemDynamite").GetComponentInChildren<Button>();
            dynamiteButton.onClick.AddListener(BuyDynamite);

            drugButton = shopInstance.transform.Find("ShopUIPanel/ItemDrug").GetComponentInChildren<Button>();
            drugButton.onClick.AddListener(BuyDrug);

            luckButton = shopInstance.transform.Find("ShopUIPanel/ItemLuck").GetComponentInChildren<Button>();
            luckButton.onClick.AddListener(BuyLuck);

            strengthButton = shopInstance.transform.Find("ShopUIPanel/ItemStreng").GetComponentInChildren<Button>();
            strengthButton.onClick.AddListener(BuyStreng);

            bookButton = shopInstance.transform.Find("ShopUIPanel/ItemBook").GetComponentInChildren<Button>();
            bookButton.onClick.AddListener(BuyBook);

            continueButton = shopInstance.transform.Find("ShopUIPanel/Continue").GetComponent<Button>();
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(HideShop);
            }
        }
        shopInstance.SetActive(true);
        UpdateUI();
    }

    public void HideShop()
    {
        if (shopInstance != null)
        {

            if (dynamiteButton != null) dynamiteButton.onClick.RemoveAllListeners();
            if (drugButton != null) drugButton.onClick.RemoveAllListeners();
            if (luckButton != null) luckButton.onClick.RemoveAllListeners();
            if (strengthButton != null) strengthButton.onClick.RemoveAllListeners();
            if (continueButton != null) continueButton.onClick.RemoveAllListeners();
            if(bookButton != null) bookButton.onClick.RemoveAllListeners();

            dynamiteButton = null;
            drugButton = null;
            luckButton = null;
            strengthButton = null;
            bookButton = null;
            continueButton = null;

            scoreText = null;
            dynamitePriceText = null;
            drugPriceText = null;
            luckPriceText = null;
            bookPriceText = null;
            strengthPriceText = null;

            itemDynamite = null;
            itemDrug = null;
            itemLuck = null;
            itemBook = null;
            itemStrength = null;

            Destroy(shopInstance);
            shopInstance = null;

            try
            {
                GameManager.instance.ResumeGameAfterShop();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Lỗi khi gọi GameManager: " + e.Message);
            }
        }
        else
        {
            Debug.LogWarning("shopInstance là null, không thể đóng Shop!");
        }
    }

    void UpdateUI()
    {
        scoreText.text = "Vàng: " + ScoreControl.instance.GetCurrentScore();
        if (itemDynamite != null) dynamitePriceText.text = dynamitePrice + " Vàng";
        if (itemDrug != null) drugPriceText.text = drugPrice + " Vàng";
        if (itemLuck != null) luckPriceText.text = luckPrice + " Vàng";
        if (itemStrength != null) strengthPriceText.text = strengthPrice + " Vàng";
        if (itemBook != null) bookPriceText.text = bookPrice + " Vàng";  
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
