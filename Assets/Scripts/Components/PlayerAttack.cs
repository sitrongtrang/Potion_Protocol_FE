using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack
{
    private PlayerController _player;
    private GameObject _weapon;
    private PlayerInputManager _inputManager;
    private InputAction[] _skillActions;
    // bool _isAttacking = false;
    private bool _canAttack = true;
    private bool[] _canUseSkills = new bool[GameConstants.NumSkills];
    private bool _isInAction = false;

    public void Initialize(PlayerController player, PlayerInputManager inputManager)
    {
        for (int i = 0; i < 3; i++)
        {
            _canUseSkills[i] = true;
        }

        _player = player;
        _weapon = _player.transform.Find("Weapons").gameObject;
        // if (_weapon) _weapon.SetActive(false);

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
    
    // Can not perform any action when an action is active by variable _isInAction
    public IEnumerator Attack()
    {
        if (!_canAttack || _isInAction) yield break;
        _canAttack = false;

        Vector2 dir = _player.Movement.PlayerDir.normalized;
        // _isInAction = true;
        Debug.Log("Player Attacked");
        // Check va chạm với tường
        float skinWidth = 0.2f;
        Vector2 origin = (Vector2)_player.AttackPoint.position + dir * skinWidth;
        bool hitObstacle = CheckWall(origin, dir);

        if (hitObstacle)
        {
            Debug.Log("Vướng tường nè má.");
            _canAttack = true;
            yield break;
        }

        // Chạy anim
        if (!PlayAnimation())
        {
            _canAttack = true;
            yield break;
        }
        yield return new WaitForSeconds(_player.Config.AttackDelay);
        // Đánh quái
        HitEnemy(origin, dir);
        // _weapon.SetActive(false);
        yield return new WaitForSeconds(_player.Config.AttackCooldown - _player.Config.AttackDelay);
        // _isInAction = false;
        _canAttack = true;
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

    private bool CheckWall(Vector2 origin, Vector2 dir)
    {
        float minDistanceToWall = 0.25f;

        RaycastHit2D[] nearWallHit = Physics2D.RaycastAll(origin, dir, minDistanceToWall);
        bool hitObstacle = false;

        for (int i = 0; i < nearWallHit.Length; i++)
        {
            if (nearWallHit[i].collider.CompareTag("Obstacle"))
            {
                hitObstacle = true;
                break;
            }
        }

        Debug.DrawRay(origin, dir * minDistanceToWall, Color.cyan, 2f);
        return hitObstacle;
    }

    private bool PlayAnimation()
    {
        // if (_weapon) _weapon.SetActive(true);
        _player.SwordAnimatr.SetTrigger("Attack");
        float playerX = _player.Movement.PlayerDir.x;
        float playerY = _player.Movement.PlayerDir.y;
        if (playerX != 0 || playerY != 0)
        {
            _player.SwordAnimatr.SetFloat("MoveX", playerX);
            _player.SwordAnimatr.SetFloat("MoveY", playerY);
            return true;
        }
        return false;
    }

    private void HitEnemy(Vector2 origin, Vector2 dir)
    {
        RaycastHit2D hitWallFar = Physics2D.Raycast(origin, dir, _player.Weapon.AttackRange);
        float maxReach = (hitWallFar.collider != null && hitWallFar.collider.CompareTag("Obstacle")) ? hitWallFar.distance : _player.Weapon.AttackRange;

        RaycastHit2D[] hitTargets = Physics2D.RaycastAll(origin, dir, maxReach);
        for (int i = 0; i < hitTargets.Length; i++)
        {
            if (hitTargets[i].collider.CompareTag("Player"))
                continue;

            if (hitTargets[i].collider.CompareTag("Enemy"))
            {
                EnemyController enemy = hitTargets[i].collider.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(_player.Weapon.AttackDamage);
                }
            } 
            if (hitTargets[i].collider.CompareTag("ItemSource"))
            {
                ItemSourceController itemSource = hitTargets[i].collider.GetComponent<ItemSourceController>();
                if (itemSource != null)
                {
                    itemSource.OnFarmed();
                }
            }
        }

        Debug.DrawRay(origin, dir * maxReach, Color.red, 2f);
    }
}
