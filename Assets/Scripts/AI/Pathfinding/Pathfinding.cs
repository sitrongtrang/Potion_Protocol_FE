using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour {
    private const int MOVE_STRAIGHT_COST = 10;
    private const int MOVE_DIAGONAL_COST = 14;

    public static Pathfinding Instance { get; private set; }

    private Grid<PathNode> _grid;
    private Heap<PathNode> _openHeap;
    private HashSet<PathNode> _openSet;
    private HashSet<PathNode> _closedSet;
    private Dictionary<(Vector2Int, Vector2Int), List<Vector2>> _pathCache = new Dictionary<(Vector2Int, Vector2Int), List<Vector2>>();

    private void Awake() {
        if (Instance != null && Instance != this) {
            Destroy(this);
            return;
        }
        Instance = this;
    }

    public void InitializeGrid(int width, int height, float cellSize, Vector2 originPosition) {
        _grid = new Grid<PathNode>(width, height, cellSize, originPosition, (g, x, y) => new PathNode(g, x, y));
    }

    public Grid<PathNode> GetGrid() => _grid;

    public void FindPath(Vector2 startWorldPosition, Vector2 endWorldPosition, System.Action<List<Vector2>> callback) {
        StartCoroutine(FindPathCoroutine(startWorldPosition, endWorldPosition, callback));
    }

    private IEnumerator FindPathCoroutine(Vector2 startWorldPosition, Vector2 endWorldPosition, System.Action<List<Vector2>> callback) {
        _grid.GetXY(startWorldPosition, out int startX, out int startY);
        _grid.GetXY(endWorldPosition, out int endX, out int endY);

        var startKey = new Vector2Int(startX, startY);
        var endKey = new Vector2Int(endX, endY);

        // Check cache
        if (_pathCache.TryGetValue((startKey, endKey), out var cachedPath)) {
            callback?.Invoke(cachedPath);
            yield break;
        }

        PathNode startNode = _grid.GetGridObject(startX, startY);
        PathNode endNode = _grid.GetGridObject(endX, endY);

        if (startNode == null || endNode == null || !endNode.IsWalkable) {
            callback?.Invoke(null);
            yield break;
        }

        _openHeap = new Heap<PathNode>(_grid.GetWidth() * _grid.GetHeight());
        _openSet = new HashSet<PathNode>();
        _closedSet = new HashSet<PathNode>();

        _openHeap.Add(startNode);
        _openSet.Add(startNode);

        // Initialize grid
        for (int x = 0; x < _grid.GetWidth(); x++) {
            for (int y = 0; y < _grid.GetHeight(); y++) {
                PathNode node = _grid.GetGridObject(x, y);
                node.GCost = int.MaxValue;
                node.CalculateFCost();
                node.CameFromNode = null;
            }
        }

        startNode.GCost = 0;
        startNode.HCost = CalculateDistanceCost(startNode, endNode);
        startNode.CalculateFCost();

        int nodesPerFrame = Mathf.Max(100, _grid.GetWidth() * _grid.GetHeight() / 10);
        int processedNodes = 0;

        while (_openHeap.Count > 0) {
            PathNode currentNode = _openHeap.RemoveFirst();
            _openSet.Remove(currentNode);

            if (currentNode == endNode) {
                var path = CalculatePath(endNode);
                _pathCache[(startKey, endKey)] = path;
                callback?.Invoke(path);
                yield break;
            }

            _closedSet.Add(currentNode);

            foreach (PathNode neighbourNode in GetNeighbourList(currentNode)) {
                if (_closedSet.Contains(neighbourNode)) continue;
                if (!neighbourNode.IsWalkable) {
                    _closedSet.Add(neighbourNode);
                    continue;
                }

                int tentativeGCost = currentNode.GCost + CalculateDistanceCost(currentNode, neighbourNode);
                if (tentativeGCost < neighbourNode.GCost) {
                    neighbourNode.CameFromNode = currentNode;
                    neighbourNode.GCost = tentativeGCost;
                    neighbourNode.HCost = CalculateDistanceCost(neighbourNode, endNode);
                    neighbourNode.CalculateFCost();

                    if (!_openSet.Contains(neighbourNode)) {
                        _openHeap.Add(neighbourNode);
                        _openSet.Add(neighbourNode);
                    } else {
                        _openHeap.UpdateItem(neighbourNode);
                    }
                }
            }

            processedNodes++;
            if (processedNodes >= nodesPerFrame) {
                processedNodes = 0;
                yield return null;
            }
        }

        callback?.Invoke(null);
    }

    private List<PathNode> GetNeighbourList(PathNode currentNode) {
        List<PathNode> neighbours = new List<PathNode>(8);

        for (int x = -1; x <= 1; x++) {
            for (int y = -1; y <= 1; y++) {
                if (x == 0 && y == 0) continue;

                int checkX = currentNode.X + x;
                int checkY = currentNode.Y + y;

                if (checkX >= 0 && checkX < _grid.GetWidth() && 
                    checkY >= 0 && checkY < _grid.GetHeight()) {
                    neighbours.Add(GetNode(checkX, checkY));
                }
            }
        }

        return neighbours;
    }

    public PathNode GetNode(int x, int y) => _grid.GetGridObject(x, y);

    private List<Vector2> CalculatePath(PathNode endNode) {
        List<PathNode> path = new List<PathNode>();
        path.Add(endNode);
        PathNode currentNode = endNode;

        while (currentNode.CameFromNode != null) {
            path.Add(currentNode.CameFromNode);
            currentNode = currentNode.CameFromNode;
        }

        path.Reverse();

        List<Vector2> vectorPath = new List<Vector2>(path.Count);
        foreach (PathNode node in path) {
            vectorPath.Add(_grid.GetWorldPosition(node.X, node.Y));
        }

        return vectorPath;
    }

    private int CalculateDistanceCost(PathNode a, PathNode b) {
        int dx = Mathf.Abs(a.X - b.X);
        int dy = Mathf.Abs(a.Y - b.Y);
        return MOVE_STRAIGHT_COST * (dx + dy) + (MOVE_DIAGONAL_COST - 2 * MOVE_STRAIGHT_COST) * Mathf.Min(dx, dy);
    }

    public void ClearCache() => _pathCache.Clear();
}