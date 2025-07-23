using System;

[Serializable]
public class BatchPlayerInputMessage : ClientMessage
{
    public PlayerInputMessage[] PlayerInputMessages;
    public BatchPlayerInputMessage() : base(NetworkMessageTypes.Client.Ingame.Input) { }
}