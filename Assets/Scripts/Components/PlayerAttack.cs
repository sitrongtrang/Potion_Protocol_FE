using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack
{
    private PlayerController _player;
    private PlayerInputManager _inputManager;
    private InputAction[] _skillActions;
    private bool _canAttack = true;
    private bool[] _canUseSkills = new bool[GameConstants.NumSkills];
    private SkillConfig[] _skills = new SkillConfig[GameConstants.NumSkills];
    private float _damageMultiplier = 1;

    public event Action<int> OnSkillDeactivated;
    public float DamageMultiplier
    {
        get => _damageMultiplier;
        set => _damageMultiplier = value;
    }

    #region Initilization
    public void Initialize(PlayerController player, PlayerInputManager inputManager)
    {
        _player = player;
        _skills = player.Config.Skills;

        _inputManager = inputManager;

        for (int i = 0; i < 3; i++)
        {
            _canUseSkills[i] = true;
        }

        _skillActions = new InputAction[]
        {
            _inputManager.controls.Player.Skill1,
            _inputManager.controls.Player.Skill2,
            _inputManager.controls.Player.Skill3
        };

        // for (int i = 0; i < _skillActions.Length; i++)
        // {
        //     int index = i;
        //     _skillActions[i].performed += ctx => ActivateSkill(index);
        // }
    }
    #endregion

    #region Attack
    public void TryAttack()
    {
        if (!_canAttack) return;

        // Check wall hit
        Vector2 dir = _player.Movement.PlayerDir.normalized;
        float skinWidth = 0.2f;
        Vector2 origin = (Vector2)_player.AttackPoint.position + dir * skinWidth;
        bool hitObstacle = CheckWall(origin, dir);

        if (hitObstacle)
        {
            Debug.Log("Vướng tường nè má.");
            return;
        }

        // Simulate attack logic
        _player.StartCoroutine(SimulateAttack(origin, dir));
    }

    public IEnumerator SimulateAttack(Vector2 origin, Vector2 dir)
    {
        if (!PlayAnimation(dir))
        {
            yield break;
        }

        _canAttack = false;
        yield return new WaitForSeconds(_player.Config.AttackDelay);
        HitEnemy(origin, dir);
        yield return new WaitForSeconds(_player.Config.AttackCooldown - _player.Config.AttackDelay);
        _canAttack = true;
    }

    private bool PlayAnimation(Vector2 dir)
    {
        _player.SwordAnimatr.SetTrigger("Attack");
        if (dir.x != 0 || dir.y != 0)
        {
            _player.SwordAnimatr.SetFloat("MoveX", dir.x);
            _player.SwordAnimatr.SetFloat("MoveY", dir.y);
            return true;
        }
        return false;
    }

    private bool CheckWall(Vector2 origin, Vector2 dir)
    {
        float minDistanceToWall = 0.15f;
        List<AABBCollider> walls = CollisionSystem.RayCast(origin, dir, minDistanceToWall, EntityLayer.Obstacle);

        Debug.DrawRay(origin, dir.normalized * minDistanceToWall, Color.cyan, 2f);

        if (walls.Count > 0) return true;
        else return false;
    }

    private void HitEnemy(Vector2 origin, Vector2 dir)
    {
        float attackRange = _player.Weapons[0].AttackRange;
        List<AABBCollider> walls = CollisionSystem.RayCast(origin, dir, attackRange, EntityLayer.Obstacle);

        float maxReach = attackRange;
        CustomRay ray = new CustomRay(origin, dir);
        for (int i = 0; i < walls.Count; i++)
        {
            AABBCollider col = walls[i];
            if (col.Raycast(ray, out float dist))
            {
                if (dist < maxReach) maxReach = dist;
            }
        }

        Vector2 bottomLeft = Vector2.zero;
        Vector2 size = Vector2.zero;
        if (dir.x < 0)
        {
            bottomLeft = new Vector2(origin.x - maxReach, origin.y);
            size = new Vector2(-dir.x * maxReach, _player.GetComponent<SpriteRenderer>().bounds.size.y * 0.5f);
        }
        else if (dir.x > 0)
        {
            bottomLeft = new Vector2(origin.x, origin.y);
            size = new Vector2(dir.x * maxReach, _player.GetComponent<SpriteRenderer>().bounds.size.y * 0.5f);
        }
        else
        {
            if (dir.y < 0)
            {
                bottomLeft = new Vector2(origin.x, origin.y - maxReach);
                size = new Vector2(_player.GetComponent<SpriteRenderer>().bounds.size.x, -dir.y * maxReach);
            }
            else if (dir.y > 0)
            {
                bottomLeft = new Vector2(origin.x - _player.GetComponent<SpriteRenderer>().bounds.size.x, origin.y);
                size = new Vector2(_player.GetComponent<SpriteRenderer>().bounds.size.x, dir.y * maxReach);
            }
        }

        AABBCollider hitbox = new AABBCollider(bottomLeft, size)
        {
            Layer = (int)EntityLayer.Player
        };
        hitbox.Mask.SetLayer((int)EntityLayer.Enemy);
        hitbox.Mask.SetLayer((int)EntityLayer.ItemSource);
        List<AABBCollider> hitTargets = CollisionSystem.RetrieveCollided(hitbox);

        for (int i = 0; i < hitTargets.Count; i++)
        {
            if (hitTargets[i].Owner.CompareTag("Player"))
                continue;

            if (hitTargets[i].Owner.CompareTag("Enemy"))
            {
                EnemyController enemy = hitTargets[i].Owner.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(_damageMultiplier * _player.Weapons[0].AttackDamage);
                }
            }

            if (hitTargets[i].Owner.CompareTag("ItemSource"))
            {
                ItemSourceController itemSource = hitTargets[i].Owner.GetComponent<ItemSourceController>();
                if (itemSource != null)
                {
                    itemSource.OnFarmed();
                }
            }
        }
    }
    #endregion

    #region Skills
    private void ActivateSkill(int skillNumber)
    {
        _canUseSkills[skillNumber] = false;
        _skills[skillNumber].Activate(_player);
        Debug.Log($"Using ability {skillNumber}");

        // Start skill effect timer
        _player.StartCoroutine(SkillTimer(_skills[skillNumber].TimeAlive, skillNumber, DeactivateSkill));
    }

    private void DeactivateSkill(int skillNumber)
    {
        _skills[skillNumber].Deactivate(_player);
        OnSkillDeactivated?.Invoke(skillNumber);

        // Start skill cooldown timer
        _player.StartCoroutine(SkillTimer(_skills[skillNumber].Cooldown, skillNumber, ResetSkill));
    }

    private void ResetSkill(int skillNumber)
    {
        _canUseSkills[skillNumber] = false;
    }

    private IEnumerator SkillTimer(float duration, int skillNumber, Action<int> callback)
    {
        yield return new WaitForSeconds(duration);
        if (callback != null) callback(skillNumber);
    }
    #endregion
}
