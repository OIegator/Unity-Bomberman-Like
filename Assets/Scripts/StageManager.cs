using System;
using UnityEngine;

[Serializable]
public class StagePage
{
    public Stage[] stages = new Stage[9];
}

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }
    [Header("Systems")] [SerializeField] private ObjectPoolManager objectPoolManager;
    [SerializeField] private EnemyFactory enemyFactory;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject tutorialCanvas1;
    [SerializeField] private GameObject tutorialCanvas2;
    [SerializeField] private GameObject tutorialCanvas3;
    [SerializeField] private GridGenerator gridGenerator;
    [SerializeField] private BoxCollider boundingVolume;

    [Header("Stages")] public StagePage[] stagePages = new StagePage[3];

    private const float ClearDelay = 0.1f;

    public int CurrentPageIndex { get; set; }

    public int CurrentStageIndex { get; set; }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }
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
                enemyFactory.StopFactory();
                break;
            case GameState.StageComplete:
                GoToNextStage();
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
            case GameState.BackToMenu:
                UIManager.Instance.SelectStage(CurrentStageIndex, CurrentPageIndex);
                UIManager.Instance.ResetPage(CurrentPageIndex);
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
                CurrentPageIndex--;
                GameManager.Instance.BackToMenu();
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
        if (GameManager.Instance.currentState != GameState.BackToMenu)
        {
            GameManager.Instance.ResumeGame();
        }
        else
        {
            GameManager.Instance.currentState = GameState.NotStarted;
        }
    }

    private System.Collections.IEnumerator DestroyStage()
    {
        ObjectPoolManager.Instance.DestroyAllObjects();
        enemyFactory.StopFactory();
        yield return new WaitForSeconds(ClearDelay);
    }

    private void NextStage()
    {
        if (CurrentPageIndex == 0 && CurrentStageIndex == 0)
        {
            tutorialCanvas1.SetActive(true);
        }
        else
        {
            tutorialCanvas1.SetActive(false);
        }
        
        if (CurrentPageIndex == 0 && CurrentStageIndex == 1)
        {
            tutorialCanvas2.SetActive(true);
        }
        else
        {
            tutorialCanvas2.SetActive(false);
        }
        
        if (CurrentPageIndex == 0 && CurrentStageIndex == 2)
        {
            tutorialCanvas3.SetActive(true);
        }
        else
        {
            tutorialCanvas3.SetActive(false);
        }
        
        boundingVolume.center = stagePages[CurrentPageIndex].stages[CurrentStageIndex].cameraConfinerCenter;
        boundingVolume.size = stagePages[CurrentPageIndex].stages[CurrentStageIndex].cameraConfinerSize;
        enemyFactory.Setup(player, stagePages[CurrentPageIndex].stages[CurrentStageIndex].maxEnemyCount);
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