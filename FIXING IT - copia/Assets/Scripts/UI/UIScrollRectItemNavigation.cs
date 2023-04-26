using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIScrollRectItemNavigation : MonoBehaviour, ISelectHandler
{
    private ScrollRect _scrollRect;
    private float _scrollPosition = 1;

    private void Start()
    {
        _scrollRect = GetComponentInParent<ScrollRect>(true);
    }

    public void OnSelect(BaseEventData eventData)
    {
        int childCount = _scrollRect.content.transform.childCount - 1;
        int childIndex = transform.GetSiblingIndex();

        childIndex = childIndex < (float)childCount / 2f ? childIndex - 1 : childIndex;

        _scrollPosition = 1 - (float)childIndex / childCount;

        if (_scrollRect)
            _scrollRect.verticalScrollbar.value = _scrollPosition;
    }
}
