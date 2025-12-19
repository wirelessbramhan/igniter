using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class GppGamepadUIScroller : MonoBehaviour
{
    private ScrollRect currentScrollRect;
    private RectTransform lastSelectedRect;

    void LateUpdate()
    {
        GameObject currentSelected = EventSystem.current.currentSelectedGameObject;
        if (currentSelected == null || !currentSelected.transform.IsChildOf(transform)) return;

        lastSelectedRect = currentSelected.GetComponent<RectTransform>();
        currentScrollRect = lastSelectedRect.GetComponentInParent<ScrollRect>();

        if (currentScrollRect != null)
        {
            ScrollToSelected();
        }
    }

    private void ScrollToSelected()
    {
        RectTransform content = currentScrollRect.content;
        RectTransform viewport = currentScrollRect.viewport;
        Vector3[] viewportCorners = new Vector3[4];
        viewport.GetWorldCorners(viewportCorners);
        float viewportTop = viewportCorners[1].y;
        float viewportBottom = viewportCorners[0].y;
        Vector3[] selectedCorners = new Vector3[4];
        lastSelectedRect.GetWorldCorners(selectedCorners);
        float selectedTop = selectedCorners[1].y;
        float selectedBottom = selectedCorners[0].y;
        float scrollDelta = 0f;
        if (selectedTop > viewportTop) scrollDelta = selectedTop - viewportTop;
        else if (selectedBottom < viewportBottom) scrollDelta = selectedBottom - viewportBottom;
        if (Mathf.Abs(scrollDelta) > 0.01f)
        {
            Vector2 newAnchoredPos = content.anchoredPosition;
            newAnchoredPos.y -= scrollDelta;
            content.anchoredPosition = newAnchoredPos;
        }
    }
}
