using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeConfig", menuName = "Scriptable Objects/RecipeConfig")]
public class RecipeConfig : ScriptableObject
{
    [SerializeField] private float _craftingTime;
    public float TimeCrafting => _craftingTime;

    [SerializeField] private ItemConfig _item;
    public ItemConfig Item => _item;

    [SerializeField] private List<ItemConfig> _items;
    public List<ItemConfig> Items => _items;
}
