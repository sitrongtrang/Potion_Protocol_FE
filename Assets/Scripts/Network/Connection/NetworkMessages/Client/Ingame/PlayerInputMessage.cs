using System;

[Flags]
public enum InputFlags
{
    None = 0,
    Dash = 1 << 0,
    Move = 1 << 1,
    Attack = 1 << 2,
    Pickup = 1 << 3,
    Drop = 1 << 4,
    Transfer = 1 << 5,
    Craft = 1 << 6,
    Submit = 1 << 7,
    UseAbility = 1 << 8,
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
        Flags = MoveDirX != 0 || MoveDirY != 0 ? Flags |= (int)InputFlags.Move : Flags;

        Flags = playerInputSnapshot.AttackPressed ? Flags |= (int)InputFlags.Attack : Flags;

        Flags = playerInputSnapshot.PickupPressed ? Flags |= (int)InputFlags.Pickup : Flags;
        Flags = playerInputSnapshot.DropPressed ? Flags |= (int)InputFlags.Drop : Flags;
        Flags = playerInputSnapshot.TransferPressed ? Flags |= (int)InputFlags.Transfer : Flags;
        Flags = playerInputSnapshot.CraftPressed ? Flags |= (int)InputFlags.Craft : Flags;
        Flags = playerInputSnapshot.SubmitPressed ? Flags |= (int)InputFlags.Submit : Flags;

        ClientSendTime = TimeSyncUtils.GetUnixTimeMilliseconds();
        ClientEstimatedServerTime = 0;

    }

    int IInputSnapshot.InputSequence => InputSequence;
}
