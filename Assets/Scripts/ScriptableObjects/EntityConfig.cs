using UnityEngine;

public abstract class EntityConfig : ScriptableObject
{
    [SerializeField] private string _name;
    public string Name => _name;
    [SerializeField] private Sprite _icon;
    public Sprite Icon => _icon;
    [SerializeField] private RuntimeAnimatorController _anim;
    public RuntimeAnimatorController Anim => _anim;
    [SerializeField] private string _id;
    public string Id => _id;
}