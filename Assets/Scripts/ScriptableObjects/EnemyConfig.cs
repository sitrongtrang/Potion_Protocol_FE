using UnityEngine;

// [CreateAssetMenu(fileName = "EnemyConfig", menuName = "Scriptable Objects/Enemy/Configuration")]
public abstract class EnemyConfig : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public float Hp { get; private set; }
    [field: SerializeField] public float Speed { get; private set; }
    [field: SerializeField] public float Damage { get; private set; }
    [field: SerializeField] public float PatrolRadius { get; private set; }
    [field: SerializeField] public float ChaseRadius { get; private set; }
    [field: SerializeField] public float VisionRadius { get; private set; }
    [field: SerializeField] public float AttackRadius { get; private set; }
    [field: SerializeField] public float SearchDuration { get; private set; }
    [field: SerializeField] public string[] IngredientsConfig { get; private set; }
    public abstract void Move(EnemyController controller);
    public abstract void Patrol(EnemyController controller);
    public abstract void Chase(EnemyController controller, Transform target);
    public abstract void Attack(EnemyController controller, Transform target);
    public abstract void Search(EnemyController controller);
    public abstract void ReturnToSpawn(EnemyController controller);
    public abstract void OnDeath(EnemyController controller);
}