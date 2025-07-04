using System;
using System.Collections;
using UnityEngine;

public class PlayerAttack : IComponent, IUpdatableComponent
{
    private PlayerController _player;
    // bool _isAttacking = false;
    bool _canAttack = true;
    bool[] _canUseSkills = new bool[GameConstants.NumSkills];
    bool _isInAction = false;

    public void Initialize(PlayerController player)
    {
        for (int i = 0; i < 3; i++)
        {
            _canUseSkills[i] = true;
        }
        _player = player;
    }

    public void MyUpdate()
    {
        
        if (!_isInAction)
        {
            
            if (_canUseSkills[0] && Input.GetKeyDown(KeyCode.U))
            {
                _player.StartCoroutine(UseSkill(1));
            }
            if (_canUseSkills[1] && Input.GetKeyDown(KeyCode.I))
            {
                _player.StartCoroutine(UseSkill(2));
            }
            if (_canUseSkills[2] && Input.GetKeyDown(KeyCode.O))
            {
                _player.StartCoroutine(UseSkill(3));
            }
        }
    }
    
    // Can not perform any action when an action is active by variable _isInAction
    public IEnumerator Attack()
    {
        if (_canAttack && !_isInAction && Input.GetKeyDown(KeyCode.J))
        {
            _canAttack = false;
            // _isInAction = true;
            Debug.Log("Player Attacked");
            yield return new WaitForSeconds(_player.Config.AttackCooldown);
            // _isInAction = false;
            _canAttack = true;
        }
        
    }
    
    private IEnumerator UseSkill(int skillNumber)
    {
        _canUseSkills[skillNumber - 1] = false;
        // _isInAction = true;
        Debug.Log($"Using ability {skillNumber}");
        yield return new WaitForSeconds(_player.Config.SkillsCoolDown[skillNumber - 1]);
        // _isInAction = false;
        _canUseSkills[skillNumber - 1] = true;
    }
}
