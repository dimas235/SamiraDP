using System.Collections.Generic;
using UnityEngine;

public class FireSpawner : MonoBehaviour
{
    public List<GameObject> firePrefabs;
    public Transform playerTransform; // Reference to the player's transform
    public float minDistanceFromPlayer = 0.5f;
    public float maxDistanceFromPlayer = 3.0f;
    public float minimumDistanceBetweenFires = 1.0f;
    public LayerMask floorLayerMask; // Layer mask to identify floor surfaces
    private List<Vector3> spawnPositions = new List<Vector3>();

    public List<GameObject> currentFireObjects { get; private set; } = new List<GameObject>(); // Daftar objek api saat ini di scene

    void Start()
    {
        Debug.Log("RoomRecognitionAndFireSpawner started");
    }

    public void SimulateRoomRecognition()
    {
        int spawnCount = 0;
        spawnPositions.Clear();
        currentFireObjects.Clear();
        // This example uses raycasting to detect the floor and spawn fires
        for (int i = 0; i < firePrefabs.Count; i++) // Ensure all firePrefabs are spawned
        {
            bool spawned = false;
            int attempts = 0;

            while (!spawned && attempts < 100) // Allow up to 100 attempts to find a valid position
            {
                Vector3 randomPoint = new Vector3(Random.Range(-5.0f, 5.0f), 10.0f, Random.Range(-5.0f, 5.0f));
                Debug.Log("Raycasting from point: " + randomPoint);
                if (Physics.Raycast(randomPoint, Vector3.down, out RaycastHit hit, Mathf.Infinity, floorLayerMask))
                {
                    Debug.Log("Hit detected at point: " + hit.point);
                    Vector3 spawnPoint = hit.point;
                    if (IsValidPosition(spawnPoint))
                    {
                        GameObject fireInstance = Instantiate(firePrefabs[i], spawnPoint, Quaternion.identity);
                        currentFireObjects.Add(fireInstance);
                        spawnPositions.Add(spawnPoint);
                        Debug.Log("Fire spawned at position: " + spawnPoint);
                        spawned = true;
                        spawnCount++;
                    }
                }
                attempts++;
            }

            if (!spawned)
            {
                Debug.LogWarning("Failed to spawn fire prefab: " + firePrefabs[i].name);
            }
        }

        if (spawnCount < firePrefabs.Count)
        {
            Debug.LogWarning("Not all fire prefabs were successfully spawned.");
        }
        else
        {
            Debug.Log("All fire prefabs successfully spawned.");
        }
    }

    private bool IsValidPosition(Vector3 position)
    {
        foreach (var existingPosition in spawnPositions)
        {
            if (Vector3.Distance(position, existingPosition) < minimumDistanceBetweenFires)
            {
                return false;
            }
        }

        // Check distance from player
        float distanceFromPlayer = Vector3.Distance(position, playerTransform.position);
        if (distanceFromPlayer < minDistanceFromPlayer || distanceFromPlayer > maxDistanceFromPlayer)
        {
            return false;
        }


        return true;
    }

    public void DeactivateAllFires()
    {
        foreach (var fire in currentFireObjects)
        {
            if (fire != null)
            {
                fire.SetActive(false);
            }
        }
    }

    public void RespawnFires()
    {
        foreach (var fire in currentFireObjects)
        {
            if (fire != null)
            {
                Destroy(fire);
            }
        }
        SimulateRoomRecognition();
    }

    public void DestroyRemainingFires()
    {
        for (int i = 0; i < currentFireObjects.Count; i++)
        {
            if (currentFireObjects[i] != null)
            {
                Fire fireComponent = currentFireObjects[i].GetComponent<Fire>();
                if (fireComponent != null && !fireComponent.canTakeDamage)
                {
                    Destroy(currentFireObjects[i]);
                    currentFireObjects[i] = null; // Set to null so it can be respawned later
                }
            }
        }
    }
}
