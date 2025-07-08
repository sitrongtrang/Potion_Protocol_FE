using UnityEngine;

public class ChaseState : IBasicState<EnemyController>
{
    private EnemyController _owner;
    private float _attackCooldown;
    private EnemyState _previousState;
    public ChaseState(EnemyController controller)
    {
        _owner = controller;
    }
    public void Enter(EnemyController owner, object[] enterParameters)
    {
        if (enterParameters != null)
            _previousState = (EnemyState)enterParameters[0];
        if (_previousState == EnemyState.Attack)
        {
            _attackCooldown = (float)enterParameters[1];
        }
        else
        {
            _attackCooldown = _owner.EnemyConf.AttackInterval / 2;
        }
    }

    public void Execute(EnemyController owner)
    {
        if (_owner.IsTooFarFromPatrolCenter())
        {
            if (
                _owner.BasicStateMachine.ChangeState(EnemyState.Return)
            ) return;
        }

        _owner.SetTargetToMove(_owner.LastSeenPlayerPosition);
        _owner.EnemyConf.Move(_owner);

        if (!_owner.IsPlayerInRange)
        {
            if (
                _owner.BasicStateMachine.ChangeState(EnemyState.Search, new object[]{
                    _owner.EnemyConf.SearchDuration
                })  
            ) return;
        }

        _attackCooldown -= Time.deltaTime;
        if (_owner.DistanceToPlayer() <= _owner.EnemyConf.AttackRadius)
        {
            if (
                _owner.BasicStateMachine.ChangeState(EnemyState.Attack, new object[]{
                    _attackCooldown
                })
            ) return;
        }
    }

    public void Exit(EnemyController owner)
    {

    }
}