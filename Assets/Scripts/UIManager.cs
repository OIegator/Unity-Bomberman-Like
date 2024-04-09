using System;
using System.Collections;
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
    [SerializeField] private CinemachineBrain cinemachineBrain;
    public CinemachineVirtualCameraBase menuCam;
    public CinemachineVirtualCameraBase gameCam;
    public GameObject uiPlayPanel;
    public GameObject uiStageSelector;
    public GameObject uiGameOverPanel;
    public GameObject uiStageCompletePanel;
    public GameObject uiPausePanel;
    public TextMeshProUGUI countdownText;
    public bool gameOverPanelActive;
    public bool stageCompletePanelActive;
    public CanvasGroup uiTransitionScreen;
    public CanvasGroup uiPauseScreen;
    public GameObject pauseButton;
    public GameObject exitButton;
    public Button startButton;
    public Bomberman player;

    private CinemachineVirtualCameraBase _currentActiveCamera;
    private bool _gameStarted;

    [Header("Stage Scroll")] [SerializeField]
    private int maxPage;

    private int _currentPage;
    private Vector3 _targetPagePos;
    private Tween _swipeTween;
    [SerializeField] private Vector3 pageStep;
    [SerializeField] private RectTransform stagePagesRect;

    [SerializeField] private float swipeTweenTime;
    [SerializeField] private Ease swipeTweenType;

    [Header("Stage Selector")] public GameObject buttonPrefab;
    public GameObject pageContainerPrefab;
    public RectTransform scrollContent;
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
                exitButton.SetActive(false);
                pauseButton.SetActive(true);
                break;
            case GameState.Paused:
                pauseButton.SetActive(false);
                uiPauseScreen.alpha = 0.75f;
                uiPausePanel.SetActive(true);
                break;
            case GameState.StageComplete:
                pauseButton.SetActive(false);
                if (stageCompletePanelActive) HideStageCompleteUIElements();
                FadeIn();
                break;
            case GameState.GameOver:
                pauseButton.SetActive(false);
                ShowGameOverUIElements();
                break;
            case GameState.NotStarted:
                break;
            case GameState.Restart:
                pauseButton.SetActive(false);
                uiPausePanel.SetActive(false);
                countdownText.text = "";
                uiPauseScreen.alpha = 0f;
                if (stageCompletePanelActive) HideStageCompleteUIElements();
                if (gameOverPanelActive) HideGameOverUIElements();
                FadeIn();
                break;
            case GameState.BackToMenu:
                StartCoroutine(DelayedBackToMenu());
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private void FadeIn(float pauseTime = 0.2f)
    {
        uiTransitionScreen.DOFade(1f, 0.5f).OnComplete(() => PauseBeforeFadeOut(pauseTime));
    }

    private void PauseBeforeFadeOut(float pauseTime)
    {
        DOTween.Sequence()
            .AppendInterval(pauseTime)
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
        SelectStage(0, 0);
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
        if (_currentActiveCamera == menuCam)
        {
            startButton.interactable = false;
            HidePlayUIElements();
            HideStageSelectorUIElements();
            StartCoroutine(player.StartGame());
            cinemachineBrain.m_DefaultBlend.m_Time = 2f;
            menuCam.Priority = 0;
            gameCam.Priority = 10;
            _currentActiveCamera = gameCam;
        }
        else
        {
            startButton.interactable = true;
            ShowPlayUIElements();
            ShowStageSelectorUIElements();
            cinemachineBrain.m_DefaultBlend.m_Time = 0.2f;
            menuCam.Priority = 10;
            gameCam.Priority = 0;
            _currentActiveCamera = menuCam;
        }
    }

    private IEnumerator DelayedBackToMenu()
    {
        exitButton.SetActive(true);
        uiPausePanel.SetActive(false);
        countdownText.text = "";
        uiPauseScreen.alpha = 0f;
        GameManager.Instance.menuTransition = true;
        FadeIn(0.5f);
        if (gameOverPanelActive) HideGameOverUIElements();
        yield return new WaitForSeconds(0.6f);
        SwitchCamera();
        ShowPlayUIElements();
        ShowStageSelectorUIElements();
    }

    public void Unpause()
    {
        StartCoroutine(UnpauseCountdown());
    }
    private IEnumerator UnpauseCountdown()
    {
        uiPausePanel.SetActive(false);
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();
            yield return new WaitForSeconds(1f);
        }
        
        countdownText.text = "";
        uiPauseScreen.alpha = 0f;
        GameManager.Instance.ResumeGame();
    }

    private void HidePlayUIElements()
    {
        RectTransform uiPlayRectTransform = uiPlayPanel.GetComponent<RectTransform>();

        uiPlayRectTransform.DOAnchorPosX(-uiPlayRectTransform.rect.width, 1f)
            .SetEase(Ease.InOutQuint)
            .OnComplete(() => uiPlayPanel.SetActive(false));
    }


    private void HideStageSelectorUIElements()
    {
        RectTransform uiStageSelectorRectTransform = uiStageSelector.GetComponent<RectTransform>();
        uiStageSelectorRectTransform.DOAnchorPosY(-uiStageSelectorRectTransform.rect.height, 1f)
            .SetEase(Ease.InOutQuint)
            .OnComplete(() => uiStageSelector.SetActive(false));
    }

    private void ShowStageSelectorUIElements()
    {
        uiStageSelector.SetActive(true);
        uiStageSelector.GetComponent<RectTransform>().DOAnchorPosY(0, 1f).SetEase(Ease.InOutQuint);
    }

    private void ShowPlayUIElements()
    {
        uiPlayPanel.SetActive(true);
        uiPlayPanel.GetComponent<RectTransform>().DOAnchorPosX(0, 1f).SetEase(Ease.InOutQuint);
    }

    private void HideGameOverUIElements()
    {
        gameOverPanelActive = false;
        RectTransform uiGameOverPanelRectTransform = uiGameOverPanel.GetComponent<RectTransform>();
        uiGameOverPanelRectTransform.DOAnchorPosY(-uiGameOverPanelRectTransform.rect.height - 30f, 0.5f)
            .SetEase(Ease.InOutQuint)
            .OnComplete(() => uiGameOverPanel.SetActive(false));
    }
    
    private void HideStageCompleteUIElements()
    {
        stageCompletePanelActive = false;
        RectTransform uiStageCompletePanelRectTransform = uiStageCompletePanel.GetComponent<RectTransform>();
        uiStageCompletePanelRectTransform.DOAnchorPosY(-uiStageCompletePanelRectTransform.rect.height - 30f, 0.5f)
            .SetEase(Ease.InOutQuint)
            .OnComplete(() => uiGameOverPanel.SetActive(false));
    }

    private void ShowGameOverUIElements()
    {
        uiGameOverPanel.SetActive(true);
        gameOverPanelActive = true;
        uiGameOverPanel.GetComponent<RectTransform>()
            .DOAnchorPosY(uiGameOverPanel.GetComponent<RectTransform>().rect.height + 30f, 0.5f)
            .SetEase(Ease.InOutElastic);
    }

    public void ShowStageCompleteUIElements()
    {
        uiStageCompletePanel.SetActive(true);
        stageCompletePanelActive = true;
        uiStageCompletePanel.GetComponent<RectTransform>()
            .DOAnchorPosY(uiStageCompletePanel.GetComponent<RectTransform>().rect.height + 30f, 0.5f)
            .SetEase(Ease.InOutElastic);
    }
    public void NextPage()
    {
        if (_currentPage < maxPage)
        {
            _currentPage++;
            _targetPagePos += pageStep;
            MovePage();
        }
        else
        {
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
        else
        {
            MovePage();
        }
    }

    public void MovePage()
    {
        _swipeTween?.Restart();
        _swipeTween = stagePagesRect.DOLocalMove(_targetPagePos, swipeTweenTime).SetEase(swipeTweenType);
    }
    
    public void ResetPage(int page)
    {
        _swipeTween?.Restart();
        _swipeTween = stagePagesRect.DOLocalMove(page * pageStep, 0.1f);
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

    public void SelectStage(int stageIndex, int pageIndex)
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
            buttonText.rectTransform.offsetMin = new Vector2(0f, 0f);
            ;
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