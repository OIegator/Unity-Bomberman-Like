using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    private GameObject _player;
    [SerializeField] private float spawnInterval = 5f;
    [SerializeField] private List<GameObject> spawnPoints = new();
    [SerializeField] private int maxEnemyCount = 2;
    [SerializeField] private List<TextMeshProUGUI> countdownText = new();
    private Coroutine _spawnCoroutine;
    private int _spawnedEnemyCount;

    public void Setup(GameObject player, int maxEnemy)
    {
        spawnPoints.Clear();
        _spawnedEnemyCount = 0;
        _player = player;
        maxEnemyCount = maxEnemy;
    }

    public void AddSpawnPoint(GameObject spawnPoint)
    {
        spawnPoints.Add(spawnPoint);
    }
    
    public void AddTextMesh(TextMeshProUGUI textMesh)
    {
        countdownText.Add(textMesh);
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
            yield return new WaitUntil(() => _spawnedEnemyCount < maxEnemyCount);
            foreach (var mesh in countdownText)
            {
                mesh.text = "5";
            }
            float timeLeft = spawnInterval;
            while (timeLeft > 0f)
            {
                yield return new WaitForSeconds(1f);
                timeLeft -= 1f;
                foreach (var mesh in countdownText)
                {
                    mesh.text = timeLeft == 0 ? "" : timeLeft.ToString(CultureInfo.InvariantCulture);
                }
            }

            if (spawnPoints.Count > 0 && _spawnedEnemyCount < maxEnemyCount)
            {
                int randomIndex = Random.Range(0, spawnPoints.Count);
                GameObject spawnPoint = spawnPoints[randomIndex];

                GameObject enemyObject = ObjectPoolManager.Instance.GetObject(ObjectType.Enemy);
                enemyObject.GetComponent<Enemy>().Setup(_player, spawnPoint);
                _spawnedEnemyCount++;
            }
        }
    }

}