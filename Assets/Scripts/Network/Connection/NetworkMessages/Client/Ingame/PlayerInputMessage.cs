using System;
using UnityEngine;

[Flags]
public enum InputFlags
{
    None = 0,
    Dash = 1 << 0,
    Move = 1 << 2,
    Attack = 1 << 3,
    Pickup = 1 << 4,
    Drop = 1 << 5,
    Transfer = 1 << 6,
    Craft = 1 << 7,
    Submit = 1 << 8,
    UseAbility1 = 1 << 9,
    UseAbility2 = 1 << 10,
    UseAbility3 = 1 << 11
}

[Serializable]
public class PlayerInputMessage : ClientMessage, IInputSnapshot
{
    [FieldOrder(0)]
    public long ClientSendTime;
    [FieldOrder(1)]
    public long ClientEstimatedServerTime;
    [FieldOrder(2)]
    public int InputSequence;
    [FieldOrder(3)]
    public int Flags;
    [FieldOrder(4)]
    public float MoveDirX;
    [FieldOrder(5)]
    public float MoveDirY;
    [FieldOrder(6)]
    public int SelectedSlot;
    public PlayerInputMessage(PlayerInputSnapshot playerInputSnapshot) : base(NetworkMessageTypes.Client.Ingame.Input)
    {
        MoveDirX = playerInputSnapshot.MoveDir.x;
        MoveDirY = playerInputSnapshot.MoveDir.y;

        Flags = playerInputSnapshot.DashPressed ? Flags |= (int)InputFlags.Dash : Flags;
        Flags = !(MoveDirX == 0 || MoveDirY == 0) ? Flags |= (int)InputFlags.Move : Flags;
        
        Flags = playerInputSnapshot.AttackPressed ? Flags |= (int)InputFlags.Attack : Flags;

        Flags = playerInputSnapshot.PickupPressed ? Flags |= (int)InputFlags.Pickup : Flags;
        Flags = playerInputSnapshot.DropPressed ? Flags |= (int)InputFlags.Drop : Flags;
        Flags = playerInputSnapshot.TransferPressed ? Flags |= (int)InputFlags.Transfer : Flags;
        Flags = playerInputSnapshot.CraftPressed ? Flags |= (int)InputFlags.Craft : Flags;
        Flags = playerInputSnapshot.SubmitPressed ? Flags |= (int)InputFlags.Submit : Flags;

        ClientSendTime = TimeSyncUtils.GetUnixTimeMilliseconds();
        ClientEstimatedServerTime = 0;

        SelectedSlot = playerInputSnapshot.SelectedSlot;
    }

    public PlayerInputMessage(PlayerInputMessage other)
        : base(other.MessageType) // copy the base class type
    {
        this.ClientSendTime = other.ClientSendTime;
        this.ClientEstimatedServerTime = other.ClientEstimatedServerTime;
        this.InputSequence = other.InputSequence;
        this.Flags = other.Flags;
        this.MoveDirX = other.MoveDirX;
        this.MoveDirY = other.MoveDirY;
        this.SelectedSlot = other.SelectedSlot;
    }

    int IInputSnapshot.InputSequence => InputSequence;
}
