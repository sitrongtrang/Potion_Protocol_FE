using System.Linq;
using UnityEngine;

public class GridCellObject : MonoBehaviour
{
    [Header("Component")]
    private bool _debug = false;
    private bool _isOverlaped = false;
    public delegate void OnOverlapBox(int x, int y, bool overlaped);
    private event OnOverlapBox OverlapBox;
    [Header("Non Walkable Tags / Layers")]
    [SerializeField] private LayerMask _nonWalkableLayers;
    [SerializeField] private string[] _nonWalkableTags;
    [Header("Node Preference")]
    private int _x;
    private int _y;
    private float _cellSize;
    public void Initialize(
        int x,
        int y,
        float cellSize,
        string[] overlapTags = null,
        LayerMask overlapLayerMasks = default,
        OnOverlapBox overlaped = null)
    {
        _x = x;
        _y = y;
        _cellSize = cellSize;
        OverlapBox = overlaped;
        _nonWalkableTags = overlapTags ?? _nonWalkableTags;
        _nonWalkableLayers = overlapLayerMasks != default ? overlapLayerMasks : _nonWalkableLayers;

        HandleOverlap();
    }

    public void HandleOverlap()
    {
        Collider2D hit = Physics2D.OverlapBox(
            transform.position,
            new(_cellSize, _cellSize),
            0,
            _nonWalkableLayers);

        _isOverlaped = !(hit == null || !_nonWalkableTags.Contains(hit.tag));
    
        OverlapBox?.Invoke(_x, _y, _isOverlaped);
    }

    public void SetDebug(bool debug)
    {
        _debug = debug;
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