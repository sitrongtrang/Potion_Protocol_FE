using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(VerticalLayoutGroup))]
public class ExpandableContainer : MonoBehaviour
{
    public float slideDuration = 0.3f;
    public float delayBetween = 0.03f;

    private VerticalLayoutGroup layoutGroup;
    private List<RectTransform> children = new();
    private List<Vector2> expandedPositions = new();
    private List<Vector2> collapsedPositions = new();

    void Awake()
    {
        layoutGroup = GetComponent<VerticalLayoutGroup>();
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform child = transform.GetChild(i) as RectTransform;
            children.Add(child);
            expandedPositions.Add(child.anchoredPosition);
            collapsedPositions.Add(new Vector2(child.anchoredPosition.x, 0));
        }
    }

    public void ToggleExpanded(bool isExpanded, Button button = null)
    {
        StartCoroutine(AnimateChildren(isExpanded));
    }

    private IEnumerator AnimateChildren(bool isExpanded, Button button = null)
    {
        List<Vector2> targetPositions = new();
        for (int i = 0; i < children.Count; i++)
        {
            targetPositions.Add(isExpanded ? expandedPositions[i] : collapsedPositions[i]);
            children[i].anchoredPosition = isExpanded ? collapsedPositions[i] : expandedPositions[i];
        }

        if (button) button.interactable = false;
        layoutGroup.enabled = false;
        for (int i = children.Count - 1; i >= 0; i--)
        {
            StartCoroutine(SlideToPosition(children[i], targetPositions[i], isExpanded));
            yield return new WaitForSeconds(delayBetween);
        }

        yield return new WaitForSeconds(slideDuration + delayBetween * children.Count);
        if (button) button.interactable = true;

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    IEnumerator SlideToPosition(RectTransform rect, Vector2 target, bool isExpanded = true)
    {
        Vector2 start = rect.anchoredPosition;
        float t = 0f;

        while (t < slideDuration)
        {
            t += Time.unscaledDeltaTime;
            float progress = Mathf.Clamp01(t / slideDuration);
            rect.anchoredPosition = Vector2.Lerp(start, target, progress);
            if (rect == transform.GetChild(transform.childCount - 1) as RectTransform)
            {
                Vector2 currentSize = transform.GetComponent<RectTransform>().sizeDelta;
                float height = Mathf.Abs(rect.anchoredPosition.y) + rect.sizeDelta.y;
                transform.GetComponent<RectTransform>().sizeDelta = new Vector2(currentSize.x, height);

                if (t >= slideDuration) layoutGroup.enabled = true;
                
                if (!isExpanded && t >= slideDuration) gameObject.SetActive(false);
            }
            yield return null;
        }

        rect.anchoredPosition = target;
    }
}
