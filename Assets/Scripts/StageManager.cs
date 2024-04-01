using System;
using System.Collections;
using UnityEngine;

public class StageManager : MonoBehaviour
{
    [Header("Systems")] [SerializeField] private ObjectPoolManager objectPoolManager;
    [SerializeField] private EnemyFactory enemyFactory;
    [SerializeField] private GameObject player;
    [SerializeField] private GridGenerator gridGenerator;
    [SerializeField] private BoxCollider boundingVolume;

    [Header("Stages")] [SerializeField] private Stage[] stages;

    private int _currentStageId = 1;

    private const float ClearDelay = 0.1f;

    private void Start()
    {
        SetupSystem();
        NextLevel();
        SetupPlayer();
    }

    private void HandleGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                enemyFactory.StartFactory();
                break;
            case GameState.Paused:
                // Pause game logic
                break;
            case GameState.StageComplete:
                _currentStageId++;
                StartCoroutine(OnStageComplete());
                break;
            case GameState.GameOver:
                enemyFactory.StopFactory();
                break;
            case GameState.NotStarted:
                break;
            case GameState.Restart:
                StartCoroutine(OnStageComplete());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void SetupSystem()
    {
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        objectPoolManager.InitializePool();
    }

    private IEnumerator OnStageComplete()
    {
        yield return new WaitForSeconds(0.6f);
        StartCoroutine(DestroyStage());
        NextLevel();
        SetupPlayer();
        GameManager.Instance.ResumeGame();
    }

    private IEnumerator DestroyStage()
    {
        ObjectPoolManager.Instance.DestroyAllObjects();
        enemyFactory.StopFactory();
        yield return new WaitForSeconds(ClearDelay);
    }

    private void NextLevel()
    {
        boundingVolume.center = stages[_currentStageId - 1].cameraConfinerCenter;
        boundingVolume.size = stages[_currentStageId - 1].cameraConfinerSize;
        enemyFactory.Setup(player);
        gridGenerator.Setup(stages[_currentStageId - 1], enemyFactory);
    }


    private void SetupPlayer()
    {
        player.GetComponent<Bomberman>().Setup(gridGenerator.GetPlayerSpawnPoint());
    }
}