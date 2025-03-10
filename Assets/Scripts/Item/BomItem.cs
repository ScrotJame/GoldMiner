using UnityEngine;

public class BomItem : ItemData
{
    public string nameItem = "Dynamite";
   public string descriptionItem = "Destroy Object";
    public int id = 2;
    public int price = 1000;
    public bool isStackable = true;
    public int maxStack = 3;
    public bool isEquippable;
    public bool isUsable;
    private void Update()
    {

    }
    private void Start()
    {
        sprite = Resources.Load<Sprite>("Sprites/BomItem");
    }
}
