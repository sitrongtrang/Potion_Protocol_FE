using UnityEngine;

public class OreConfig : ScriptableObject
{
    [SerializeField] private ItemConfig _droppedItem;
    public ItemConfig DroppedItem => _droppedItem;

    [SerializeField] private string _id;
    public string Id => _id;
}