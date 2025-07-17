using System.Collections.Generic;
using UnityEngine;

public class AlchemyStationController : StationController
{
    [SerializeField] private GameObject[] itemsOnTable;

    public override void Initialize(StationConfig config, List<RecipeConfig> recipes)
    {
        base.Initialize(config, recipes);
        for (int i = 0; i < itemsOnTable.Length; i++)
        {
            itemsOnTable[i].SetActive(false);
        }
    }

    public override void AddItem(ItemConfig config)
    {
        if (_items.Count < GameConstants.MaxItemsInAlchemyStation)
        {
            base.AddItem(config);
            itemsOnTable[_items.Count-1].SetActive(true);
            itemsOnTable[_items.Count-1].GetComponent<SpriteRenderer>().sprite = config.Prefab.GetComponent<SpriteRenderer>().sprite; 
        }
        else
        {
            Vector2 stationPos = transform.position;
            Vector2 dropPosition = stationPos + 0.5f * Vector2.down;
            ItemPool.Instance.SpawnItem(config, dropPosition);
            Debug.LogWarning("Cannot add more items to the Alchemy Station.");
        }
    }

    public override void RemoveItem(int idx)
    {
        base.RemoveItem(idx);
        itemsOnTable[_items.Count].SetActive(true);
        itemsOnTable[_items.Count].GetComponent<SpriteRenderer>().sprite = null;
    }
}