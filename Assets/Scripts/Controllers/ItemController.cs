using UnityEngine;

public class ItemController : MonoBehaviour
{
    private ItemConfig _config;

    public ItemConfig Config => _config;

    public void Initialize(ItemConfig config)
    {
        _config = config;
    }
}