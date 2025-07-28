public class GetPlayerId : ServerMessage
{
    [FieldOrder(0)]
    public string PlayerId;
    [FieldOrder(1)]
    public string[] AllPlayerIds;
    public GetPlayerId() : base(NetworkMessageTypes.Server.Pregame.GetPlayerId) { }
}