using Unity.VisualScripting;
using UnityEngine;

public class FurnaceController : StationController
{
    public override void AddItem(ItemConfig config)
    {
        base.AddItem(config);
        StartCrafting();
    }
}