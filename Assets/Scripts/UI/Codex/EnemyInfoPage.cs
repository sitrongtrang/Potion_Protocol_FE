using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyInfoPage : InfoPage
{
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private TextMeshProUGUI _speedText;
    [SerializeField] private TextMeshProUGUI _dropItemText;
    [SerializeField] private Image _dropItemIcon;
    
    public override void DisplayInfo(EntityConfig entityConfig)
    {
        base.DisplayInfo(entityConfig);
        if (entityConfig is EnemyConfig enemyConfig)
        {
            _hpText.text = $"HP: {enemyConfig.Hp}";
            _speedText.text = $"Speed: {enemyConfig.Speed}";
            if (enemyConfig.Item != null)
            {
                _dropItemText.text = $"Drop:";
                _dropItemIcon.gameObject.SetActive(true);
                _dropItemIcon.sprite = enemyConfig.Item.Icon;
            }
            else
            {   
                _dropItemText.text = $"Drop: N/A";
                _dropItemIcon.sprite = null;
                _dropItemIcon.gameObject.SetActive(false);
            }
        }
        else
        {
            _hpText.text = "HP: N/A";
            _speedText.text = "Speed: N/A";
            _dropItemText.text = $"Drop: N/A";
            _dropItemIcon.sprite = null;
            _dropItemIcon.gameObject.SetActive(false);
        }
    }
}