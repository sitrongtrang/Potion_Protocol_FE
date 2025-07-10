using UnityEngine;

[CreateAssetMenu(fileName = "IntermediateProductConfig", menuName = "Scriptable Objects/Items/IntermediateProductConfig")]
public class IntermediateProductConfig : ItemConfig
{
    [SerializeField] private float _expireTime;
    public float ExpireTime => _expireTime;
}
