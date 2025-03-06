using UnityEngine;

public class DrugItem : ItemData
{
    public string nameItem = "Drug";
    public string descriptionItem = "Drug Item";
    public int id = 4;
    public int price = 100;
    public bool isStackable;
    public bool isUseable;
    public void Buy()
    {
        Debug.Log("Buy " + nameItem);
       // Destroy(gameObject);
    }
}
