using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class FireSpawner : MonoBehaviour
{
    public List<GameObject> firePrefabs;
    public Transform playerTransform; // Reference to the player's transform
    public float minDistanceFromPlayer = 0.5f;
    public float maxDistanceFromPlayer = 3.0f;
    public float minimumDistanceBetweenFires = 1.0f;
    public ARPlaneManager arPlaneManager; // Reference to ARPlaneManager

    private List<Vector3> spawnPositions = new List<Vector3>();
    public List<GameObject> currentFireObjects { get; private set; } = new List<GameObject>(); // List of current fire objects in the scene

    void Start()
    {
        Debug.Log("FireSpawner started");
        arPlaneManager.planesChanged += OnPlanesChanged;
        AddSpawnPointsForExistingPlanes();
    }

    void OnDestroy()
    {
        arPlaneManager.planesChanged -= OnPlanesChanged;
    }

    private void AddSpawnPointsForExistingPlanes()
    {
        foreach (var plane in arPlaneManager.trackables)
        {
            AddSpawnPoint(plane);
        }
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        foreach (var plane in args.added)
        {
            AddSpawnPoint(plane);
        }

        foreach (var plane in args.updated)
        {
            if (!spawnPositions.Contains(plane.center))
            {
                AddSpawnPoint(plane);
            }
        }
    }

    private void AddSpawnPoint(ARPlane plane)
    {
        Vector3 spawnPoint = plane.center;
        if (IsValidPosition(spawnPoint))
        {
            spawnPositions.Add(spawnPoint);
            Debug.Log("Spawn point added at position: " + spawnPoint);
        }
    }

    private GameObject GetNextFirePrefab()
    {
        if (firePrefabs.Count > 0)
        {
            int randomIndex = Random.Range(0, firePrefabs.Count);
            GameObject firePrefab = firePrefabs[randomIndex];
            firePrefabs.RemoveAt(randomIndex);
            return firePrefab;
        }
        return null;
    }

    private bool IsValidPosition(Vector3 position)
    {
        // Check distance from player
        float distanceFromPlayer = Vector3.Distance(position, playerTransform.position);
        if (distanceFromPlayer < minDistanceFromPlayer || distanceFromPlayer > maxDistanceFromPlayer)
        {
            return false;
        }

        // Check distance between fires
        foreach (var existingPosition in spawnPositions)
        {
            if (Vector3.Distance(position, existingPosition) < minimumDistanceBetweenFires)
            {
                return false;
            }
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
        currentFireObjects.Clear();
        spawnPositions.Clear();
        AddSpawnPointsForExistingPlanes();
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

    public void SpawnFiresAtSpawnPoints()
    {
        foreach (var spawnPoint in spawnPositions)
        {
            GameObject firePrefab = GetNextFirePrefab();
            if (firePrefab != null)
            {
                Vector3 randomSpawnPoint = GetRandomSpawnPoint();
                GameObject fireInstance = Instantiate(firePrefab, randomSpawnPoint, Quaternion.identity);
                currentFireObjects.Add(fireInstance);
                Debug.Log("Fire spawned at position: " + randomSpawnPoint);
            }
        }
    }

    private Vector3 GetRandomSpawnPoint()
    {
        if (spawnPositions.Count > 0)
        {
            int randomIndex = Random.Range(0, spawnPositions.Count);
            return spawnPositions[randomIndex];
        }
        return Vector3.zero;
    }
}
