using System;

[Flags]
public enum PositionFlag
{
    None = 0,
    OnGround = 1 << 0,
    Inventory = 1 << 1,
    Station = 1 << 2,
    InObject = 1 << 3,
}

public class ItemState
{
    [FieldOrder(0)]
    public string ItemId;
    [FieldOrder(1)]
    public string ItemType;
    [FieldOrder(2)]
    public float PositionX;
    [FieldOrder(3)]
    public float PositionY;
}