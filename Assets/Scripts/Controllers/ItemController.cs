using System.Collections;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    private ItemConfig _config;

    public ItemConfig Config => _config;

    void OnEnable()
    {
        if (_config is CraftableItemConfig craftableItemConfig) {
            StartCoroutine(Disappear(craftableItemConfig.ExpireTime));
        }
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Initialize(EntityConfig entityConfig)
    {
        if (entityConfig is ItemConfig itemConfig)
        {
            _config = itemConfig;
            GetComponent<SpriteRenderer>().sprite = itemConfig.Icon;
        }
    }

    private IEnumerator Disappear(float delay)
    {
        yield return new WaitForSeconds(delay);
        ItemPool.Instance.RemoveItem(this);
    }
}