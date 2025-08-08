using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StationController : MonoBehaviour
{
    [SerializeField] private StationConfig _config;
    private SpriteRenderer _spriteRenderer;
    private AABBCollider _collider;
    private Vector2 _size;
    private List<RecipeConfig> _recipes;
    [SerializeField] private ProgressBarUI progressBarPrefab;
    private ProgressBarUI _progressBar;

    protected List<ItemConfig> _items;
    protected bool _isCrafting = false;

    public StationConfig Config => _config;

    #region Initialization
    public virtual void Initialize(List<RecipeConfig> recipes)
    {
        _recipes = recipes;
        _items = new();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        if (_spriteRenderer)
        {
            _spriteRenderer.sprite = _config.Icon;
            SetCollider(ref _collider, _spriteRenderer, transform);
            CollisionSystem.InsertStaticCollider(_collider);
        }
    }
    #endregion

    #region Add & Remove
    public virtual bool AddItem(ItemConfig config)
    {
        if (_isCrafting)
        {
            DropItem(config);
            return false;
        }
        else
        {
            _items.Add(config);
            return true;
        }
    }

    public virtual void RemoveItem(int idx, bool drop = true)
    {
        ItemConfig item = _items[idx];
        _items.RemoveAt(idx);
        if (drop) DropItem(item, idx + 1);
    }

    protected void DropItem(ItemConfig item, int offset = 1)
    {
        Vector2 stationPos = transform.position;
        Vector2 dropPosition = stationPos + GameConstants.DropItemSpacing * offset * Vector2.down;
        ItemPool.Instance.SpawnItem(item, dropPosition);
    }

    private void ClearItems(bool drop = true)
    {
        for (int i = _items.Count - 1; i >= 0; i--)
            RemoveItem(i, drop);
    }
    #endregion

    #region Craft
    public virtual void StartCrafting()
    {
        if (_isCrafting)
        {
            ClearItems(true);
            return;
        }

        int recipeIndex = FindMatchingRecipe();
        ClearItems(recipeIndex == -1);
        if (recipeIndex != -1)
            StartCoroutine(SimulateCraft(_recipes[recipeIndex]));
    }

    private IEnumerator SimulateCraft(RecipeConfig recipe)
    {
        _isCrafting = true;

        StartProgressBar(recipe.CraftingTime);
        yield return new WaitForSeconds(recipe.CraftingTime);

        _isCrafting = false;
        if (_progressBar != null)
            _progressBar.gameObject.SetActive(false);

        DropItem(recipe.Product);
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
                return i;
        }
        return -1;
    }
    #endregion

    private void StartProgressBar(float duration)
    {
        if (_progressBar == null)
        {
            _progressBar = Instantiate(progressBarPrefab, FindFirstObjectByType<Canvas>().transform);
            Vector2 barOffset = Vector2.down * 0.8f + (_config.Type == StationType.AlchemyStation ? Vector2.right * 0.4f : Vector2.zero);
            _progressBar.Initialize(transform, barOffset);
        }
        else
        {
            _progressBar.gameObject.SetActive(true);
        }

        _progressBar.StartProgress(duration);
    }

    #region Interaction
    public virtual Vector2 GetTransferZone()
    {
        return transform.position;
    }

    public void SetCollider(ref AABBCollider collider, SpriteRenderer spriteRenderer, Transform transform)
    {
        if (collider == null)
        {
            AABBCollider temp = AABBCollider.GetColliderBaseOnSprite(spriteRenderer, transform);
            collider = new AABBCollider(temp)
            {
                Layer = (int)EntityLayer.Obstacle,
                Owner = gameObject
            };
        }
        else
        {
            collider.SetSize(_size);
            Vector2 center = transform.position;
            collider.SetBottomLeft(center - _size / 2f);
        }

    }
    #endregion
}