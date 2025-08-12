using UnityEngine;

public class PlayerState : IStateSnapshot
{
    [FieldOrder(0)]
    public string PlayerId;
    [FieldOrder(1)]
    public float MoveSpeed;
    [FieldOrder(2)]
    public float PositionX;
    [FieldOrder(3)]
    public float PositionY;
    [FieldOrder(4)]
    public bool IsDashing;
    [FieldOrder(5)]
    public bool IsAttacking;
    [FieldOrder(6)]
    public int ProcessedInputSequence;
    [FieldOrder(7)]
    public string[] InventoryItemTypes;
    [FieldOrder(8)]
    public int Score;

    int IStateSnapshot.ProcessedInputSequence => ProcessedInputSequence;
}