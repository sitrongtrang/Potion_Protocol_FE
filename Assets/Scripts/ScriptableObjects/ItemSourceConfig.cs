using UnityEngine;

[CreateAssetMenu(fileName = "ItemSourceConfig", menuName = "Scriptable Objects/ItemSourceConfig")]
public class ItemSourceConfig : EntityConfig
{
    [SerializeField] private string _id;
    public string Id => _id;

    [SerializeField] private ItemSourceController _prefab;
    public ItemSourceController Prefab => _prefab;

    [SerializeField] private ItemConfig _droppedItem;
    public ItemConfig DroppedItem => _droppedItem;
}