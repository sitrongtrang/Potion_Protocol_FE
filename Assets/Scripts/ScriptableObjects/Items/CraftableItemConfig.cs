using UnityEngine;

[CreateAssetMenu(fileName = "CraftableItemConfig", menuName = "Scriptable Objects/Items/CraftableItemConfig")]
public class CraftableItemConfig : ItemConfig
{
    [SerializeField] private Sprite _craftPlace;
    public Sprite CraftPlace => _craftPlace;
}
