using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RecipeConfig", menuName = "Scriptable Objects/RecipeConfig")]
public class RecipeConfig : EntityConfig
{
    [SerializeField] private float _craftingTime;
    public float CraftingTime => _craftingTime;

    [SerializeField] private RecipeType _type;
    public RecipeType Type => _type;

    [SerializeField] private ItemConfig _product;
    public ItemConfig Product => _product;

    [SerializeField] private List<ItemConfig> _inputs;
    public List<ItemConfig> Inputs => _inputs;
}
