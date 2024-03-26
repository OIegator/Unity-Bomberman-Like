using System.Collections;
using System.Threading;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("Systems")] [SerializeField] private ObjectPoolManager objectPoolManager;
    [SerializeField] private EnemyFactory enemyFactory;
    [SerializeField] private Bomberman player;
    [SerializeField] private GridGenerator gridGenerator;

    [Header("Stages")] [SerializeField] private Stage[] stages;

    private int _currentStageId = 0;

    private const float ClearDelay = 3f;

    private void Start()
    {
        SetupSystem();
        StartCoroutine(StartNextLevel());
    }

    private void SetupSystem()
    {
        objectPoolManager.InitializePool();
    }

    private IEnumerator StartNextLevel()
    {
        player.gameObject.SetActive(false);
        enemyFactory.StopFactory();
        yield return new WaitForSeconds(ClearDelay);
        NextLevel();
        StartCoroutine(CoroutineStartGame());
    }

    public void NextLevel()
    {
        _currentStageId = 1;
        enemyFactory.Setup(player);
        gridGenerator.Setup(stages[_currentStageId - 1], enemyFactory);
    }

    private IEnumerator CoroutineStartGame()
    {
        yield return new WaitForSeconds(0.01f);
        SetupPlayer();
        enemyFactory.StartFactory();
    }

    private void SetupPlayer()
    {
        var spawnPoint = gridGenerator.GetPlayerSpawnPoint();
        player.transform.position = new Vector3(spawnPoint.transform.position.x,
            spawnPoint.transform.position.y + 0.5f, spawnPoint.transform.position.z);
        player.gameObject.SetActive(true);
    }
}