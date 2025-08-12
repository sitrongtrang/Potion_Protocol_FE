using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillContainerUI : MonoBehaviour
{
    [SerializeField] private GameObject _skillUIPrefab;
    private PlayerAttack _playerAttack;
    private List<SkillSlotUI> skillSlots = new();

    public void Initialize(PlayerController player)
    {
        _playerAttack = player.Attack;

        for (int i = 0; i < GameConstants.NumSkills; i++)
        {
            GameObject skillUI = Instantiate(_skillUIPrefab, transform);
            skillUI.transform.SetParent(transform);
            SkillSlotUI skillSlot = skillUI.GetComponent<SkillSlotUI>();
            if (skillSlot != null)
            {
                skillSlot.Initialize(player.Config.Skills[i]);
                skillSlots.Add(skillSlot);
            }
        }
    }

    void OnEnable()
    {
        _playerAttack.OnSkillDeactivated += StartCooldown;
    }

    void OnDisable()
    {
        _playerAttack.OnSkillDeactivated -= StartCooldown;
    }

    private void StartCooldown(int skillNumber)
    {
        if (skillNumber < 0 || skillNumber >= GameConstants.NumSkills) return;
        skillSlots[skillNumber].OnDeactivated();
    }
}