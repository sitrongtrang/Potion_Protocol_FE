using System;

[Flags]
public enum EnemyStt
{
    None = 0,
    Idle = 1 << 0,
    Move = 1 << 1,

}
public class EnemyStateServer
{
    public string EnemyId;
    public float PositionX;
    public float PositionY;
    public float CurrentHealth;
    public int CurrentState;
    public string ItemDrop;
}