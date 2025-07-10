using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Scriptable Objects/Enemy/BasicMeleeConfiguration")]
public class EnemyConfig_BasicMelee : EnemyConfig
{
    public override void HandleAttack(EnemyController controller)
    {
        
    }

    public override void OnDeath(EnemyController controller)
    {
        Debug.Log(Name + "died, dropping " + Item.Name);
    }
}