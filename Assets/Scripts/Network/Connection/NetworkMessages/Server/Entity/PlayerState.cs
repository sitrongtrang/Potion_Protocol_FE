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
    public bool IsAttacking;
    [FieldOrder(5)]
    public int ProcessedInputSequence;
    [FieldOrder(6)]
    public string[] InventoryItemTypes;
    [FieldOrder(7)]
    public int[] InventoryItemIndicies;
    [FieldOrder(8)]
    public int Score;

    int IStateSnapshot.ProcessedInputSequence => ProcessedInputSequence;
}