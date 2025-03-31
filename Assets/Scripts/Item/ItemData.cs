using UnityEngine;

[System.Serializable]
public class ItemData
{
    public string itemName;
    public int quantity;
    public Sprite itemIcon;
    public System.Action onUse; // Hành động khi sử dụng vật phẩm

    public ItemData(string name, int qty, Sprite icon = null, System.Action useAction = null)
    {
        itemName = name;
        quantity = qty;
        itemIcon = icon;
        onUse = useAction;
    }
}