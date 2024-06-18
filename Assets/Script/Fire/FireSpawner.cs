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
    }

    void OnDestroy()
    {
        arPlaneManager.planesChanged -= OnPlanesChanged;
    }

    private void OnPlanesChanged(ARPlanesChangedEventArgs args)
    {
        foreach (var plane in args.added)
        {
            // Check if we can spawn a fire prefab at the center of the plane
            Vector3 spawnPoint = plane.center;
            if (IsValidPosition(spawnPoint))
            {
                // Instantiate the fire prefab at the plane's center
                GameObject firePrefab = GetNextFirePrefab();
                if (firePrefab != null)
                {
                    GameObject fireInstance = Instantiate(firePrefab, spawnPoint, Quaternion.identity);
                    currentFireObjects.Add(fireInstance);
                    spawnPositions.Add(spawnPoint);
                    Debug.Log("Fire spawned at position: " + spawnPoint);
                }
            }
        }
    }

    private GameObject GetNextFirePrefab()
    {
        if (firePrefabs.Count > 0)
        {
            GameObject firePrefab = firePrefabs[0];
            firePrefabs.RemoveAt(0);
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

    private void SimulateRoomRecognition()
    {
        // This method can be used to simulate the room recognition if needed
        // For now, it's left empty as the primary spawning is handled by AR plane detection
    }
}
