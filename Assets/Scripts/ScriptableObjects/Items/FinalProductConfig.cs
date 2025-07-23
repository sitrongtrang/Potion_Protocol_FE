using UnityEngine;

[CreateAssetMenu(fileName = "FinalProductConfig", menuName = "Scriptable Objects/Items/FinalProductConfig")]
public class FinalProductConfig : CraftableItemConfig
{
    [SerializeField] private int _score;
    public int Score => _score;
}
