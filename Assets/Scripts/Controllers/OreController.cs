using UnityEngine;

public class OreController : MonoBehaviour
{
    private OreConfig _config;

    public OreConfig Config => _config;

    public void Initialize(OreConfig config)
    {
        _config = config;
    }

    public void OnFarmed()
    {
        ItemPool.Instance.SpawnItem(_config.DroppedItem, transform.position);
        Destroy(gameObject);
    }
}