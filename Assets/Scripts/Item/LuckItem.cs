using UnityEngine;

public class LuckItem : ItemData
{
    [SerializeField] private float duration = 10f; // Thời gian hiệu ứng

    private void Awake()
    {
        itemName = "Luck";
        cost = 150;
    }

    public override void UseItem(Pod pod)
    {
        if (!isAvailable) return;

        Debug.Log("Luck activated! Higher chance for valuable items.");
        isAvailable = false;
        if (Spawner.instance != null)
        {
            Spawner.instance.StartCoroutine(Spawner.instance.ApplyLuckBoost(duration));
        }
        else
        {
            Debug.LogError("Spawner instance not found!");
        }
        Invoke(nameof(ResetCooldown), duration);
    }

    protected virtual void ResetCooldown()
    {
        isAvailable = true;
    }
}