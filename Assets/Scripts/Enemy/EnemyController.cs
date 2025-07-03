using UnityEngine;

public class EnemyController : MonoBehaviour
{
    // [Header("Movement State")]
    // [Header("Components")]
    [field: SerializeField] public EnemyConfig EnemyConf { get; private set; }
    public enum EnemyMovementState
    {
        Patrol,
        Chase,
        Attack,
        Search,
        Return
    }
    private EnemyMovementState _currentMovementState;
}