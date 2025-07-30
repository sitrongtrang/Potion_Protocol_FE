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
    public int ProcessedInputSequence;
    // public ItemState[] Inventory;

    int IStateSnapshot.ProcessedInputSequence => ProcessedInputSequence;
}