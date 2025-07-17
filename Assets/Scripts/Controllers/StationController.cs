using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StationController : MonoBehaviour
{
    private StationConfig _config;
    private List<RecipeConfig> _recipes;
    protected List<ItemConfig> _items;

    [SerializeField] private Slider progressBarPrefab;
    private Canvas mainCanvas;
    private Slider _progressBarInstance;

    private void Awake()
    {
        if (mainCanvas == null)
            mainCanvas = FindFirstObjectByType<Canvas>();
    }

    public virtual void Initialize(StationConfig config, List<RecipeConfig> recipes)
    {
        _config = config;
        _recipes = recipes;
        _items = new();
    }

    public virtual void AddItem(ItemConfig config)
    {
        _items.Add(config);
    }

    public virtual void StartCrafting()
    {

        int recipeIndex = FindMatchingRecipe();
        if (recipeIndex != -1)
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                RemoveItem(i);
            }
            _progressBarInstance = Instantiate(progressBarPrefab, mainCanvas.transform);
            StartCoroutine(WaitForCraft(_recipes[recipeIndex]));
        }
        else
        {
            for (int i = _items.Count - 1; i >= 0; i--)
            {
                Vector2 stationPos = transform.position;
                Vector2 dropPosition = stationPos + GameConstants.DropItemSpacing * (i + 1) * Vector2.down;
                ItemPool.Instance.SpawnItem(_items[i], dropPosition);
                RemoveItem(i);
            }
        }
    }

    public virtual void RemoveItem(int idx)
    {
        _items.RemoveAt(idx);
    }

    IEnumerator WaitForCraft(RecipeConfig recipe)
    {
        //yield return new WaitForSeconds(recipe.CraftingTime);

        var slider = _progressBarInstance;
        slider.maxValue = 1f;
        slider.value = 0f;
        var rt = slider.GetComponent<RectTransform>();
        Vector3 offset = Vector3.down * 0.8f
        + (_config.Type == StationType.AlchemyStation ? Vector3.right * 0.4f : Vector3.zero);

        float elapsed = 0f;
        float total = recipe.CraftingTime;
        while (elapsed < total)
        {
            elapsed += Time.deltaTime;
            slider.value = Mathf.Clamp01(elapsed / total);
            Vector3 worldPos = transform.position + offset;
            rt.position = Camera.main.WorldToScreenPoint(worldPos);
            yield return null;
        }

        if (_progressBarInstance != null)
            Destroy(_progressBarInstance.gameObject);

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