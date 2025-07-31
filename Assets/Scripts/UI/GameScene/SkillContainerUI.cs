using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SkillContainerUI : MonoBehaviour
{
    [SerializeField] private GameObject _skillUIPrefab;
    private List<SkillSlotUI> skillSlots = new();

    public void Initialize(PlayerController player)
    {
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

        player.Attack.OnSkillUsed += UseSkill;
    }

    private void UseSkill(int skillNumber)
    {
        if (skillNumber < 0 || skillNumber >= GameConstants.NumSkills) return;
        skillSlots[skillNumber].Use();
    }
}