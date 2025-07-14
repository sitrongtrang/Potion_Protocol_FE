using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using static InputSystem_Actions;

public class PlayerAttack : IComponent, IUpdatableComponent
{
    private PlayerInputManager _inputManager;
    private PlayerController _player;
    // bool _isAttacking = false;
    private bool _canAttack = true;
    private bool[] _canUseSkills = new bool[GameConstants.NumSkills];
    private bool _isInAction = false;
    private InputAction[] _skillActions;

    public void Initialize(PlayerController player, PlayerInputManager inputManager)
    {
        for (int i = 0; i < 3; i++)
        {
            _canUseSkills[i] = true;
        }

        _player = player;
        _inputManager = inputManager;
        _skillActions = new InputAction[]
        {
            _inputManager.controls.Player.Skill1,
            _inputManager.controls.Player.Skill2,
            _inputManager.controls.Player.Skill3
        };

        for (int i = 0; i < _skillActions.Length; i++)
        {
            int index = i; // Bắt buộc tạo biến tạm, tránh lỗi closure
            _skillActions[i].performed += ctx =>
            {
                if (_canUseSkills[index] && !_isInAction)
                {
                    _player.StartCoroutine(UseSkill(index + 1));
                }
            };
        }
    }

    public void MyUpdate()
    {
        
    }
    
    // Can not perform any action when an action is active by variable _isInAction
    public IEnumerator Attack()
    {
        if (_canAttack && !_isInAction)
        {
            _canAttack = false;
            // _isInAction = true;
            Debug.Log("Player Attacked");
            _player.SwordAnimatr.SetTrigger("Attack");
            float playerX = _player.Movement.PlayerDir.x;
            float playerY = _player.Movement.PlayerDir.y;
            if (playerX != 0 || playerY != 0)
            {
                _player.SwordAnimatr.SetFloat("MoveX", playerX);
                _player.SwordAnimatr.SetFloat("MoveY", playerY);
            }
            yield return new WaitForSeconds(_player.Config.AttackDelay);
            _player.Weapon.Attack(_player);
            yield return new WaitForSeconds(_player.Config.AttackCooldown - _player.Config.AttackDelay);
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
