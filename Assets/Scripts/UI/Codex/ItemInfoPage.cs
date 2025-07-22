using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPage : InfoPage
{
    [SerializeField] private TextMeshProUGUI _craftText;
    [SerializeField] private Image _craftPlaceIcon;
    [SerializeField] private TextMeshProUGUI _droppedByText;
    [SerializeField] private Image _droppedByIcon;

    public override void DisplayInfo(EntityConfig entityConfig)
    {
        base.DisplayInfo(entityConfig);
        if (entityConfig is ItemConfig itemConfig)
        {
            if (itemConfig.DroppedBy != null)
            {
                _droppedByText.text = $"Dropped by:";
                _droppedByIcon.gameObject.SetActive(true);
                _droppedByIcon.sprite = itemConfig.DroppedBy.Icon;
            }
            else
            {
                _droppedByText.text = "Dropped by: N/A";
                _droppedByIcon.sprite = null;
                _droppedByIcon.gameObject.SetActive(false);
            }

            if (itemConfig is CraftableItemConfig craftableItem && craftableItem.CraftPlace != null)
            {
                _craftText.text = $"Crafted at:";
                _craftPlaceIcon.gameObject.SetActive(true);
                _craftPlaceIcon.sprite = craftableItem.CraftPlace;
            }
            else
            {
                _craftText.text = "Crafted at: N/A";
                _craftPlaceIcon.sprite = null;
                _craftPlaceIcon.gameObject.SetActive(false);
            }
        }
        else
        {
            _craftText.text = "Crafted at: N/A";
            _droppedByText.text = "Dropped by: N/A";
            _craftPlaceIcon.sprite = null;
            _craftPlaceIcon.gameObject.SetActive(false);
            _droppedByIcon.sprite = null;
            _droppedByIcon.gameObject.SetActive(false);
        }
    }
}