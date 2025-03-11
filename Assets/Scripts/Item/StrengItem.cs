using Unity.VisualScripting;
using UnityEngine;

public class StrengItem : ItemData
{
    [SerializeField] private float weightReduction = 0.5f;
    [SerializeField] private float duration = 10f;

    private void Awake()
    {
        itemName = "Strength";
        cost = 120;
    }

    public override void UseItem(Pod pod)
    {
        if (!isAvailable) return;

        pod.StartCoroutine(ApplyStrengthBoost(pod));
        isAvailable = false;
        Debug.Log("Strength used! Weight reduced.");
    }

    private System.Collections.IEnumerator ApplyStrengthBoost(Pod pod)
    {
        yield return new WaitForSeconds(duration);
    }
}
