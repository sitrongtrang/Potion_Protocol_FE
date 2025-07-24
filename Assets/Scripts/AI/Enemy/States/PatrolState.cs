

using UnityEngine;

public class PatrolState : IBasicState<EnemyController>
{
    private EnemyController _owner;
    private Vector2 _patrolTarget;
    public PatrolState(EnemyController controller)
    {
        _owner = controller;
    }
    public void Enter(EnemyController owner, object[] enterParameters = null)
    {
        _patrolTarget = GetPatrolTarget();
        _owner.SetTargetToMove(_patrolTarget);
    }

    public void Execute(EnemyController owner)
    {
        if (_owner.IsPlayerInRange)
        {
            if (_owner.BasicStateMachine.ChangeState(EnemyActionEnum.Chase))
                return;
        }
        
        if (_owner.PathVectorList != null)
        {
            _owner.EnemyConf.HandleMove(_owner);
        }
        else
        {
            if (
                _owner.BasicStateMachine.ChangeState(EnemyActionEnum.Idle, new object[]{
                    EnemyActionEnum.Patrol, _owner.EnemyConf.PatrolInterval
                })
            ) return;
        }
    }

    public void Exit(EnemyController owner)
    {

    }

    private Vector2 GetPatrolTarget()
    {
        return _owner.PatrolCenter + (Vector2)Random.insideUnitCircle * _owner.EnemyConf.PatrolRadius;
    }
}