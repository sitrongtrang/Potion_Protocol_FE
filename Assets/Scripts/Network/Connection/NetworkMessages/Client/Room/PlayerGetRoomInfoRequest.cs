using UnityEngine;

[SerializeField]
public class PlayerGetRoomInfoRequest : ClientMessage
{
    public PlayerGetRoomInfoRequest() : base(NetworkMessageTypes.Client.Pregame.GetRoomInfo) { }
}
