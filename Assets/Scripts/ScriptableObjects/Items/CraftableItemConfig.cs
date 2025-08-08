using UnityEngine;

[CreateAssetMenu(fileName = "CraftableItemConfig", menuName = "Scriptable Objects/Items/CraftableItemConfig")]
public class CraftableItemConfig : ItemConfig
{
    [SerializeField] private StationConfig _craftPlace;
    public StationConfig CraftPlace => _craftPlace;
}
