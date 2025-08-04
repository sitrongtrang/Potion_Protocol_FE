using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillSlotUI : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private GameObject _cooldown;
    [SerializeField] private TextMeshProUGUI _cooldownText;
    private float _cooldownTime;
    private float _timeRemaining;

    public void Initialize(SkillConfig skill)
    {
        _icon.sprite = skill.SkillIcon;
        _cooldown.SetActive(false);
        _cooldownTime = skill.Cooldown;
        _cooldownText.text = _cooldownTime.ToString();
        _timeRemaining = _cooldownTime;
    }

    public void OnDeactivated()
    {
        StartCoroutine(Cooldown());
    }

    private IEnumerator Cooldown()
    {
        _cooldown.SetActive(true);
        while (_timeRemaining > 0)
        {
            _cooldownText.text = _timeRemaining.ToString();
            yield return new WaitForSeconds(1);
            _timeRemaining--;

        }
        _cooldown.SetActive(false);
        _timeRemaining = _cooldownTime;
    }
}