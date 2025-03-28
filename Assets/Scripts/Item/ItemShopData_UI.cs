using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class ItemShopData_UI : MonoBehaviour
{
    [SerializeField] Button Item_image;
    [SerializeField] Text Item_name;
    [SerializeField] Text Item_price;

    public void SetItemPosition(Vector2 pos)
    {
        GetComponent<RectTransform>().anchoredPosition += pos;
    }
    public void SetItemName(string name) { 
        Item_name.text = name;
    }
    public void SetItemPrice(int price) { 
        Item_price.text = price.ToString();
    }
    public void SetItemisPurchased() { 
        Item_image.gameObject.SetActive(false);
    }
}
