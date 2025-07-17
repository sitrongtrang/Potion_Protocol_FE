using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StationController : MonoBehaviour
{
    private StationConfig _config;
    private List<RecipeConfig> _recipes;
    protected List<ItemConfig> _items;

    [SerializeField] private ProgressBarUI progressBarPrefab;
    private ProgressBarUI _progressBar;
    protected bool _isCrafting = false;   

    public virtual void Initialize(StationConfig config, List<RecipeConfig> recipes)
    {
        _config = config;
        _recipes = recipes;
        _items = new();
    }

    public virtual bool AddItem(ItemConfig config)
    {
        if (_isCrafting)
        {
            Vector2 stationPos = transform.position;
            Vector2 dropPosition = stationPos + GameConstants.DropItemSpacing * Vector2.down;
            ItemPool.Instance.SpawnItem(config, dropPosition);
            return false;
        }
        else
        {
            _items.Add(config);
            return true;
        }
    }

    public virtual void StartCrafting()
    {
        if (_isCrafting)
        {
            ClearItems();
        }

        int recipeIndex = FindMatchingRecipe();
        if (recipeIndex != -1)
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                RemoveItem(i);
            }

            StartCoroutine(WaitForCraft(_recipes[recipeIndex]));
        }
        else
        {
            ClearItems();
        }
    }

    public virtual void RemoveItem(int idx)
    {
        _items.RemoveAt(idx);
    }

    private void ClearItems()
    {
        for (int i = _items.Count - 1; i >= 0; i--)
        {
            Vector2 stationPos = transform.position;
            Vector2 dropPosition = stationPos + GameConstants.DropItemSpacing * (i + 1) * Vector2.down;
            ItemPool.Instance.SpawnItem(_items[i], dropPosition);
            RemoveItem(i);
        }
    }

    IEnumerator WaitForCraft(RecipeConfig recipe)
    {
        //yield return new WaitForSeconds(recipe.CraftingTime);
        _progressBar = Instantiate(progressBarPrefab, FindFirstObjectByType<Canvas>().transform);
        Vector2 barOffset = Vector2.down * 0.8f + (_config.Type == StationType.AlchemyStation ? Vector2.right * 0.4f : Vector2.zero);
        _progressBar.Initialize(transform, recipe.CraftingTime, barOffset);

        float elapsed = 0f;
        _isCrafting = true;
        while (elapsed < recipe.CraftingTime)
        {
            elapsed += Time.deltaTime;
            _progressBar.SetProgress(elapsed);
            yield return null;
        }

        _isCrafting = false;
        if (_progressBar != null)
            Destroy(_progressBar.gameObject);

        Vector2 stationPos = transform.position;
        Vector2 dropPosition = stationPos + GameConstants.DropItemSpacing * Vector2.down;
        ItemPool.Instance.SpawnItem(recipe.Product, dropPosition);
    }

    private bool MatchRecipe(RecipeConfig recipe)
    {
        var stationCounts = _items.GroupBy(i => i.Id).ToDictionary(g => g.Key, g => g.Count());
        var recipeCounts = recipe.Inputs.GroupBy(i => i.Id).ToDictionary(g => g.Key, g => g.Count());

        if (stationCounts.Count != recipeCounts.Count)
            return false;

        foreach (var kvp in recipeCounts)
        {
            if (!stationCounts.TryGetValue(kvp.Key, out int count) || count != kvp.Value)
            return false;
        }
        return true;
    }

    private int FindMatchingRecipe()
    {
        for (int i = 0; i < _recipes.Count; i++)
        {
            if (MatchRecipe(_recipes[i]))
            {
                return i;
            }
        }
        return -1;
    }
}