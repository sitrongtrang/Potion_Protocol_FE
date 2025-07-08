using UnityEngine;

[CreateAssetMenu(fileName = "ProductConfig", menuName = "Scriptable Objects/ProductConfig")]
public class ProductConfig : ItemConfig
{
    [SerializeField] private float _score;
    public float Score => _score;
}
