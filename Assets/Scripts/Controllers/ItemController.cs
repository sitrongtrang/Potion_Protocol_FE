using System.Collections;
using UnityEngine;

public class ItemController : MonoBehaviour
{
    [SerializeField] private ItemConfig _config;

    public ItemConfig Config => _config;

    void OnEnable()
    {
        StartCoroutine(Disappear(_config.ExpireTime));
    }

    void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Disappear(float delay)
    {
        yield return new WaitForSeconds(delay);
        ItemPool.Instance.RemoveItem(this);
    }
}