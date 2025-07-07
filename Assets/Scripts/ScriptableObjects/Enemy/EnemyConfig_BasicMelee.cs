using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Scriptable Objects/Enemy/BasicMeleeConfiguration")]
public class EnemyConfig_BasicMelee : EnemyConfig
{
    public override void Attack(EnemyController controller)
    {
        
    }

    public override void Initialize(EnemyController controller)
    {
        
    }

    public override void Move(EnemyController controller)
    {
        Vector2 direction = (controller.TargetToMove - controller.transform.position).normalized;
        controller.transform.Translate(Speed * Time.deltaTime * direction);
    }

    public override void OnDeath(EnemyController controller)
    {
        Debug.Log(Name + "died, dropping " + Ingredient.Name);
    }
}