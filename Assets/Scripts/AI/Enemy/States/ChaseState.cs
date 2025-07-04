public class ChaseState : IBasicState<EnemyController>
{
    private EnemyController _owner;
    public ChaseState(EnemyController controller)
    {
        _owner = controller;
    }
    public void Enter(EnemyController owner, object[] enterParameters)
    {

    }

    public void Execute(EnemyController owner)
    {
        if (!_owner.IsPlayerInRange())
        {
            _owner.BasicStateMachine.ChangeState(EnemyState.Search, new object[]{
                _owner.EnemyConf.SearchDuration
            });
            return;
        }
        if (_owner.DistanceToPlayer() <= _owner.EnemyConf.AttackRadius)
        {
            _owner.BasicStateMachine.ChangeState(EnemyState.Attack);
            return;
        }
        else if (_owner.DistanceToPlayer() > _owner.EnemyConf.ChaseRadius)
        {
            _owner.BasicStateMachine.ChangeState(EnemyState.Return);
            return;
        }
    }

    public void Exit(EnemyController owner)
    {

    }
}