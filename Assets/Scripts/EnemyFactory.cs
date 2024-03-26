using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    private Bomberman player;
    [SerializeField] private float spawnInterval = 1f;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    private Coroutine spawnCoroutine;

    public void Setup(Bomberman player)
    {
        this.player = player;
    }

    public void AddSpawnPoint(Transform spawnPoint)
    {
        spawnPoints.Add(spawnPoint);
    }
    
    public void StartFactory()
    {
        if (spawnCoroutine == null)
        {
            spawnCoroutine = StartCoroutine(SpawnEnemies());
        }
    }

    public void StopFactory() 
    {
        if (spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if (spawnPoints.Count > 0)
            {
                int randomIndex = Random.Range(0, spawnPoints.Count);
                Transform spawnPoint = spawnPoints[randomIndex];

                GameObject enemyObject = ObjectPoolManager.Instance.GetObject(ObjectType.Enemy);
                enemyObject.transform.position = spawnPoint.position + Vector3.up * 0.3f;
                enemyObject.GetComponent<Enemy>().Setup(player);
            }
        }
    }
}