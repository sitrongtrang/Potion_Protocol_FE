using System.Collections;
using System.Linq;
using UnityEngine;

public class GridCellObject : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask _collisionLayers;
    [SerializeField] private string[] _collisionTags;
    
    [Header("Debug")]
    [SerializeField] private bool _debug = false;

    // Public properties
    public bool IsOverlapped { get;  private set; }
    public Vector2Int GridPosition => new Vector2Int(_x, _y);
    
    // Events
    public delegate void OnOverlapBox(int x, int y, bool overlapped);
    public event OnOverlapBox OnOverlapChanged;

    // Private fields
    private int _x, _y;
    private float _cellSize;
    private GridBuilder _gridBuilder;
    private Coroutine _recycleCoroutine;
    private bool _isBeingAccessed;
    private bool _isInitialized;

    #region INITIALIZATION
    public void Initialize(
        GridBuilder gridBuilder,
        int x, int y,
        float cellSize,
        string[] overlapTags = null,
        LayerMask overlapLayerMasks = default,
        OnOverlapBox onOverlap = null,
        float lifeTime = 10f)
    {
        _gridBuilder = gridBuilder;
        _x = x;
        _y = y;
        _cellSize = cellSize;
        _collisionTags = overlapTags ?? new string[0];
        _collisionLayers = overlapLayerMasks;

        if (onOverlap != null)
        {
            OnOverlapChanged += onOverlap;
        }

        _isInitialized = true;

        gameObject.SetActive(true);
        transform.position = _gridBuilder.GetWorldPosition(x, y);
        
        CheckOverlap();
        StartRecycleTimer(lifeTime);
        
    }

    public void ResetCell()
    {
        StopRecycleTimer();
        OnOverlapChanged = null;
        _isBeingAccessed = false;
        _isInitialized = false;
        gameObject.SetActive(false);
    }
    #endregion

    #region OVERLAP DETECTION
    public void CheckOverlap()
    {
        if (!_isInitialized) return;

        var hit = Physics2D.OverlapBox(
            transform.position,
            new Vector2(_cellSize, _cellSize),
            0,
            _collisionLayers);

        IsOverlapped = hit != null && _collisionTags.Contains(hit.tag);
        OnOverlapChanged?.Invoke(_x, _y, IsOverlapped);
    }
    #endregion

    #region LIFETIME MANAGEMENT
    public void SetAccessed(bool isBeingAccessed)
    {
        _isBeingAccessed = isBeingAccessed;
        
        if (isBeingAccessed)
        {
            StopRecycleTimer();
        }
        else if (_isInitialized)
        {
            StartRecycleTimer(_gridBuilder.DefaultCellLifetime);
        }
    }

    private void StartRecycleTimer(float lifetime)
    {
        StopRecycleTimer();
        _recycleCoroutine = StartCoroutine(RecycleAfterDelay(lifetime));
    }

    private void StopRecycleTimer()
    {
        if (_recycleCoroutine != null)
        {
            StopCoroutine(_recycleCoroutine);
            _recycleCoroutine = null;
        }
    }

    private IEnumerator RecycleAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (!_isBeingAccessed && _isInitialized)
        {
            _gridBuilder.ReturnCellToPool(this);
        }
    }
    #endregion

    #region DEBUG
    public void SetDebug(bool debug)
    {
        _debug = debug;
    }

    private void OnDrawGizmos()
    {
        if (_debug && _isInitialized)
        {
            Gizmos.color = IsOverlapped ? Color.red : Color.green;
            Gizmos.DrawWireCube(transform.position, new Vector3(_cellSize, _cellSize, 1));
        }
    }
    #endregion
}