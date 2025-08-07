using UnityEngine;

[SerializeField]
public class PlayerGetRoomByNameRequest : ClientMessage
{
    [FieldOrder(0)]
    public string RoomName;
    public PlayerGetRoomByNameRequest() : base(NetworkMessageTypes.Client.Pregame.GetRoomByName) { }
}
