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
    [FieldOrder(0)]
    public string EnemyId;
    [FieldOrder(1)]
    public string EnemyType;
    [FieldOrder(2)]
    public float PositionX;
    [FieldOrder(3)]
    public float PositionY;
    [FieldOrder(4)]
    public float CurrentHealth;
}