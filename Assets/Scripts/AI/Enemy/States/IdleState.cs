using UnityEngine;

public class IdleState : IBasicState<EnemyController>
{
    private EnemyController _owner;
    private float _idleTime;
    private EnemyState _previousState;
    private float _searchRemaining;
    public IdleState(EnemyController controller)
    {
        _owner = controller;
    }
    public void Enter(EnemyController owner, object[] enterParameters = null)
    {
        _previousState = (EnemyState)enterParameters[0];
        _idleTime = (float)enterParameters[1];

        if (_previousState == EnemyState.Search)
            _searchRemaining = (float)enterParameters[2];
    }

    public void Execute(EnemyController owner)
    {
        if (_owner.IsPlayerInRange())
        {
            _owner.BasicStateMachine.ChangeState(EnemyState.Chase);
            return;
        }
        
        _idleTime -= Time.deltaTime;
        if (_idleTime < 0)
        {
            if (_previousState == EnemyState.Patrol)
            {
                _owner.BasicStateMachine.ChangeState(EnemyState.Patrol);
                return;
            }
            else if (_previousState == EnemyState.Search)
            {
                _owner.BasicStateMachine.ChangeState(EnemyState.Search, new object[]{
                    _searchRemaining
                });
                return;
            }
        }
    }

    public void Exit(EnemyController owner)
    {

    }
}