using UnityEngine;

public class OreConfig : ScriptableObject
{
    [SerializeField] private ItemConfig _resource;
    public ItemConfig Resource => _resource;
}