using UnityEngine;

[SerializeField]
public class PlayerGetRoomByIDRequest : ClientMessage
{
    [FieldOrder(0)]
    public string roomID;
    public PlayerGetRoomByIDRequest() : base(NetworkMessageTypes.Client.Pregame.GetRoomByID) { }
}
