using UnityEngine;
using Cinemachine;
using UnityEngine.UI;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public CinemachineVirtualCameraBase menuCam;
    public CinemachineVirtualCameraBase gameCam;
    public RectTransform[] uiElementsToHide; 
    public Button startButton;
    public Bomberman player;

    private CinemachineVirtualCameraBase currentActiveCamera;
    private bool _gameStarted = false;

    void Start()
    {
        if (menuCam != null && gameCam != null)
        {
            menuCam.Priority = 10;
            gameCam.Priority = 0;
            currentActiveCamera = menuCam;
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
            if (currentActiveCamera == menuCam)
            {
                HideUIElements();
                StartCoroutine(player.StartGame());
                menuCam.Priority = 0;
                gameCam.Priority = 10;
                currentActiveCamera = gameCam;
            }
            else
            {
                ShowUIElements();
                menuCam.Priority = 10;
                gameCam.Priority = 0;
                currentActiveCamera = menuCam;
            }
        }
    }

    void HideUIElements()
    {
        foreach (RectTransform element in uiElementsToHide)
        {
            element.DOAnchorPosX(-element.rect.width, 1f).SetEase(Ease.InOutQuint);
        }
    }

    void ShowUIElements()
    {
        foreach (RectTransform element in uiElementsToHide)
        {
            element.DOAnchorPosX(0, 1f).SetEase(Ease.InOutQuint);
        }
    }
}