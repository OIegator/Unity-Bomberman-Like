using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState currentState = GameState.NotStarted;

    public bool menuTransition = true;

    public event Action<GameState> OnGameStateChanged;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public void PauseGame()
    {
        currentState = GameState.Paused;
        OnGameStateChanged?.Invoke(currentState);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        currentState = GameState.Playing;
        OnGameStateChanged?.Invoke(currentState);
        Time.timeScale = 1;
    }

    public void EndStage()
    {
        UIManager.Instance.nextButton.SetActive(true);
        currentState = GameState.NotStarted;
        OnGameStateChanged?.Invoke(currentState);
    }
    
    public void Restart()
    {
        UIManager.Instance.restartButton.SetActive(true);
        currentState = GameState.Restart;
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
}

public enum GameState
{
    NotStarted,
    Playing,
    Paused,
    Restart, 
    StageComplete,
    GameOver
}
