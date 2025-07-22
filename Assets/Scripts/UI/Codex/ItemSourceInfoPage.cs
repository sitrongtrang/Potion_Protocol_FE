using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemSourceInfoPage : InfoPage
{
    [SerializeField] private Image _dropItemIcon;
    [SerializeField] private TextMeshProUGUI _dropItemText;
    
    public override void DisplayInfo(EntityConfig entityConfig)
    {
        base.DisplayInfo(entityConfig);
        if (entityConfig is ItemSourceConfig itemSourceConfig && itemSourceConfig.DroppedItem != null)
        {
            _dropItemText.text = $"Drop:";
            _dropItemIcon.gameObject.SetActive(true);
            _dropItemIcon.sprite = itemSourceConfig.DroppedItem.Icon;
        }
        else
        {
            _dropItemText.text = $"Drop: N/A";
            _dropItemIcon.sprite = null;
            _dropItemIcon.gameObject.SetActive(false);
        }
    }
}