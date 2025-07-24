using System;

[Serializable]
public enum EnemyActionEnum
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