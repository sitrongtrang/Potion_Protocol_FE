using System;

[Flags]
public enum EnemyStateFlags
{
    None = 0,
    Return = 1 << 0,
    Patrol = 1 << 1,
    Idle = 1 << 2,
}
public class EnemyState
{
    public string EnemyId;
    public string EnemyType;
    public float PositionX;
    public float PositionY;
    public float CurrentHealth;
    public int CurrentState;
    public string ItemDrop;
}