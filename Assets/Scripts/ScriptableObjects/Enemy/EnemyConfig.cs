using UnityEngine;

public abstract class EnemyConfig : ScriptableObject
{
    [Header("Basic Stats")]
    [SerializeField] public string Name { get; private set; }
    [SerializeField] public float Hp { get; private set; }
    [SerializeField] public float Speed { get; private set; }
    [SerializeField] public float Damage { get; private set; }

    [Header("Patrol")]
    [SerializeField] public float PatrolRadius { get; private set; }
    [SerializeField] public float PatrolInterval { get; private set; }

    [Header("Chase")]
    [SerializeField] public float ChaseRadius { get; private set; }

    [Header("Attack")]
    [SerializeField] public float VisionRadius { get; private set; }
    [SerializeField] public float AttackRadius { get; private set; }
    [SerializeField] public float AttackInterval { get; private set; }

    [Header("Search")]
    [SerializeField] public float SearchRadius { get; private set; }
    [SerializeField] public float SearchDuration { get; private set; }
    [SerializeField] public float SearchInterval { get; private set; }

    [Header("Drop")]
    [SerializeField] public IngredientConfig Ingredient { get; private set; }

    public abstract void Move(EnemyController controller);
    // public abstract void Patrol(EnemyController controller);
    // public abstract void Chase(EnemyController controller, Transform target);
    // public abstract void Attack(EnemyController controller, Transform target);
    // public abstract void Search(EnemyController controller);
    // public abstract void ReturnToSpawn(EnemyController controller);
    public abstract void OnDeath(EnemyController controller);
}