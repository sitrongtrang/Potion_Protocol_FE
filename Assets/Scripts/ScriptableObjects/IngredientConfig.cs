using UnityEngine;

[CreateAssetMenu(fileName = "IngredientConfig", menuName = "Scriptable Objects/IngredientConfig")]
public class IngredientConfig : ScriptableObject
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string Id { get; private set; }
    [field: SerializeField] public IngredientType Type { get; private set; }
    [field: SerializeField] public IngredientController Prefab { get; private set; }
}
