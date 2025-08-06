using UnityEngine;

public class ItemControllerNetwork : NetworkBehaviour
{
    private ItemConfig _config;

    public ItemConfig Config => _config;

    #region Initialization
    public override void Initialize(string entityId, ScriptableObject scriptableObject)
    {
        EntityId = entityId;
        if (scriptableObject is ItemConfig itemConfig)
        {
            _config = itemConfig;
            GetComponent<SpriteRenderer>().sprite = itemConfig.Icon;
        }
    }
    #endregion
}