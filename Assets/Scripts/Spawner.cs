using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class Spawner : MonoBehaviour
{
    public static Spawner instance;
    [System.Serializable]
    public class SpawnableObject
    {
        public GameObject prefab;           
        public int minCount;               
        public int maxCount;
        [Range(0f, 1f)]                    
        public float spawnChance;
        [HideInInspector]
        public string objectType => prefab != null ? prefab.name : "Unknown";
    }
    private bool isLuckActive = false;
    private float diamondLuckBoost = 0.5f;

    [SerializeField] private SpawnableObject[] spawnableObjects; 
    [SerializeField] private int maxAttempts = 1000000000;          
    [SerializeField] private float minSpawnDistance = 1.5f;    
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-6.8f, -4.5f); 
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(6.8f, 0.17f);  

    private HashSet<Vector2> usedPositions = new HashSet<Vector2>(); 
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

    void Start()
    {
        SpawnObjects();
    }

    private void SpawnObjects()
    {
        usedPositions.Clear(); 
        int currentLevel = GameManager.instance != null ? GameManager.instance.currentLevel: 1  ;
        foreach (var spawnable in spawnableObjects)
        {
            int itemsToSpawn = Random.Range(spawnable.minCount, spawnable.maxCount + 1);

            for (int i = 0; i < itemsToSpawn; i++)
            {
                float chance = Random.value;
                float adjustedChance = spawnable.spawnChance * (currentLevel / 10f);
                if (isLuckActive && spawnable.objectType == "Diamond")
                {
                    adjustedChance = Mathf.Min(1f, spawnable.spawnChance + diamondLuckBoost);
                }
                if (chance <= adjustedChance ) ;
                    Vector2 spawnPoint = GetValidSpawnPoint();
                if (spawnPoint != Vector2.zero)
                {
                    GameObject spawnedObject = Instantiate(spawnable.prefab, spawnPoint, Quaternion.identity);
                    spawnedObject.name = spawnable.prefab.name; 
                    usedPositions.Add(spawnPoint);
                }
                else
                {
                    Debug.LogWarning($"Failed to find spawn point for {spawnable.objectType}");
                    break;
                }
            }
            Debug.Log($"Spawned {itemsToSpawn} {spawnable.objectType}");
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
        Debug.LogWarning("Could not find a valid spawn point after max attempts.");
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
            if (System.Array.Exists(spawnableObjects, element => element.prefab != null && element.prefab.name == objName))
            {
                Destroy(obj);
            }
        }
        isLuckActive = false;
        usedPositions.Clear();
        SpawnObjects();

        Debug.Log("New level has been created!");
    }
    public IEnumerator ApplyLuckBoost(float duration)
    {
        isLuckActive = true;
        yield return new WaitForSeconds(duration);
        isLuckActive = false;
        Debug.Log("Luck boost ended.");
    }
}