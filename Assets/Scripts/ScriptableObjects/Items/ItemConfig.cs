using UnityEngine;

[CreateAssetMenu(fileName = "ItemConfig", menuName = "Scriptable Objects/Items/ItemConfig")]
public class ItemConfig : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name => _name;

    [SerializeField] private string _id;
    public string Id => _id;

    [SerializeField] private ItemType _type; 
    public ItemType Type => _type;

    [SerializeField] private ItemController _prefab;
    public ItemController Prefab => _prefab;
}
