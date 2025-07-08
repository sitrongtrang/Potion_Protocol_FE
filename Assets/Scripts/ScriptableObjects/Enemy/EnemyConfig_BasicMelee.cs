using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Scriptable Objects/Enemy/BasicMeleeConfiguration")]
public class EnemyConfig_BasicMelee : EnemyConfig
{
    public override void HandleAttack(EnemyController controller)
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

    public override void HandleMove(EnemyController controller)
    {
        if (controller.PathVectorList != null)
        {
            Vector3 targetPosition = controller.PathVectorList[controller.CurrentPathIndex];
            if (Vector3.Distance(controller.transform.position, targetPosition) > 0.1f)
            {
                Vector3 moveDir = (targetPosition - controller.transform.position).normalized;
                controller.transform.Translate(Speed * Time.deltaTime * moveDir);
                // Set animation move here
            }
            else
            {
                controller.CurrentPathIndexIncrement();
                if (controller.CurrentPathIndex >= controller.PathVectorList.Count)
                {
                    controller.StopMoving();
                    // Set animation stop here
                }
            }
        }
        else
        {
            // Set animation stop here
        }
    }

    public override void OnDeath(EnemyController controller)
    {
        Debug.Log(Name + "died, dropping " + Ingredient.Name);
    }
}