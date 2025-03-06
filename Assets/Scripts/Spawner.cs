using UnityEngine;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public static Spawner instance;
    [SerializeField] private GameObject[] objectsToSpawn;
    [SerializeField] private int maxAttempts = 100;
    private float minSpawnDistance = 1.5f;
    [SerializeField] private int minObjects = 3, maxObjects = 7;
    private HashSet<Vector2> usedPositions = new HashSet<Vector2>();

    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-6.8f, -4.5f);
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(6.8f, 0.17f);

    void Start()
    {
        SpawnObjects();
    }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    private void SpawnObjects()
    {
        foreach (var objectToSpawn in objectsToSpawn)
        {
            int itemsToSpawn = Random.Range(minObjects, maxObjects);
            for (int i = 0; i < itemsToSpawn; i++)
            {
                Vector2 spawnPoint = GetValidSpawnPoint();
                if (spawnPoint != Vector2.zero)
                {
                    Instantiate(objectToSpawn, spawnPoint, Quaternion.identity);
                    usedPositions.Add(spawnPoint);
                }
                else
                {
                    Debug.LogWarning($"Failed to find spawn point for {objectToSpawn.name}");
                }
            }
        }
    }

    private Vector2 GetValidSpawnPoint()
    {
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector2 randomPoint = new Vector2(
                Random.Range(spawnAreaMin.x, spawnAreaMax.x),
                Random.Range(spawnAreaMin.y, spawnAreaMax.y)
            );

            if (!IsPositionOccupied(randomPoint))
                return randomPoint;
        }
        Debug.LogWarning("Could not find a valid spawn point.");
        return Vector2.zero;
    }

    private bool IsPositionOccupied(Vector2 position)
    {
        foreach (var usedPosition in usedPositions)
        {
            if (Vector2.Distance(usedPosition, position) < minSpawnDistance)
            {
                return true;
            }
        }
        return false;
    }

    public void ResetLevel()
    {
        foreach (var obj in GameObject.FindObjectsOfType<GameObject>())
        {
            string objName = obj.name.Replace("(Clone)", "").Trim();

            // Kiểm tra nếu objName tồn tại trong objectsToSpawn
            if (System.Array.Exists(objectsToSpawn, element => element.name == objName))
            {
                Destroy(obj);
            }
        }

        usedPositions.Clear();
        SpawnObjects();

        Debug.Log("New level has been created!");
    }

}
