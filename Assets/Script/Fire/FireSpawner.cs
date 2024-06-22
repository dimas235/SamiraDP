using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class FireSpawner : MonoBehaviour
{
    public List<GameObject> firePrefabs; // Daftar prefab api tetap
    public Transform playerTransform; // Referensi ke transformasi pemain
    public float minDistanceFromPlayer = 0.5f;
    public float maxDistanceFromPlayer = 3.0f;
    public float minimumDistanceBetweenFires = 2.0f; // Mengubah jarak minimum menjadi 2
    public ARPlaneManager arPlaneManager; // Referensi ke ARPlaneManager

    private List<Vector3> spawnPositions = new List<Vector3>();
    public List<GameObject> currentFireObjects { get; private set; } = new List<GameObject>(); // Daftar objek api saat ini di scene
    private bool spawnPointsAdded = false; // Flag untuk memastikan titik spawn hanya ditambahkan sekali
    private bool hasSpawned = false; // Flag untuk memastikan api hanya di-spawn sekali

    void Start()
    {
        Debug.Log("FireSpawner dimulai");
    }

    private void AddSpawnPointsForExistingPlanes()
    {
        foreach (var plane in arPlaneManager.trackables)
        {
            AddSpawnPointsOnPlane(plane);
        }
    }

    private void AddSpawnPointsOnPlane(ARPlane plane)
    {
        float gridSize = minimumDistanceBetweenFires;
        Vector3 planeCenter = plane.center;
        Vector2 planeSize = plane.size;

        for (float x = -planeSize.x / 2; x <= planeSize.x / 2; x += gridSize)
        {
            for (float z = -planeSize.y / 2; z <= planeSize.y / 2; z += gridSize)
            {
                Vector3 potentialSpawnPoint = planeCenter + new Vector3(x, 0, z);
                if (IsValidPosition(potentialSpawnPoint))
                {
                    spawnPositions.Add(potentialSpawnPoint);
                    Debug.Log("Titik spawn ditambahkan pada posisi: " + potentialSpawnPoint);
                }
            }
        }

        if (spawnPositions.Count < firePrefabs.Count)
        {
            Debug.LogWarning("Tidak dapat menemukan cukup titik spawn yang valid.");
        }
    }

    private bool IsValidPosition(Vector3 position)
    {
        // Cek jarak dari pemain
        float distanceFromPlayer = Vector3.Distance(position, playerTransform.position);
        if (distanceFromPlayer < minDistanceFromPlayer || distanceFromPlayer > maxDistanceFromPlayer)
        {
            Debug.Log("Posisi terlalu dekat atau terlalu jauh dari pemain: " + position);
            return false;
        }

        // Cek jarak antar api
        foreach (var existingPosition in spawnPositions)
        {
            if (Vector3.Distance(position, existingPosition) < minimumDistanceBetweenFires)
            {
                Debug.Log("Posisi terlalu dekat dengan api lain: " + position);
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
        Debug.Log("Respawning fires...");
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
        hasSpawned = false; // Reset flag untuk memungkinkan spawn kembali
    }

    public void DestroyRemainingFires()
    {
        Debug.Log("Menghancurkan api yang tersisa...");
        for (int i = 0; i < currentFireObjects.Count; i++)
        {
            if (currentFireObjects[i] != null)
            {
                Destroy(currentFireObjects[i]);
                currentFireObjects[i] = null; // Set ke null agar dapat di-spawn ulang nanti
            }
        }
        currentFireObjects.Clear();
        hasSpawned = false; // Reset flag untuk memungkinkan spawn kembali
        Debug.Log("Semua api yang tersisa dihancurkan.");
    }

    public void SpawnFiresAtSpawnPoints()
    {
        if (!spawnPointsAdded)
        {
            Debug.LogWarning("Titik spawn belum ditambahkan. Panggil EnableSpawning terlebih dahulu.");
            return;
        }

        if (hasSpawned)
        {
            Debug.LogWarning("Api sudah di-spawn, tidak bisa spawn lagi.");
            return;
        }

        Debug.Log("Menambahkan api pada titik spawn...");

        if (spawnPositions.Count < firePrefabs.Count)
        {
            Debug.LogWarning("Tidak cukup titik spawn untuk menambahkan semua api.");
            return;
        }

        // Menggunakan HashSet untuk memastikan posisi unik
        HashSet<Vector3> usedSpawnPositions = new HashSet<Vector3>();

        for (int i = 0; i < firePrefabs.Count; i++)
        {
            Vector3 spawnPoint = spawnPositions[i];
            usedSpawnPositions.Add(spawnPoint);

            GameObject fireInstance = Instantiate(firePrefabs[i], spawnPoint, Quaternion.identity);
            currentFireObjects.Add(fireInstance);
            Debug.Log("Api ditambahkan pada posisi: " + spawnPoint);
        }

        hasSpawned = true; // Set flag untuk mencegah spawn berulang
    }

    public void EnableSpawning()
    {
        if (!spawnPointsAdded)
        {
            Debug.Log("Menambahkan titik spawn...");
            AddSpawnPointsForExistingPlanes();
            spawnPointsAdded = true; // Set flag untuk mencegah penambahan berulang
        }
    }
}
