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
    private bool isLuckActive ;
    private float diamondLuckBoost = 2.25f;

    private bool applyLuckToNextLevel = false;
    private bool applyBookToNextLevel = false;
    private bool applyDrugtoNextLevel = false;
    [SerializeField] private SpawnableObject[] spawnableObjects; 
    [SerializeField] private int maxAttempts = 1000000000;          
    [SerializeField] private float minSpawnDistance = 1.5f;    
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-6.8f, -4.5f); 
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(6.8f, 0.17f);  

    private HashSet<Vector2> usedPositions = new HashSet<Vector2>();
    private bool isDrugActive;
    private bool isBookActive;

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
        int currentLevel = GameManager.instance != null ? GameManager.instance.currentLevel : 1;
        foreach (var spawnable in spawnableObjects)
        {
            int itemsToSpawn = Random.Range(spawnable.minCount, spawnable.maxCount + 1);
            float spawnChance = spawnable.spawnChance;
            if (isLuckActive && spawnable.prefab.name == "kc_2_0")
            {
                spawnChance *= diamondLuckBoost;  
            }

            for (int i = 0; i < itemsToSpawn; i++)
            {
                float chance = Random.value;

                    Vector2 spawnPoint = GetValidSpawnPoint();
                    if (spawnPoint != Vector2.zero)
                    {
                        GameObject spawnedObject = Instantiate(spawnable.prefab, spawnPoint, Quaternion.identity);
                        spawnedObject.name = spawnable.prefab.name;
                        usedPositions.Add(spawnPoint);
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
        if (applyLuckToNextLevel || applyDrugtoNextLevel )
        {
            isLuckActive = true;
            isDrugActive = true;

            applyLuckToNextLevel = false;
            applyDrugtoNextLevel = false;
        }
        else
        {
            isLuckActive = false;
            isDrugActive = false;
        }
        if (applyBookToNextLevel)
        {
            isBookActive = true;
            applyBookToNextLevel = false;
        }
        else        { isBookActive = false; }
        usedPositions.Clear();
        SpawnObjects();
    }
    public void ApplyLuckBoost()
    {
        applyBookToNextLevel = true;
    }

    public void ApplyDrugEffect()
    {
        applyDrugtoNextLevel=true;
    }
    public void ApplyBookEffect()
    {
        applyBookToNextLevel = true;
    }
    public bool IsDrugActive
    {
        get { return isDrugActive; }
    }
}