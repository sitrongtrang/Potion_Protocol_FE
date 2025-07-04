using UnityEngine;

public class IdleState : IBasicState<EnemyController>
{
    private EnemyController _owner;
    private float _idleTime;
    private EnemyState _previousState;
    public IdleState(EnemyController controller)
    {
        _owner = controller;
    }
    public void Enter(EnemyController owner, object[] enterParameters = null)
    {
        _previousState = (EnemyState)enterParameters[0];
        _idleTime = (float)enterParameters[1];
    }

    public void Execute(EnemyController owner)
    {
        _idleTime -= Time.deltaTime;
        if (_idleTime < 0)
        {
            if (_previousState == EnemyState.Patrol)
            {
                _owner.BasicStateMachine.ChangeState(EnemyState.Patrol);
            }
        }
        else
        {

        }
    }

    public void Exit(EnemyController owner)
    {

    }
}