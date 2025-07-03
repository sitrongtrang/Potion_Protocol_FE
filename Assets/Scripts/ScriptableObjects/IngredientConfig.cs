using UnityEngine;

public enum IngredientType 
{
    Monster,
    Plant,
    Ore,
}

[CreateAssetMenu(fileName = "IngredientConfig", menuName = "Scriptable Objects/IngredientConfig")]
public class IngredientConfig : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private string _id;
    [SerializeField] private IngredientType _type;
    [SerializeField] private float _existDuration;
    [SerializeField] private IngredientController _prefab;

    public string Name => _name;
    public float ExistDuration => _existDuration;
    public IngredientController Prefab => _prefab;
}
