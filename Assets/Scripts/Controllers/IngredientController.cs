using UnityEngine;

public class IngredientController : MonoBehaviour
{
    private IngredientConfig _config;

    public IngredientConfig Config => _config;

    public void Initialize(IngredientConfig config)
    {
        _config = config;
    }
}