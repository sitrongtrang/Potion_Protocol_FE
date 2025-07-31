using UnityEngine;

[CreateAssetMenu(fileName = "ItemConfig", menuName = "Scriptable Objects/Items/ItemConfig")]
public class ItemConfig : EntityConfig
{
    [SerializeField] private ItemType _type;
    public ItemType Type => _type;

    [SerializeField] private EntityConfig _droppedBy;
    public EntityConfig DroppedBy => _droppedBy;

    [SerializeField] private float _expireTime;
    public float ExpireTime => _expireTime;

}
