using System.Collections;
using UnityEngine;

public class IngredientController : MonoBehaviour
{
    [SerializeField] private IngredientConfig _config;
    public IngredientConfig Config => _config;

    public void Initialize(IngredientConfig config)
    {
        _config = config;
        StartCoroutine(DisappearCoroutine());
    }
    
    private IEnumerator DisappearCoroutine()
    {
        yield return new WaitForSeconds(_config.ExistDuration);
        Destroy(gameObject);
    }
}