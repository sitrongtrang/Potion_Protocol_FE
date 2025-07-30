using UnityEngine;

public abstract class EnemyConfig : EntityConfig
{
    [Header("Basic Stats")]
    [SerializeField] private float _hp;
    public float Hp => _hp;

    [SerializeField] private float _speed;
    public float Speed => _speed;

    [SerializeField] private float _damage;
    public float Damage => _damage;

    [Header("Patrol")]
    [SerializeField] private float _patrolRadius;
    public float PatrolRadius => _patrolRadius;

    [SerializeField] private float _patrolInterval;
    public float PatrolInterval => _patrolInterval;

    [Header("Chase")]
    [SerializeField] private float _chaseRadius;
    public float ChaseRadius => _chaseRadius;

    [Header("Attack")]
    [SerializeField] private float _visionRadius;
    public float VisionRadius => _visionRadius;

    [SerializeField] private float _attackRadius;
    public float AttackRadius => _attackRadius;

    [SerializeField] private float _attackInterval;
    public float AttackInterval => _attackInterval;

    [Header("Search")]
    [SerializeField] private float _searchRadius;
    public float SearchRadius => _searchRadius;

    [SerializeField] private float _searchDuration;
    public float SearchDuration => _searchDuration;

    [SerializeField] private float _searchInterval;
    public float SearchInterval => _searchInterval;

    [Header("Drop")]
    [SerializeField] private ItemConfig _item;
    public ItemConfig Item => _item;

    public virtual void HandleMove(EnemyController controller)
    {
        if (controller.PathVectorList != null)
        {
            Vector2 targetPosition =
                controller.CurrentPathIndex < controller.PathVectorList.Count ?
                controller.PathVectorList[controller.CurrentPathIndex] : controller.transform.position;
            if (Vector2.Distance(controller.transform.position, targetPosition) > 0.1f)
            {
                Vector2 moveDir = (targetPosition - (Vector2)controller.transform.position).normalized;
                controller.transform.Translate(Speed * Time.deltaTime * moveDir);
                //Vector2 targetPos = controller.Rb.position + moveDir * Speed * Time.fixedDeltaTime;
                //controller.Rb.MovePosition(targetPos);
                // Set animation move here
                controller.Animatr.SetBool("IsMoving", true);
                controller.Animatr.SetFloat("MoveX", moveDir.x);
                controller.Animatr.SetFloat("MoveY", moveDir.y);
            }
            else
            {
                controller.CurrentPathIndexIncrement();
                if (controller.CurrentPathIndex >= controller.PathVectorList.Count)
                {
                    controller.StopMoving();
                    // Set animation stop here
                    controller.Animatr.SetBool("IsMoving", false);
                }
            }
        }
        else
        {
            // Set animation stop here
        }
    }
    public abstract void HandleAttack(EnemyController controller);
    public virtual void OnDeath(EnemyController controller)
    {
        ItemPool.Instance.SpawnItem(_item, controller.transform.position);
    }
    public virtual void Initialize(EnemyController controller)
    {
        IdleState idleState = new(controller);
        PatrolState patrolState = new(controller);
        ReturnState returnState = new(controller);

        controller.BasicStateMachine.AddState(EnemyActionEnum.Idle, idleState);
        controller.BasicStateMachine.AddState(EnemyActionEnum.Patrol, patrolState);
        controller.BasicStateMachine.AddState(EnemyActionEnum.Return, returnState);
        
        controller.BasicStateMachine.ChangeState(EnemyActionEnum.Return);
    }
    private void OnValidate()
    {
        // Ensure AttackRadius < VisionRadius < ChaseRadius
        if (_attackRadius >= _visionRadius)
        {
            Debug.LogWarning($"Attack radius ({_attackRadius}) should be smaller than vision radius ({_visionRadius}). Adjusting...");
            _visionRadius = _attackRadius + 0.1f;
        }

        if (_visionRadius >= _chaseRadius)
        {
            Debug.LogWarning($"Vision radius ({_visionRadius}) should be smaller than chase radius ({_chaseRadius}). Adjusting...");
            _chaseRadius = _visionRadius + 0.1f;
        }
    }
}