using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftConfig", menuName = "Scriptable Objects/CraftConfig")]
public class CraftConfig : ScriptableObject
{
    [SerializeField] private float _craftingTime;
    [SerializeField] private GameObject _item;

    public List<IngredientConfig> Ingredients;

    public float TimeCrafting => _craftingTime;
    public GameObject Item => _item;
}
