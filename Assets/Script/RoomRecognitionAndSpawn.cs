using UnityEngine;
using System.Collections.Generic;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class RoomRecognitionAndSpawn : MonoBehaviour
{
    public List<GameObject> firePrefabs;
    public float minimumDistanceBetweenFires = 1.0f;
    public ARPlaneManager arPlaneManager; // Reference to ARPlaneManager
    private List<Vector3> spawnPositions = new List<Vector3>();

    void Start()
    {
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
                    Instantiate(firePrefab, spawnPoint, Quaternion.identity);
                    spawnPositions.Add(spawnPoint);
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
