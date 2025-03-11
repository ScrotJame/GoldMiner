using Unity.VisualScripting;
using UnityEngine;

public class DynamiteItem : ItemData
{
    
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionRadius = 2f;

    private void Awake()
    {
        itemName = "Dynamite";
        cost = 100;   
    }

    public override void UseItem(Pod pod)
    {
        if (!isAvailable) return;

        Vector3 spawnPosition = pod.transform.position + Vector3.down * 2f;
        GameObject explosion = Instantiate(explosionPrefab, spawnPosition, Quaternion.identity);

        Collider2D[] nearbyObjects = Physics2D.OverlapCircleAll(spawnPosition, explosionRadius);
        foreach (var obj in nearbyObjects)
        {
            if (obj.CompareTag("Item"))
            {
                Destroy(obj.gameObject);
            }
        }

        Destroy(explosion, 1f);
        isAvailable = false;
        Debug.Log("Dynamite used!");
    }
}

