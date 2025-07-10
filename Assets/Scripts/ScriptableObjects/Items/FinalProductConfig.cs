using UnityEngine;

[CreateAssetMenu(fileName = "FinalProductConfig", menuName = "Scriptable Objects/Items/FinalProductConfig")]
public class FinalProductConfig : ItemConfig
{
    [SerializeField] private int _score;
    public int Score => _score;

    [SerializeField] private float _expireTime;
    public float ExpireTime => _expireTime;
}
