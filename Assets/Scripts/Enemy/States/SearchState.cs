public class SearchState : IBasicState<EnemyController>
{
    private EnemyController _owner;
    public SearchState(EnemyController controller)
    {
        _owner = controller;
    }
    public void Enter(EnemyController owner, object[] enterParameters = null)
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