using UnityEngine;
using UnityEngine.UI;

public class StoreItem : MonoBehaviour
{
    [SerializeField] Text ItemName;
    [SerializeField] Text ItemDescription;
    [SerializeField] Text ItemPrice;
    [SerializeField] Button Item;
    [SerializeField] Button itemButton;

    public void SetItemPosition(Vector2 pos)
    {
        GetComponent<RectTransform>().anchoredPosition += pos;
    }
    public void SetItemName(string name)
    {
        ItemName.text = name;
    }
    public void SetItemPrice(int price)
    {
        ItemPrice.text = price.ToString();
    }
    public void SetItemDescription(string des)
    {
        ItemDescription.text = des;
    }
    public void SetItemisPurchased()
    {
        Item.gameObject.SetActive(false);
        itemButton.interactable=true;
    }
}
