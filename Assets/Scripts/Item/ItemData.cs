using UnityEngine;

public class ItemData
{
    public string nameItem;
    public string descriptionItem;
    public int id;
    public int price;
    public Sprite sprite;
    public bool isStackable;
    public int maxStack;
    public int currentStack;
    public bool isEquipped;
    public void Buy()
    {
        Debug.Log("Buy " + nameItem);
    }
}
