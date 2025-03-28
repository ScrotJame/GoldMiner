using UnityEngine;

[System.Serializable]
public class ItemData 
{

    public enum ItemType
    {
        Dynamite,
        Drug,
        Streng,
        Luck,
        TimeClock
    }
    public static int Getcost(ItemType cost)
    {
        switch (cost)
        {
            default:
                case ItemType.Drug: return 700;
                case ItemType.Streng: return 600;
                case ItemType.Luck: return 1500;
                case ItemType.Dynamite: return 250;
        }
    }
    public static Sprite GetSprite(ItemType type) {
        switch (type) {
            default:
            case ItemType.Dynamite: return GameAsset.i.Dynamite;
            case ItemType.Drug: return GameAsset.i.Drug;
            case ItemType.Streng: return GameAsset.i.Streng;
            case ItemType.Luck: return GameAsset.i.Luck;
        }
    }
}