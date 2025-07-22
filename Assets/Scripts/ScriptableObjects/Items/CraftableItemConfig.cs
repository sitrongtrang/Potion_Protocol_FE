using UnityEngine;

[CreateAssetMenu(fileName = "CraftableItemConfig", menuName = "Scriptable Objects/Items/CraftableItemConfig")]
public class CraftableItemConfig : ItemConfig
{
    [SerializeField] private float _expireTime;
    public float ExpireTime => _expireTime;

    [SerializeField] private Sprite _craftPlace;
    public Sprite CraftPlace => _craftPlace;
}
