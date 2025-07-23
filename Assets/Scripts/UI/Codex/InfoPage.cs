using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class InfoPage : MonoBehaviour
{
    [SerializeField] protected Image _iconImage;
    [SerializeField] protected TextMeshProUGUI _nameText;
    // [SerializeField] protected Text _descriptionText;

    public virtual void DisplayInfo(EntityConfig entityConfig)
    {
        _iconImage.sprite = entityConfig.Icon;
        _nameText.text = entityConfig.Name;
    }
}