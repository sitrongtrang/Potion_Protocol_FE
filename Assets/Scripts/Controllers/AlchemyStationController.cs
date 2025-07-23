using System.Collections.Generic;
using UnityEngine;

public class AlchemyStationController : StationController
{
    [SerializeField] private GameObject[] itemsOnTable;

    public override void Initialize(List<RecipeConfig> recipes)
    {
        base.Initialize(recipes);
        for (int i = 0; i < itemsOnTable.Length; i++)
        {
            itemsOnTable[i].SetActive(false);
        }
    }

    public override bool AddItem(ItemConfig config)
    {
        if (_items.Count < GameConstants.MaxItemsInAlchemyStation)
        {
            bool added = base.AddItem(config);
            if (!added) return false;
            itemsOnTable[_items.Count - 1].SetActive(true);
            itemsOnTable[_items.Count - 1].GetComponent<SpriteRenderer>().sprite = config.Icon;
            return true;
        }
        else
        {
            Vector2 stationPos = transform.position;
            Vector2 dropPosition = stationPos + GameConstants.DropItemSpacing * Vector2.down;
            ItemPool.Instance.SpawnItem(config, dropPosition);
            Debug.LogWarning("Cannot add more items to the Alchemy Station.");
            return false;
        }
    }

    public override void RemoveItem(int idx)
    {
        base.RemoveItem(idx);
        itemsOnTable[_items.Count].SetActive(true);
        itemsOnTable[_items.Count].GetComponent<SpriteRenderer>().sprite = null;
    }
}