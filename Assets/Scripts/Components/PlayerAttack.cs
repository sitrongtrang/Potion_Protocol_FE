using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack
{
    private PlayerController _player;
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
            //yield break;
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
        CustomRay ray = new CustomRay(origin, dir);
        float attackRange = _player.Weapon.AttackRange;
        Vector2 end = origin + dir.normalized * attackRange;
        float minX = Mathf.Min(origin.x, end.x);
        float minY = Mathf.Min(origin.y, end.y);
        float maxX = Mathf.Max(origin.x, end.x);
        float maxY = Mathf.Max(origin.y, end.y);
        AABBCollider rayCollider = new AABBCollider(new Vector2(minX, minY), new Vector2(maxX - minX, maxY - minY))
        {
            Layer = (int)EntityLayer.Player
        };
        rayCollider.Mask.SetLayer((int)EntityLayer.Obstacle);

        List<AABBCollider> walls = CollisionSystem.RetrieveCollided(rayCollider);

        float maxReach = attackRange;

        for (int i = 0; i < walls.Count; i++)
        {
            AABBCollider col = walls[i];
            if (col.Raycast(ray, out float dist))
            {
                if (dist < maxReach)
                    maxReach = dist;
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

        Debug.Log("Hitbox size: " + size);
        rayCollider = new AABBCollider(bottomLeft, size)
        {
            Layer = (int)EntityLayer.Player
        };
        rayCollider.Mask.SetLayer((int)EntityLayer.Enemy);
        List<AABBCollider> hitTargets = CollisionSystem.RetrieveCollided(rayCollider);

        List<EnemyController> hitEnemies = new List<EnemyController>();

        for (int i = 0; i < hitTargets.Count; i++)
        {
            if (hitTargets[i].Owner.CompareTag("Player"))
                continue;

            if (hitTargets[i].Owner.CompareTag("Enemy"))
            {
                EnemyController enemy = hitTargets[i].Owner.GetComponent<EnemyController>();
                if (enemy != null)
                {
                    enemy.TakeDamage(_player.Weapon.AttackDamage);
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

        Debug.DrawLine(new Vector2(rayCollider.Bounds.xMin, rayCollider.Bounds.yMin),
                        new Vector2(rayCollider.Bounds.xMax, rayCollider.Bounds.yMin));
        Debug.DrawLine(new Vector2(rayCollider.Bounds.xMax, rayCollider.Bounds.yMin),
                        new Vector2(rayCollider.Bounds.xMax, rayCollider.Bounds.yMax));
        Debug.DrawLine(new Vector2(rayCollider.Bounds.xMax, rayCollider.Bounds.yMax),
                        new Vector2(rayCollider.Bounds.xMin, rayCollider.Bounds.yMax));
        Debug.DrawLine(new Vector2(rayCollider.Bounds.xMin, rayCollider.Bounds.yMax),
                        new Vector2(rayCollider.Bounds.xMin, rayCollider.Bounds.yMin));
    }
}
