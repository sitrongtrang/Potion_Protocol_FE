public class PathNode : IHeapItem<PathNode> {
    public int X { get; private set; }
    public int Y { get; private set; }
    public int GCost { get; set; }
    public int HCost { get; set; }
    public int FCost { get; private set; }
    public bool IsWalkable { get; set; } = true;
    public PathNode CameFromNode { get; set; }
    public int HeapIndex { get; set; }

    private Grid<PathNode> _grid;

    public PathNode(Grid<PathNode> grid, int x, int y) {
        _grid = grid;
        X = x;
        Y = y;
    }

    public void CalculateFCost() => FCost = GCost + HCost;

    public int CompareTo(PathNode other) {
        int compare = FCost.CompareTo(other.FCost);
        return compare == 0 ? HCost.CompareTo(other.HCost) : -compare;
    }

    public override string ToString() => $"{X},{Y}";
}