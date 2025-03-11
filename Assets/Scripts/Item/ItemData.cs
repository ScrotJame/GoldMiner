using UnityEngine;

public abstract class ItemData : MonoBehaviour
{
    [SerializeField] protected string itemName;        
    [SerializeField] protected int cost;           
    protected bool isAvailable = false;               
    public abstract void UseItem(Pod pod);

    public int GetCost() => cost;
    public bool IsAvailable() => isAvailable;
}