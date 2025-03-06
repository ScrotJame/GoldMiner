using UnityEngine;

[CreateAssetMenu(fileName = "ShopItems", menuName = "Configuration/ShopItems")]
public class ShopItem : ScriptableObject
{
    public int id;
    public string title;
    public string description;
    public int price;
    public Sprite Sprite;
}
