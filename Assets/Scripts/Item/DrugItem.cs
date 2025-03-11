using Unity.VisualScripting;
using UnityEngine;

public class DrugItem : ItemData
{
    [SerializeField] private float speedBoost = 2f;
    [SerializeField] private float duration = 5f;

    private void Awake()
    {
        itemName = "Drug";
        cost = 80;
    }

    public override void UseItem(Pod pod)
    {
        if (!isAvailable) return;

        pod.StartCoroutine(ApplySpeedBoost(pod));
        isAvailable = false;
        Debug.Log("Drug used! Speed boosted.");
    }

    private System.Collections.IEnumerator ApplySpeedBoost(Pod pod)
    {
        float originalSpeed = pod._scrollSpeed;
        pod._scrollSpeed *= speedBoost;
        yield return new WaitForSeconds(duration);
        pod._scrollSpeed = originalSpeed;
    }
}
