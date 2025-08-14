using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoPage : InfoPage
{
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _speedText;
    [SerializeField] private TextMeshProUGUI _dropItemText;
    [SerializeField] private Image _dropItemIcon;
    [SerializeField] private TextMeshProUGUI _descriptionText;
    
    public override void DisplayInfo(EntityConfig entityConfig)
    {
        base.DisplayInfo(entityConfig);
        if (entityConfig is EnemyConfig enemyConfig)
        {
            _hpText.text = $"HP: {enemyConfig.Hp}";
            _speedText.text = $"Speed: {enemyConfig.Speed}";
            if (!string.IsNullOrEmpty(enemyConfig.Description))
                _descriptionText.text = enemyConfig.Description;
            else
                _descriptionText.text = "...";
            if (enemyConfig.Item != null)
            {
                _dropItemText.text = $"Drop:";
                _dropItemIcon.gameObject.SetActive(true);
                _dropItemIcon.sprite = enemyConfig.Item.Icon;
            }
            else
            {
                _dropItemText.text = $"Drop: ???";
                _dropItemIcon.sprite = null;
                _dropItemIcon.gameObject.SetActive(false);
            }
        }
        else
        {
            _hpText.text = "HP: ???";
            _speedText.text = "Speed: ???";
            _dropItemText.text = $"Drop: ???";
            _dropItemIcon.sprite = null;
            _dropItemIcon.gameObject.SetActive(false);
            _descriptionText.text = "...";
        }
    }
}