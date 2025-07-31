using UnityEngine;

[SerializeField]
public class PlayerLeaveRoom : ClientMessage
{
    public PlayerLeaveRoom() : base(NetworkMessageTypes.Client.Pregame.LeaveRoom) { }
}
