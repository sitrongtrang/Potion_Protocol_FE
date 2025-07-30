using System;

[Serializable]
public class BatchPlayerInputMessage : ClientMessage
{
    [FieldOrder(0)]
    public string PlayerId;
    [FieldOrder(1)]
    public PlayerInputMessage[] PlayerInputMessages;
    public BatchPlayerInputMessage() : base(NetworkMessageTypes.Client.Ingame.Input) { }
}