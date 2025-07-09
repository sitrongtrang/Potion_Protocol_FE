using UnityEngine;

public class ReturnState : IBasicState<EnemyController>
{
    private EnemyController _owner;
    public ReturnState(EnemyController controller)
    {
        _owner = controller;
    }
    public void Enter(EnemyController owner, object[] enterParameters = null)
    {
        _owner.SetTargetToMove(_owner.PatrolCenter);
    }

    public void Execute(EnemyController owner)
    {
        if (_owner.IsPlayerInRange && !_owner.IsTooFarFromPatrolCenter())
        {
            if (_owner.BasicStateMachine.ChangeState(EnemyState.Chase))
                return;
        }
        
        if (_owner.PathVectorList != null)
        {
            // Debug.Log(Vector2.Distance(_owner.transform.position, _owner.PatrolCenter));
            _owner.EnemyConf.HandleMove(_owner);
        }
        else
        {   
            if (_owner.BasicStateMachine.ChangeState(EnemyState.Patrol))
                return;
        }
    }

    public void Exit(EnemyController owner)
    {
        
    }
}