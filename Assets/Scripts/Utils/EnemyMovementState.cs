using System;

[Serializable]
public enum EnemyState
{
    // MOVEMENT
    Idle,
    Patrol,
    Chase,
    Search,
    Return,

    // COMBAT
    Attack,
}