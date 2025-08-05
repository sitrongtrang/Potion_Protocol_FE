using UnityEngine;

public class ItemSourceControllerNetwork : NetworkBehaviour
{
    private ItemSourceConfig _config;

    public ItemSourceConfig Config => _config;

    #region Initialization
    public override void Initialize(string entityId, ScriptableObject scriptableObject)
    {
        EntityId = entityId;
        if (scriptableObject is ItemSourceConfig itemSourceConfig)
        {
            _config = itemSourceConfig;
            GetComponent<SpriteRenderer>().sprite = itemSourceConfig.Icon;
        }
    }
    #endregion
}