using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private ItemConfig _config;

    public ItemConfig Config => _config;
}