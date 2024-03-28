using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    private Bomberman _player;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private List<GameObject> spawnPoints = new();

    private Coroutine spawnCoroutine;
    private int spawnedEnemyCount = 0;

    public void Setup(Bomberman player)
    {
        this._player = player;
    }

    public void AddSpawnPoint(GameObject spawnPoint)
    {
        spawnPoints.Add(spawnPoint);
    }

    public void StartFactory()
    {
        spawnCoroutine ??= StartCoroutine(SpawnEnemies());
    }

    public void StopFactory()
    {
        if (spawnCoroutine == null) return;
        StopCoroutine(spawnCoroutine);
        spawnCoroutine = null;
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (spawnPoints.Count > 0 && spawnedEnemyCount < 1)
            {
                int randomIndex = Random.Range(0, spawnPoints.Count);
                GameObject spawnPoint = spawnPoints[randomIndex];

                GameObject enemyObject = ObjectPoolManager.Instance.GetObject(ObjectType.Enemy);
                enemyObject.transform.position = spawnPoint.transform.position + Vector3.up * 0.8f;
                enemyObject.GetComponent<Enemy>().Setup(_player, spawnPoint);
                spawnedEnemyCount++;
            }
        }
    }
}