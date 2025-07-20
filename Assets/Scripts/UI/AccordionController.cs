using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class AccordionController : MonoBehaviour
{
    [System.Serializable]
    public class AccordionSection
    {
        public GameObject button;         
        public ExpandableContainer childContainer; 
    }

    public List<AccordionSection> sections;
    private LayoutElement layoutElement;
    private int expandedIndex = -1;

    void Start()
    {
        layoutElement = GetComponent<LayoutElement>();
    }

    public void ToggleSection(int index)
    {
        if (expandedIndex == index)
        {
            SetSectionActive(index, false);
            expandedIndex = -1;
        }
        else
        {
            if (expandedIndex != -1)
                SetSectionActive(expandedIndex, false);

            SetSectionActive(index, true);
            expandedIndex = index;
        }
    }

    private void SetSectionActive(int index, bool active)
    {
        if (index < 0 || index >= sections.Count) return;

        sections[index].childContainer.gameObject.SetActive(active);

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
    }
}
