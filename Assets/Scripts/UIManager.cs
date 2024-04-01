using System;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public CinemachineVirtualCameraBase menuCam;
    public CinemachineVirtualCameraBase gameCam;
    public RectTransform uiPlayPanel;
    public RectTransform uiStageSelector;
    public CanvasGroup uiTransitionScreen;
    public Button startButton;
    public Bomberman player;

    private CinemachineVirtualCameraBase _currentActiveCamera;
    private bool _gameStarted = false;

    [SerializeField] public GameObject nextButton;
    [SerializeField] public GameObject restartButton;
    
    [Header("Stage Scroll")] [SerializeField] private int maxPage;
    private int _currentPage;
    private Vector3 _targetPagePos;
    private Tween _swipeTween;
    [SerializeField] private Vector3 pageStep;
    [SerializeField] private RectTransform stagePagesRect;

    [SerializeField] private float swipeTweenTime;
    [SerializeField] private Ease swipeTweenType;
    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
        
        _currentPage = 1;
        _targetPagePos = stagePagesRect.localPosition;
    }

    private void HandleGameStateChanged(GameState state)
    {
        switch (state)
        {
            case GameState.Playing:
                // Resume game logic
                break;
            case GameState.Paused:
                // Pause game logic
                break;
            case GameState.StageComplete:
                nextButton.SetActive(false);
                FadeIn();
                break;
            case GameState.GameOver:
                restartButton.SetActive(true);
                break;
            case GameState.NotStarted:
                break;
            case GameState.Restart:
                restartButton.SetActive(false);
                FadeIn();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void FadeIn()
    {
        uiTransitionScreen.DOFade(1f, 0.5f).OnComplete(PauseBeforeFadeOut);
    }

    private void PauseBeforeFadeOut()
    {
        DOTween.Sequence()
            .AppendInterval(0.2f)
            .OnComplete(FadeOut);
    }

    private void FadeOut()
    {
        uiTransitionScreen.DOFade(0f, 0.5f);
    }

    void Start()
    {
        GameManager.Instance.OnGameStateChanged += HandleGameStateChanged;
        if (menuCam != null && gameCam != null)
        {
            menuCam.Priority = 10;
            gameCam.Priority = 0;
            _currentActiveCamera = menuCam;
        }
        else
        {
            Debug.LogError("Please assign both cameras in the inspector.");
        }
    }

    public void SwitchCamera()
    {
        if (!_gameStarted)
        {
            _gameStarted = true;
            startButton.interactable = false;
            if (_currentActiveCamera == menuCam)
            {
                HidePlayUIElements();
                HideStageSelectorUIElements();
                StartCoroutine(player.StartGame());
                menuCam.Priority = 0;
                gameCam.Priority = 10;
                _currentActiveCamera = gameCam;
            }
            else
            {
                ShowPlayUIElements();
                ShowStageSelectorUIElements();
                menuCam.Priority = 10;
                gameCam.Priority = 0;
                _currentActiveCamera = menuCam;
            }
        }
    }

    private void HidePlayUIElements()
    {
        uiPlayPanel.DOAnchorPosX(-uiPlayPanel.rect.width, 1f).SetEase(Ease.InOutQuint);
    }

    private void HideStageSelectorUIElements()
    {
        uiStageSelector.DOAnchorPosY(-uiPlayPanel.rect.height, 1f).SetEase(Ease.InOutQuint);
    }

    private void ShowStageSelectorUIElements()
    {
        uiStageSelector.DOAnchorPosY(uiPlayPanel.rect.height, 1f).SetEase(Ease.InOutQuint);
    }

    private void ShowPlayUIElements()
    {
        uiPlayPanel.DOAnchorPosX(0, 1f).SetEase(Ease.InOutQuint);
    }
    
    public void NextPage()
    {
        if (_currentPage < maxPage)
        {
            _currentPage++;
            _targetPagePos += pageStep;
            MovePage();
        }   
    }

    public void PreviousPage()
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            _targetPagePos -= pageStep;
            MovePage();
        }
    }

    private void MovePage()
    {
        _swipeTween?.Restart();
        _swipeTween = stagePagesRect.DOLocalMove(_targetPagePos, swipeTweenTime).SetEase(swipeTweenType);
    }
}