using UnityEngine;

public class LuckItem : ItemData
{
    public string nameItem = "Luck Item";
    public string descriptionItem = "This item will increase your luck";
    public int id = 1;
    public int price = 100;
    public bool isStackable = false;
    public int maxStack = 1;
    public bool isEquippable;
    public bool isUsable = false;

    private void Update()
    {
        
    }
    private void Start()
    {
        sprite = Resources.Load<Sprite>("Sprites/LuckItem");
    }
    public void Buy()
    {
        Debug.Log("Using " + nameItem);
        isEquippable = true;
        //Destroy(gameObject);
    }
}
