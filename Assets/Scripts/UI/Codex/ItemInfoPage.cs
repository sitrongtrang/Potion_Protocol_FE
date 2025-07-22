using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemInfoPage : InfoPage
{
    [SerializeField] private TextMeshProUGUI _itemDescriptionText;

    public override void DisplayInfo(EntityConfig entityConfig)
    {
        base.DisplayInfo(entityConfig);
        if (entityConfig is ItemConfig itemConfig)
        {
            
        }
        else
        {
            
        }
    }
}