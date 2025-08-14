using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSourceInfoPage : InfoPage
{
    [SerializeField] private Image _dropItemIcon;
    [SerializeField] private TextMeshProUGUI _dropItemText;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    
    public override void DisplayInfo(EntityConfig entityConfig)
    {
        base.DisplayInfo(entityConfig);
        if (entityConfig is ItemSourceConfig itemSourceConfig && itemSourceConfig.DroppedItem != null)
        {
            if (!string.IsNullOrEmpty(itemSourceConfig.Description))
                _descriptionText.text = itemSourceConfig.Description;
            else
                _descriptionText.text = "...";
            _dropItemText.text = $"Drop:";
            _dropItemIcon.gameObject.SetActive(true);
            _dropItemIcon.sprite = itemSourceConfig.DroppedItem.Icon;
        }
        else
        {
            _dropItemText.text = $"Drop: ???";
            _dropItemIcon.sprite = null;
            _dropItemIcon.gameObject.SetActive(false);
            _descriptionText.text = "...";
        }
    }
}