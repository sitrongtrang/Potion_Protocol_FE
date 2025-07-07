using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Scriptable Objects/Enemy/BasicMeleeConfiguration")]
public class EnemyConfig_BasicMelee : EnemyConfig
{
    public override void Attack(EnemyController controller)
    {
        
    }

    public override void Initialize(EnemyController controller)
    {
        IdleState idleState = new(controller);
        PatrolState patrolState = new(controller);
        ReturnState returnState = new(controller);

        controller.BasicStateMachine.AddState(EnemyState.Idle, idleState);
        controller.BasicStateMachine.AddState(EnemyState.Patrol, patrolState);
        controller.BasicStateMachine.AddState(EnemyState.Return, returnState);
        
        controller.BasicStateMachine.ChangeState(EnemyState.Return);
    }

    public override void Move(EnemyController controller)
    {
        Vector2 direction = (controller.TargetToMove - new Vector2(controller.transform.position.x, controller.transform.position.y)).normalized;
        controller.transform.Translate(Speed * Time.deltaTime * direction);
    }

    public override void OnDeath(EnemyController controller)
    {
        Debug.Log(Name + "died, dropping " + Ingredient.Name);
    }
}