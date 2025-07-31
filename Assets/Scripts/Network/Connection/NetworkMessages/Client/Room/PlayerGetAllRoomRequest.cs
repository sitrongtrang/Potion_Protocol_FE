using UnityEngine;

[SerializeField]
public class PlayerGetAllRoomRequest : ClientMessage
{
    public PlayerGetAllRoomRequest() : base(NetworkMessageTypes.Client.Pregame.GetAllRoom) { }
}
