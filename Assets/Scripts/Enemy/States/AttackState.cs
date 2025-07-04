public class AttackState : IBasicState<EnemyController>
{
    private EnemyController _owner;
    private float _attackInterval;
    private bool _attacked;
    public AttackState(EnemyController controller)
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
        if (_attackInterval <= 0)
        {
            // ATTACK HERE
            _attackInterval = _owner.EnemyConf.AttackInterval;
            _attacked = true;
            return;
        }
        if (_owner.DistanceToPlayer() > _owner.EnemyConf.AttackRadius)
        {

        }
    }

    public void Exit(EnemyController owner)
    {

    }
}
