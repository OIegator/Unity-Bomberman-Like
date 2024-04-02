using System;
using UnityEngine;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public GameState currentState = GameState.NotStarted;

    public bool menuTransition = true;

    public int unlockedStage = 1;
    
    private const string SaveFilePath = "game_progress.json";

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
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        currentState = GameState.Playing;
        SaveProgress();
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
    
    private void SaveProgress()
    {
        GameProgressData data = new GameProgressData();
        data.unlockedStage = unlockedStage;

        string jsonData = JsonUtility.ToJson(data);
        File.WriteAllText(GetSaveFilePath(), jsonData);
    }

    private void LoadProgress()
    {
        string filePath = GetSaveFilePath();
        if (File.Exists(filePath))
        {
            string jsonData = File.ReadAllText(filePath);
            GameProgressData data = JsonUtility.FromJson<GameProgressData>(jsonData);
            unlockedStage = data.unlockedStage;
        }
        else
        {
            unlockedStage = 1;
        }
    }

    private string GetSaveFilePath()
    {
        return Path.Combine(Application.persistentDataPath, SaveFilePath);
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
    StageComplete,
    GameOver
}
