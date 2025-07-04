

using UnityEngine;

public class PatrolState : IBasicState<EnemyController>
{
    private EnemyController _owner;
    private Vector3 _patrolTarget;
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
        if (_owner.IsPlayerInRange())
        {
            _owner.BasicStateMachine.ChangeState(EnemyState.Chase);
            return;
        }
        
        if (Vector3.Distance(_owner.transform.position, _patrolTarget) >= 0.1f)
        {
            _owner.EnemyConf.Move(_owner);
        }
        else
        {
            _owner.BasicStateMachine.ChangeState(EnemyState.Idle, new object[]{
                EnemyState.Patrol, _owner.EnemyConf.PatrolInterval
            });
            return;
        }
    }

    public void Exit(EnemyController owner)
    {

    }

    private Vector3 GetPatrolTarget()
    {
        return _owner.PatrolCenter + (Vector3)Random.insideUnitCircle * _owner.EnemyConf.PatrolRadius;
    }
}