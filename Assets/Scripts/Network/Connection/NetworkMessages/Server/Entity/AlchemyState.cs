public class AlchemyState
{
    [FieldOrder(0)]
    public string StationId;
    [FieldOrder(1)]
    public float CraftTime;
    [FieldOrder(2)]
    public float CraftMaxTime;
    [FieldOrder(3)]
    public bool IsCrafting;
    [FieldOrder(4)]
    public string[] ItemTypeIds;
}