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

    public void OnPickup(PlayerInventory inventory)
    {
        bool isAdded = inventory.Add(_config);
        if (isAdded) Destroy(gameObject);
    }

    private IEnumerator DisappearCoroutine()
    {
        yield return new WaitForSeconds(_config.ExistDuration);
        Destroy(gameObject);
    }
}