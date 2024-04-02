using System;
using UnityEngine;

[Serializable]
public class StagePage
{
    public Stage[] stages = new Stage[9];
}

public class StageManager : MonoBehaviour
{
    [Header("Systems")] [SerializeField] private ObjectPoolManager objectPoolManager;
    [SerializeField] private EnemyFactory enemyFactory;
    [SerializeField] private GameObject player;
    [SerializeField] private GridGenerator gridGenerator;
    [SerializeField] private BoxCollider boundingVolume;

    [Header("Stages")] public StagePage[] stagePages = new StagePage[3];

    private const float ClearDelay = 0.1f;

    private int CurrentPageIndex { get; set; }

    private int CurrentStageIndex { get; set; }

    private void Start()
    {
        SetupSystem();
        NextStage();
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
                GoToNextStage();
                GameManager.Instance.unlockedStage = Mathf.Max(GameManager.Instance.unlockedStage,
                    stagePages[CurrentPageIndex].stages[CurrentStageIndex].id);
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

    private void GoToNextStage()
    {
        CurrentStageIndex++;
        if (CurrentStageIndex >= stagePages[CurrentPageIndex].stages.Length)
        {
            CurrentStageIndex = 0;
            CurrentPageIndex++;
            if (CurrentPageIndex >= stagePages.Length)
            {
                CurrentPageIndex = 0;
            }
        }
    }

    private void SetupSystem()
    {
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        objectPoolManager.InitializePool();
    }

    private System.Collections.IEnumerator OnStageComplete()
    {
        yield return new WaitForSeconds(0.6f);
        StartCoroutine(DestroyStage());
        NextStage();
        SetupPlayer();
        GameManager.Instance.ResumeGame();
    }

    private System.Collections.IEnumerator DestroyStage()
    {
        ObjectPoolManager.Instance.DestroyAllObjects();
        enemyFactory.StopFactory();
        yield return new WaitForSeconds(ClearDelay);
    }

    private void NextStage()
    {
        boundingVolume.center = stagePages[CurrentPageIndex].stages[CurrentStageIndex].cameraConfinerCenter;
        boundingVolume.size = stagePages[CurrentPageIndex].stages[CurrentStageIndex].cameraConfinerSize;
        enemyFactory.Setup(player);
        gridGenerator.Setup(stagePages[CurrentPageIndex].stages[CurrentStageIndex], enemyFactory);
    }

    private void SetupPlayer()
    {
        player.GetComponent<Bomberman>().Setup(gridGenerator.GetPlayerSpawnPoint());
    }

    public void LoadStage(int pageIndex, int stageIndex)
    {
        if (pageIndex >= 0 && pageIndex < stagePages.Length && stageIndex >= 0 &&
            stageIndex < stagePages[pageIndex].stages.Length)
        {
            if (pageIndex == CurrentPageIndex && stageIndex == CurrentStageIndex)
                return;

            CurrentPageIndex = pageIndex;
            CurrentStageIndex = stageIndex;

            // Load the stage
            StartCoroutine(DestroyStage());
            NextStage();
            SetupPlayer();
        }
        else
        {
            Debug.LogError("Invalid page or stage index.");
        }
    }
}