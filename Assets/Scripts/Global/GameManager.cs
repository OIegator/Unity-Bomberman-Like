using System;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState currentState = GameState.NotStarted;

    public bool menuTransition = true;

    public int unlockedStage = 1;

    private const string UnlockedStageKey = "UnlockedStage";

    public event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        LoadProgress();
    }

    public void PauseGame()
    {
        currentState = GameState.Paused;
        OnGameStateChanged?.Invoke(currentState);
    }

    public void ResumeGame()
    {
        currentState = GameState.Playing;
        SaveProgress();
        OnGameStateChanged?.Invoke(currentState);
    }

    public void EndStage()
    {
        var currentStageId = StageManager.Instance.stagePages[StageManager.Instance.CurrentPageIndex]
            .stages[StageManager.Instance.CurrentStageIndex].id;
        if (currentStageId == unlockedStage)
        {
            unlockedStage++;
        }
        else
        {
            unlockedStage = Math.Max(unlockedStage, currentStageId);
        }

        UIManager.Instance.ShowStageCompleteUIElements();
        currentState = GameState.NotStarted;
        OnGameStateChanged?.Invoke(currentState);
    }

    public void Restart()
    {
        currentState = GameState.Restart;
        OnGameStateChanged?.Invoke(currentState);
    }

    public void BackToMenu()
    {
        currentState = GameState.BackToMenu;
        OnGameStateChanged?.Invoke(currentState);
    }


    public void GameOver()
    {
        currentState = GameState.GameOver;
        OnGameStateChanged?.Invoke(currentState);
    }

    public void StartNextStage()
    {
        currentState = GameState.StageComplete;
        OnGameStateChanged?.Invoke(currentState);
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetInt(UnlockedStageKey, unlockedStage);
        PlayerPrefs.Save();
    }

    private void LoadProgress()
    {
        unlockedStage = PlayerPrefs.GetInt(UnlockedStageKey, 1);
    }

    public void ResetProgress()
    {
        PlayerPrefs.DeleteKey(UnlockedStageKey);
        PlayerPrefs.Save();
        unlockedStage = 1;
    }

    public void ExitGame()
    {
        PlayerPrefs.Save();
        Application.Quit();
    }
}

[Serializable]
public class GameProgressData
{
    public int unlockedStage;
}

public enum GameState
{
    NotStarted,
    Playing,
    Paused,
    Restart,
    BackToMenu,
    StageComplete,
    GameOver
}