using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject[] prefabsToSpawn;   // Array of bounceable prefabs
    public float minSpawnInterval = 3f;   // Minimum time between spawns
    public float maxSpawnInterval = 7f;   // Maximum time between spawns
    public Transform spawnPoint;          // optional: where to spawn

    private float timer;
    private float currentSpawnInterval;
    private bool hasSpawnedFirst = false; // Track if first object has been spawned

    void Start()
    {
        // Set the first spawn interval and spawn immediately
        SetRandomSpawnInterval();
        SpawnObject();
        hasSpawnedFirst = true;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= currentSpawnInterval)
        {
            SpawnObject();
            SetRandomSpawnInterval();
            timer = 0f;
        }
    }

    void SetRandomSpawnInterval()
    {
        currentSpawnInterval = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void SpawnObject()
    {
        if (prefabsToSpawn == null || prefabsToSpawn.Length == 0)
        {
            Debug.LogWarning("No prefabs assigned to BounceSpawner!");
            return;
        }

        // Randomly select a prefab from the array
        int randomIndex = Random.Range(0, prefabsToSpawn.Length);
        GameObject selectedPrefab = prefabsToSpawn[randomIndex];

        if (selectedPrefab == null)
        {
            Debug.LogWarning("Selected prefab is null!");
            return;
        }

        Vector3 spawnPosition = spawnPoint != null ? spawnPoint.position : transform.position;
        Quaternion spawnRotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

        Instantiate(selectedPrefab, spawnPosition, spawnRotation);

        if (!hasSpawnedFirst)
        {
            Debug.Log("First object spawned immediately!");
            hasSpawnedFirst = true;
        }
        else
        {
            Debug.Log($"Spawned: {selectedPrefab.name}, Next spawn in: {currentSpawnInterval:F1}s");
        }
    }

    // Optional: Method to change spawn interval range during gameplay
    public void ChangeSpawnIntervalRange(float newMin, float newMax)
    {
        minSpawnInterval = newMin;
        maxSpawnInterval = newMax;
        SetRandomSpawnInterval(); // Update current interval with new range
        timer = 0f; // Reset timer
    }

    // Optional: Method to add prefabs dynamically
    public void AddPrefab(GameObject newPrefab)
    {
        if (newPrefab == null) return;

        // Create new array with one more slot
        GameObject[] newPrefabs = new GameObject[prefabsToSpawn.Length + 1];

        // Copy old prefabs
        for (int i = 0; i < prefabsToSpawn.Length; i++)
        {
            newPrefabs[i] = prefabsToSpawn[i];
        }

        // Add new prefab
        newPrefabs[prefabsToSpawn.Length] = newPrefab;

        // Replace old array
        prefabsToSpawn = newPrefabs;
    }
}