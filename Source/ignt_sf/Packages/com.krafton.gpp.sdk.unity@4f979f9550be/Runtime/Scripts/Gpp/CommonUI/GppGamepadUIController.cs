using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

public class GppGamepadUIController : MonoBehaviour
{
    public Selectable firstSelected;
    public Button submitButton;
    public Button cancelButton;

    public UnityEvent onCancel;

    public List<Selectable> selectables = new List<Selectable>();
    public ScrollRect currentScrollRect;
    public RectTransform lastSelectedRect;
    public GameObject prevSelectedObject;

    private Coroutine setupCoroutine;

    private void OnEnable()
    {
        TriggerSetup();
    }
    private void OnTransformChildrenChanged()
    {
        TriggerSetup();
    }

    private void TriggerSetup()
    {
        if (setupCoroutine != null)
        {
            StopCoroutine(setupCoroutine);
        }
        if (gameObject.activeInHierarchy)
        {
            setupCoroutine = StartCoroutine(SetupNavigationAfterLayout());
        }
    }

    private IEnumerator SetupNavigationAfterLayout()
    {
        yield return new WaitForEndOfFrame();
        SetupNavigation();
    }
    
    private void SetupNavigation()
    {
        selectables = GetComponentsInChildren<Selectable>(false)
            .Where(s => s.interactable && s.navigation.mode != Navigation.Mode.None && !(s is Scrollbar))
            .ToList();

        if (selectables.Count == 0) return;

        if (selectables.Count > 1)
        {
            for (int i = 0; i < selectables.Count; i++)
            {
                Selectable current = selectables[i];
                Navigation nav = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = FindClosestSelectable(current, Vector3.up),
                    selectOnDown = FindClosestSelectable(current, Vector3.down),
                    selectOnLeft = FindClosestSelectable(current, Vector3.left),
                    selectOnRight = FindClosestSelectable(current, Vector3.right)
                };
                current.navigation = nav;
            }
        }
        
        var currentSelected = EventSystem.current.currentSelectedGameObject;
        bool needsNewSelection = currentSelected == null || !selectables.Contains(currentSelected.GetComponent<Selectable>());

        if (needsNewSelection)
        {
            Selectable initialSelection = null;
            if (firstSelected != null && firstSelected.interactable && firstSelected.gameObject.activeInHierarchy)
            {
                initialSelection = firstSelected;
            }
            else
            {
                initialSelection = selectables.FirstOrDefault(s => s.interactable);
            }

            if (initialSelection != null)
            {
                EventSystem.current.SetSelectedGameObject(initialSelection.gameObject);
            }
        }
    }
    
    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            cancelButton?.onClick.Invoke();
            onCancel?.Invoke();
        }
    }

    private Selectable FindClosestSelectable(Selectable origin, Vector3 direction)
    {
        RectTransform originRect = origin.transform as RectTransform;
        Vector3 originPos = originRect.position;
        Selectable bestTarget = null;
        float closestDistance = float.MaxValue;
        foreach (var target in selectables)
        {
            if (target == origin) continue;
            RectTransform targetRect = target.transform as RectTransform;
            Vector3 toTarget = targetRect.position - originPos;
            if (Vector3.Dot(toTarget.normalized, direction) < 0.3f) continue;
            float distance = (direction == Vector3.up || direction == Vector3.down) ?
                Mathf.Abs(toTarget.y) + Mathf.Abs(toTarget.x) * 2f :
                Mathf.Abs(toTarget.x) + Mathf.Abs(toTarget.y) * 2f;
            if (distance < closestDistance)
            {
                closestDistance = distance;
                bestTarget = target;
            }
        }
        return bestTarget;
    }
}
