using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Scriptable Objects/Enemy/BasicMeleeConfiguration")]
public class EnemyConfig_BasicMelee : EnemyConfig
{
    // public override void Attack(EnemyController controller, Transform target)
    // {
        
    // }

    // public override void Chase(EnemyController controller, Transform target)
    // {
        
    // }

    public override void Move(EnemyController controller)
    {
        Vector3 direction = (controller.TargetToMove - controller.transform.position).normalized;
        controller.transform.Translate(Speed * Time.deltaTime * direction);
    }

    public override void OnDeath(EnemyController controller)
    {
        Debug.Log(Name + "died, dropping " + Ingredient.Name);
    }

    // public override void Patrol(EnemyController controller)
    // {
    //     if (Vector3.Distance(controller.transform.position, controller.TargetToMove) < 0.1f)
    //     {
    //         controller.SetPatrolTarget(PatrolRadius);
    //         controller._patrolInterval = PatrolInterval;
    //     }
    //     Move(controller);

    // }

    // public override void ReturnToSpawn(EnemyController controller)
    // {
        
    // }

    // public override void Search(EnemyController controller)
    // {
    //     if (Vector3.Distance(controller.transform.position, controller.TargetToMove) < 0.1f)
    //     {
    //         controller.SetSearchTarget(SearchRadius);
    //         controller._searchInterval = SearchInterval;
    //     }
    //     Move(controller);
    // }
}