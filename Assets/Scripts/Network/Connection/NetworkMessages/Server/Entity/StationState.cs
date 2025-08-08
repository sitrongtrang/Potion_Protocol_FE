public class StationState
{
    [FieldOrder(0)] 
    public string StationId;
    [FieldOrder(1)]
    public string StationType;
    [FieldOrder(2)]
    public float PositionX;
    [FieldOrder(3)]
    public float PositionY;
    [FieldOrder(4)]
    public float CraftTime;
    [FieldOrder(5)]
    public float CraftMaxTime;
    [FieldOrder(6)]
    public bool IsCrafting;

}