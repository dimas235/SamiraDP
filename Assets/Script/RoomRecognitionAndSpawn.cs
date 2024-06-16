using UnityEngine;
using System.Collections.Generic;

public class RoomRecognitionAndSpawn : MonoBehaviour
{
    public List<GameObject> firePrefabs;
    public float minimumDistanceBetweenFires = 1.0f;
    public LayerMask floorLayerMask; // Layer mask to identify floor surfaces
    private List<Vector3> spawnPositions = new List<Vector3>();

    void Start()
    {
        Debug.Log("RoomRecognitionAndSpawn started");
        // Simulate the room recognition by raycasting
        SimulateRoomRecognition();
    }

    private void SimulateRoomRecognition()
    {
        int spawnCount = 0;
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
                        Instantiate(firePrefabs[i], spawnPoint, Quaternion.identity);
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
        return true;
    }
}
