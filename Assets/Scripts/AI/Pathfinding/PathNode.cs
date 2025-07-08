/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode {

    private Grid<PathNode> _grid;
    public int X;
    public int Y;

    public int GCost;
    public int HCost;
    public int FCost;

    public bool IsWalkable;
    public PathNode CameFromNode;

    public PathNode(Grid<PathNode> grid, int x, int y) {
        this._grid = grid;
        this.X = x;
        this.Y = y;
        IsWalkable = true;
    }

    public void CalculateFCost() {
        FCost = GCost + HCost;
    }

    public void SetIsWalkable(bool isWalkable) {
        this.IsWalkable = isWalkable;
        _grid.TriggerGridObjectChanged(X, Y);
    }

    public override string ToString() {
        return X + "," + Y;
    }

}
