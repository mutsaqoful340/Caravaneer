using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner_Enemy : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject playerConstraint;
    [SerializeField] private GameObject[] enemyPrefab;
    [SerializeField] private float minSpawnRadius = 10f;
    [SerializeField] private float maxSpawnRadius = 20f;
    [SerializeField] private int enemiesToSpawn = 3;
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private float spawnRayHeight = 50f;
    [SerializeField] private float spawnRayDistance = 200f;
    [SerializeField] private float enemySpawnHeightOffset = 0.5f;

    private float _spawnTimer;

    [Header("Additional References")]
    public GameObject folder;

    [Header("Debug")]
    [SerializeField] private bool isSpawning = true;
    [SerializeField] private GameObject[] enemies;

    void Start()
    {
        if (folder == null)
        {
            Debug.LogWarning("Spawner_Enemy: 'folder' is not assigned. Enemies will be spawned at root level.");
        }
    }

    private void Update()
    {
        if (!isSpawning) return;

        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0f)
        {
            OnSpawnEnemy();
            _spawnTimer = spawnInterval;
        }
    }

    private void OnSpawnEnemy()
    {
        List<GameObject> spawnedEnemies = new List<GameObject>();

        if (enemyPrefab != null && folder != null)
        {
            Vector3 center = playerConstraint != null ? playerConstraint.transform.position : Vector3.zero;
            for (int i = 0; i < enemiesToSpawn; i++)
            {
                if (!TryGetSpawnPosition(center, out Vector3 spawnPos))
                {
                    continue;
                }

                GameObject prefab = enemyPrefab[Random.Range(0, enemyPrefab.Length)];
                GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity, folder.transform);
                spawnedEnemies.Add(enemy);
            }
        }
        else
        {
            Debug.LogWarning("Enemy prefab or spawn point is not assigned.");
        }

        OnAddEnemyToTheList(spawnedEnemies.ToArray());
    }

    private bool TryGetSpawnPosition(Vector3 center, out Vector3 spawnPos)
    {
        if (groundLayerMask.value == 0)
        {
            Debug.LogWarning("Spawner_Enemy: 'groundLayerMask' is not assigned. Consider setting the ground layer to prevent edge spawns.");
            spawnPos = center;
            return false;
        }

        for (int attempt = 0; attempt < 10; attempt++)
        {
            float radius = Random.Range(minSpawnRadius, maxSpawnRadius);
            Vector2 randomCircle = Random.insideUnitCircle.normalized * radius;
            Vector3 candidatePos = center + new Vector3(randomCircle.x, 0f, randomCircle.y);
            Vector3 rayOrigin = candidatePos + Vector3.up * spawnRayHeight;

            if (Physics.Raycast(rayOrigin, Vector3.down, out RaycastHit hit, spawnRayDistance, groundLayerMask))
            {
                spawnPos = hit.point + Vector3.up * enemySpawnHeightOffset;
                return true;
            }
        }

        spawnPos = center;
        return false;
    }

    private void OnAddEnemyToTheList(GameObject[] newlySpawnedEnemies)
    {
        if (newlySpawnedEnemies == null || newlySpawnedEnemies.Length == 0)
        {
            return;
        }

        List<GameObject> trackedEnemies = enemies == null ? new List<GameObject>() : new List<GameObject>(enemies);
        trackedEnemies.AddRange(newlySpawnedEnemies);
        enemies = trackedEnemies.ToArray();
    }
}