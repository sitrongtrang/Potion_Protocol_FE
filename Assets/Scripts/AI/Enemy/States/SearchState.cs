using UnityEngine;

public class SearchState : IBasicState<EnemyController>
{
    private EnemyController _owner;
    private float _searchRemaining;
    private Vector3 _searchTarget;
    public SearchState(EnemyController controller)
    {
        _owner = controller;
    }
    public void Enter(EnemyController owner, object[] enterParameters = null)
    {
        _searchRemaining = (float)enterParameters[0];

        _searchTarget = GetSearchTarget();
        _owner.SetTargetToMove(_searchTarget);

    }

    public void Execute(EnemyController owner)
    {
        if (_owner.IsPlayerInRange())
        {
            if (_owner.BasicStateMachine.ChangeState(EnemyState.Chase))
                return;
        }
        
        _searchRemaining -= Time.deltaTime;
        if (_searchRemaining <= 0)
        {
            if (_owner.BasicStateMachine.ChangeState(EnemyState.Return))
                return;
        }
        if (Vector3.Distance(_owner.transform.position, _searchTarget) >= 0.1f)
        {
            _owner.EnemyConf.Move(_owner);
        }
        else
        {
            if (
                _owner.BasicStateMachine.ChangeState(EnemyState.Idle, new object[]{
                    EnemyState.Search, _owner.EnemyConf.SearchInterval, _searchRemaining
                })
            ) return;
        }
    }

    public void Exit(EnemyController owner)
    {

    }
    private Vector3 GetSearchTarget()
    {
        return _owner.LastSeenPlayerPosition + (Vector3)Random.insideUnitCircle * _owner.EnemyConf.SearchRadius;
    }
}