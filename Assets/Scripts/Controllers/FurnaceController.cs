using UnityEngine;

public class FurnaceController : StationController
{
    public override bool AddItem(ItemConfig config)
    {
        bool added = base.AddItem(config);
        if (added) StartCrafting();
        return added;
    }
}