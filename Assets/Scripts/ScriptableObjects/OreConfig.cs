using UnityEngine;

[CreateAssetMenu(fileName = "OreConfig", menuName = "Scriptable Objects/OreConfig")]
public class OreConfig : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private string _id;
    public string Id => _id;

    [SerializeField] private OreController _prefab;
    public OreController Prefab => _prefab;

    [SerializeField] private ItemConfig _droppedItem;
    public ItemConfig DroppedItem => _droppedItem;
}