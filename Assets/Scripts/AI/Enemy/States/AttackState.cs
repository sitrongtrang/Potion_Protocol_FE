public class AttackState : IBasicState<EnemyController>
{
    private EnemyController _owner;
    private float _attackCooldown;
    public AttackState(EnemyController controller)
    {
        _owner = controller;
    }
    public void Enter(EnemyController owner, object[] enterParameters)
    {
        _attackCooldown = (float)enterParameters[0];
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
        
        if (_attackCooldown <= 0)
        {
            // ATTACK HERE
            _attackCooldown = _owner.EnemyConf.AttackInterval;
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
