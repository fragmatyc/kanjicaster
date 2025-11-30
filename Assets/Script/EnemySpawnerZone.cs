using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnerZone : MonoBehaviour
{
    bool alreadySpawned = false;
    public bool OneTimeSpawn = false;
    public GameObject[] enemyPrefabs;
    public Transform[] spawnPoints;
    public int ennemyNumber = 3;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (OneTimeSpawn && alreadySpawned)
            return;

        SpawnEnemies();
        alreadySpawned = true;
    }

    private void SpawnEnemies()
    {
        if (spawnPoints.Length == 0)
        {
            Debug.LogWarning("Pas de spawnPoints assignés sur " + name);
            return;
        }
        List<Transform> points = new List<Transform>(spawnPoints);

        for (int i = 0; i < ennemyNumber && points.Count > 0; i++)
        {
            
            int index = Random.Range(0, points.Count);
            Transform spawnPoint = points[index];

            Vector3 spawnPos = spawnPoint.position;

            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
            
            GameObject ennemyCreate = Instantiate(prefab, spawnPos, spawnPoint.rotation);
            Monster monsterComponent = ennemyCreate.GetComponent<Monster>();
            monsterComponent.enemyId = System.Guid.NewGuid().ToString();
            points.RemoveAt(index);
        }
    }
}
