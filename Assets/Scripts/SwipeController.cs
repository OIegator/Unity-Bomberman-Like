using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeController : MonoBehaviour, IEndDragHandler
{
    private float _dragThreshold;

    private void Awake()
    {
        _dragThreshold = Screen.width / 15f;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (Mathf.Abs(eventData.position.x - eventData.pressPosition.x) > _dragThreshold)
        {
            if (eventData.position.x > eventData.pressPosition.x) UIManager.Instance.PreviousPage();
            else UIManager.Instance.NextPage();
        }
        else
        {
            UIManager.Instance.MovePage();
        }
    }
}