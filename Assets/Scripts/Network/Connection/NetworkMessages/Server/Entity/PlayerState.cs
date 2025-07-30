using UnityEngine;

public class PlayerState : IStateSnapshot
{
    [FieldOrder(0)]
    public string PlayerId;
    [FieldOrder(1)]
    public float MoveSpeed;
    [FieldOrder(2)]
    public Vector2 Position;
    [FieldOrder(3)]
    public bool IsDashing;
    [FieldOrder(4)]
    public int ProcessedInputSequence;
    // public ItemState[] Inventory;

    int IStateSnapshot.ProcessedInputSequence => ProcessedInputSequence;
}