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

    void Awake()
    {
        layoutGroup = GetComponent<VerticalLayoutGroup>();
    }

    void OnEnable()
    {
        ExpandChildren();
    }

    public void ExpandChildren()
    {
        StartCoroutine(AnimateChildren(true));
    }

    IEnumerator AnimateChildren(bool isExpanded)
    {
        children.Clear();
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform child = transform.GetChild(i) as RectTransform;
            children.Add(child);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());

        List<Vector2> targetPositions = new();
        foreach (RectTransform child in children)
        {
            targetPositions.Add(child.anchoredPosition);
        }
            
        Vector2 topAnchor = new Vector2(targetPositions[0].x, 0);
        foreach (RectTransform child in children)
        { 
            child.anchoredPosition = topAnchor;
        }

        layoutGroup.enabled = false; 

        for (int i = 0; i < children.Count; i++)
        {
            StartCoroutine(SlideToPosition(children[i], targetPositions[i]));
            yield return new WaitForSeconds(delayBetween);
        }

        yield return new WaitForSeconds(slideDuration + delayBetween * children.Count);

        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    IEnumerator SlideToPosition(RectTransform rect, Vector2 target)
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
                transform.GetComponent<RectTransform>().sizeDelta = new Vector2(currentSize.x, Mathf.Abs(rect.anchoredPosition.y) + rect.sizeDelta.y);
                if (t >= slideDuration) layoutGroup.enabled = true;
            }
            yield return null;
        }

        rect.anchoredPosition = target;
    }
}
