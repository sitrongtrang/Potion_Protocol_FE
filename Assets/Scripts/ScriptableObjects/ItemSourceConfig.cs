using UnityEngine;

[CreateAssetMenu(fileName = "ItemSourceConfig", menuName = "Scriptable Objects/ItemSourceConfig")]
public class ItemSourceConfig : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private string _id;
    public string Id => _id;

    [SerializeField] private ItemSourceController _prefab;
    public ItemSourceController Prefab => _prefab;

    [SerializeField] private ItemConfig _droppedItem;
    public ItemConfig DroppedItem => _droppedItem;
}