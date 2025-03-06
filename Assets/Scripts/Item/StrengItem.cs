using UnityEngine;

public class StrengItem : ItemData
{
    public StrengItem instance;
    public string nameItem = "Streng";
    public string descriptionItem = "Streng Item";
    public int id = 3;
    public int price = 100;
    public bool isStackable;
    public bool isUseable;

    private void Update()
    {
    }
    public void Buy()
    {
        Debug.Log("Buy " + nameItem);
    }
}
