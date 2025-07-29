using UnityEngine;

public class IdleState : IBasicState<EnemyController>
{
    private EnemyController _owner;
    private float _idleTime;
    private EnemyActionEnum _previousState;
    private float _searchRemaining;
    public IdleState(EnemyController controller)
    {
        _owner = controller;
    }
    public void Enter(EnemyController owner, object[] enterParameters = null)
    {
        _previousState = (EnemyActionEnum)enterParameters[0];
        _idleTime = (float)enterParameters[1];

        if (_previousState == EnemyActionEnum.Search)
            _searchRemaining = (float)enterParameters[2];
    }

    public void Execute(EnemyController owner)
    {
        if (_owner.IsPlayerInRange)
        {
            if (_owner.BasicStateMachine.ChangeState(EnemyActionEnum.Chase))
                return;
        }
        
        _idleTime -= Time.deltaTime;
        if (_idleTime < 0)
        {
            if (_previousState == EnemyActionEnum.Patrol)
            {
                if (_owner.BasicStateMachine.ChangeState(EnemyActionEnum.Patrol))
                    return;
            }
            else if (_previousState == EnemyActionEnum.Search)
            {
                if (
                    _owner.BasicStateMachine.ChangeState(EnemyActionEnum.Search, new object[]{
                        _searchRemaining
                    })
                ) return;
            }
        }
    }

    public void Exit(EnemyController owner)
    {

    }
}