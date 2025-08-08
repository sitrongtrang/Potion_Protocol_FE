using System.Runtime.CompilerServices;
using UnityEngine;

[SerializeField]
public class ServerPlayerLeft : ServerMessage
{
    [FieldOrder(0)]
    public string UserID;
    [FieldOrder(1)]
    public string UserDisplayName;
    [FieldOrder(2)]
    public string LeaderID;
    public ServerPlayerLeft() : base(NetworkMessageTypes.Server.Room.PlayerLeft) { }
}
