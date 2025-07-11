using System.Collections;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class GridCellObject : MonoBehaviour
{
    [Header("Attribute")]
    private bool _debug = false;
    private bool _isOverlaped = false;
    public bool IsOverlaped => _isOverlaped;
    public delegate void OnOverlapBox(int x, int y, bool overlaped);
    private float _lifeTimeLeft = 0.1f;
    private event OnOverlapBox OverlapBox;
    [Header("Non Walkable Tags / Layers")]
    [SerializeField] private LayerMask _collisionLayers;
    [SerializeField] private string[] _collisionTags;
    [Header("Node Preference")]
    private int _x;
    private int _y;
    private float _cellSize;
    private GridBuilder _gridBuilder;
    public void Initialize(
        GridBuilder gridBuilder,
        int x,
        int y,
        float cellSize,
        string[] overlapTags = null,
        LayerMask overlapLayerMasks = default,
        OnOverlapBox overlaped = null,
        float lifeTime = 0.1f
    )
    {
        _gridBuilder = gridBuilder;
        _x = x;
        _y = y;
        _cellSize = cellSize;
        OverlapBox = overlaped;
        _collisionTags = overlapTags ?? new string[0];
        _collisionLayers = overlapLayerMasks != default ? overlapLayerMasks : _collisionLayers;
        _lifeTimeLeft = lifeTime;

        HandleOverlap();
        StartCoroutine(KillSelfDelay());
    }

    public void HandleOverlap()
    {
        Collider2D hit = Physics2D.OverlapBox(
            transform.position,
            new(_cellSize, _cellSize),
            0,
            _collisionLayers);

        _isOverlaped = !(hit == null || !_collisionTags.Contains(hit.tag));
    
        OverlapBox?.Invoke(_x, _y, _isOverlaped);
    }

    public void RegisterOnOverlapBox(OnOverlapBox func)
    {
        OverlapBox += func;
    }

    public void SetDebug(bool debug)
    {
        _debug = debug;
    }

    IEnumerator KillSelfDelay()
    {
        yield return new WaitForSeconds(_lifeTimeLeft);
        _gridBuilder.OnDebugChanged -= SetDebug;
        _gridBuilder.SetCellNull(_x, _y);
        Destroy(gameObject);
    }
    
    void OnDrawGizmos() {
        if (_debug)
        {
            if (_isOverlaped)
                Gizmos.color = Color.red;
            else
                Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position, new(_cellSize, _cellSize));
        }
    }
}