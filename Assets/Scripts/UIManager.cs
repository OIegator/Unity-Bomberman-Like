using System;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [SerializeField] private StageManager stageManager;
    public CinemachineVirtualCameraBase menuCam;
    public CinemachineVirtualCameraBase gameCam;
    public RectTransform uiPlayPanel;
    public RectTransform uiStageSelector;
    public CanvasGroup uiTransitionScreen;
    public Button startButton;
    public Bomberman player;

    private CinemachineVirtualCameraBase _currentActiveCamera;
    private bool _gameStarted;

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
    
    [Header("Stage Selector")] 
    public GameObject buttonPrefab; // Prefab for the stage button
    public GameObject pageContainerPrefab; // Prefab for the page container
    public RectTransform scrollContent; // Content of the scroll rect
    public Color selectedColor; // Color for selected button
    public Color defaultColor; // Default color for buttons
    public Sprite selectedSprite;
    public Sprite defaultSprite;
    private Button _selectedButtonComponent;

    private readonly List<List<Button>> _stageButtons = new();
    private Button _selectedButton;
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
        GenerateStageButtons();
        SelectStage(0,0);
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
    
    void GenerateStageButtons()
    {
        StageManager stageManager = FindObjectOfType<StageManager>(); // Find the StageManager instance
        if (stageManager == null)
        {
            Debug.LogError("StageManager not found in the scene!");
            return;
        }

        int totalPages = stageManager.stagePages.Length;
        int stageIndex = 1;

        for (int page = 0; page < totalPages; page++)
        {
            GameObject pageContainerGo = Instantiate(pageContainerPrefab, scrollContent);
            RectTransform pageContainerRect = pageContainerGo.GetComponent<RectTransform>();

            List<Button> pageButtonList = new List<Button>();

            for (int i = 0; i < stageManager.stagePages[page].stages.Length; i++)
            {
                GameObject buttonGo = Instantiate(buttonPrefab, pageContainerRect);
                Button button = buttonGo.GetComponent<Button>();
                TextMeshProUGUI buttonText = buttonGo.GetComponentInChildren<TextMeshProUGUI>();
                
                // Set stage Id
                stageManager.stagePages[page].stages[i].id = stageIndex;

                // Set button text to stage Id
                buttonText.text = "" + (stageIndex);

                // Set button interactable based on unlocked stages
                button.interactable = CheckIfStageUnlocked(stageIndex);
                if (!CheckIfStageUnlocked(stageIndex))
                {
                    buttonText.color = Color.gray;
                }

                // Add listener for button click
                int stageID = i;
                int pageID = page;
                button.onClick.AddListener(() => SelectStage(stageID, pageID));
                stageIndex++;

                pageButtonList.Add(button);
            }

            _stageButtons.Add(pageButtonList);
        }
    }

    bool CheckIfStageUnlocked(int stageIndex)
    {
        return stageIndex <= GameManager.Instance.unlockedStage;
    }

    void SelectStage(int stageIndex, int pageIndex)
    {
        // Reset the previously selected button
        if (_selectedButton != null)
        {
            // Revert width and height to default values using DoTween
            RectTransform selectedButtonRect = _selectedButton.GetComponent<RectTransform>();
            selectedButtonRect.DOSizeDelta(new Vector2(160f, 255f), 0.3f);

            // Optionally revert sprite to default
            _selectedButtonComponent.image.sprite = defaultSprite;

            // Revert color to default
            //_selectedButtonComponent.image.color = defaultColor;

            // Revert text bottom padding
            TextMeshProUGUI buttonText = _selectedButton.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.color = Color.white;
            buttonText.rectTransform.offsetMin = new Vector2(0f, 0f);;
        }

        // Set the new selected button
        _selectedButton = _stageButtons[pageIndex][stageIndex];
        _selectedButtonComponent = _selectedButton.GetComponent<Button>();

        // Change width and height using DoTween
        RectTransform newSelectedButtonRect = _selectedButton.GetComponent<RectTransform>();
        newSelectedButtonRect.DOSizeDelta(new Vector2(190f, 270f), 0.3f);

        // Change sprite
        _selectedButtonComponent.image.sprite = selectedSprite;

        // Change text bottom padding
        TextMeshProUGUI newButtonText = _selectedButton.GetComponentInChildren<TextMeshProUGUI>();
        newButtonText.color = Color.white;
        newButtonText.rectTransform.offsetMin = new Vector2(0f, 50f);

        // Change color
        //_selectedButtonComponent.image.color = selectedColor;

        // Load the selected stage
        stageManager.LoadStage(pageIndex, stageIndex);
    }

}