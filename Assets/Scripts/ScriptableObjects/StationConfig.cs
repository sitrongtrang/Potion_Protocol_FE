using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StationConfig", menuName = "Scriptable Objects/StationConfig")]
public class StationConfig : EntityConfig
{
    [SerializeField] private StationType _type;
    public StationType Type => _type;
}