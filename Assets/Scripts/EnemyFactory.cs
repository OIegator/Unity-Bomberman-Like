using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    private Bomberman _player;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private List<GameObject> spawnPoints = new();
    [SerializeField] private int maxEnemyCount = 2;
    private Coroutine _spawnCoroutine;
    private int _spawnedEnemyCount = 0;

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
        _spawnCoroutine ??= StartCoroutine(SpawnEnemies());
    }

    public void StopFactory()
    {
        if (_spawnCoroutine == null) return;
        StopCoroutine(_spawnCoroutine);
        _spawnCoroutine = null;
    }

    public void DecreaseEnemyCount()
    {
        _spawnedEnemyCount--;
    }
    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (spawnPoints.Count > 0 && _spawnedEnemyCount < maxEnemyCount)
            {
                int randomIndex = Random.Range(0, spawnPoints.Count);
                GameObject spawnPoint = spawnPoints[randomIndex];

                GameObject enemyObject = ObjectPoolManager.Instance.GetObject(ObjectType.Enemy);
                enemyObject.transform.position = spawnPoint.transform.position + Vector3.up * 0.8f;
                enemyObject.GetComponent<Enemy>().Setup(_player, spawnPoint);
                _spawnedEnemyCount++;
            }
        }
    }
}