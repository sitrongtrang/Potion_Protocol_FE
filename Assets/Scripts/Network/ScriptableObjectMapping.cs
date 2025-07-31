using System;
using UnityEngine;

[CreateAssetMenu(fileName ="ScriptableObjectMapping", menuName ="Scriptable Objects/ScriptableObjectMapping")]
public class ScriptableObjectMapping : ScriptableObject
{
    [Serializable]
    public class SOMap
    {
        public string Id;
        public ScriptableObject SO;
    }

    [SerializeField] private SOMap[] _enemyMap;
    [SerializeField] private SOMap[] _itemSourceMap;
    [SerializeField] private SOMap[] _itemMap;
    [SerializeField] private SOMap[] _recipeMap;
    [SerializeField] private SOMap[] _stationMap;
}