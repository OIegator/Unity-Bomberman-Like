using DG.Tweening;
using UnityEngine;

public class Swipe : MonoBehaviour
{

    [SerializeField] private int maxPage;
    private int _currentPage;
    private Vector3 _targetPos;
    private Tween _tween;
    [SerializeField] private Vector3 pageStep;
    [SerializeField] private RectTransform stagePagesRect;

    [SerializeField] private float tweenTime;
    [SerializeField] private Ease tweenType;

    private void Awake()
    {
        _currentPage = 1;
        _targetPos = stagePagesRect.localPosition;
    }

    public void Next()
    {
        if (_currentPage < maxPage)
        {
            _currentPage++;
            _targetPos += pageStep;
            MovePage();
        }   
    }

    public void Previous()
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            _targetPos -= pageStep;
            MovePage();
        }
    }

    private void MovePage()
    {
        _tween?.Restart();
        _tween = stagePagesRect.DOLocalMove(_targetPos, tweenTime).SetEase(tweenType);
    }
}
