public class AttackState : IBasicState<EnemyController>
{
    private EnemyController _owner;
    public AttackState(EnemyController controller)
    {
        _owner = controller;
    }
    public void Enter(EnemyController owner, object[] enterParameters)
    {
        throw new System.NotImplementedException();
    }

    public void Execute(EnemyController owner)
    {
        throw new System.NotImplementedException();
    }

    public void Exit(EnemyController owner)
    {
        throw new System.NotImplementedException();
    }
}
