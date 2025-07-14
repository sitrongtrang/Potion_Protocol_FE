using UnityEngine;

[CreateAssetMenu(fileName = "WeaponConfig", menuName = "Scriptable Objects/WeaponConfig")]
public class WeaponConfig : ScriptableObject
{
    [SerializeField] private float attackRange;
    [SerializeField] private int attackDamage;
    [SerializeField] private LayerMask enemyLayers;

    public void Attack(PlayerController player)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(
               player.AttackPoint.position,
               attackRange,
               enemyLayers
            );

        for (int i = 0; i < hitEnemies.Length; i++)
        {
            EnemyController enemy = hitEnemies[i].GetComponent<EnemyController>();
            if (enemy != null)
            {
                enemy.TakeDamage(attackDamage);
            }
        }
    }
}
