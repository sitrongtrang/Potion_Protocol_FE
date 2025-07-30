public class PlayerState : IStateSnapshot
{
    public int ProcessedInputSequence;
    public string PlayerId;
    public float MoveSpeed;
    public float PositionX;
    public float PositionY;

    int IStateSnapshot.ProcessedInputSequence => ProcessedInputSequence;
}