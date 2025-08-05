using UnityEngine;

[CreateAssetMenu(fileName = "SkillConfig", menuName = "Scriptable Objects/SkillConfig")]
public abstract class SkillConfig : ScriptableObject
{
    [SerializeField] private string _skillName;
    public string SkillName => _skillName;
    [SerializeField] private Sprite _skillIcon;
    public Sprite SkillIcon => _skillIcon;
    [SerializeField] private Sprite _skillVFX;
    public Sprite SkillVFX => SkillVFX;
    [SerializeField] private float _timeAlive;
    public float TimeAlive => _timeAlive;
    [SerializeField] private float _cooldown;
    public float Cooldown => _cooldown;

    public abstract void Activate(PlayerController player);
    public abstract void Deactivate(PlayerController player);
}
