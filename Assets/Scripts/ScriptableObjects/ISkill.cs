using UnityEngine;

[CreateAssetMenu(fileName = "ISkill", menuName = "Scriptable Objects/ISkill")]
public abstract class Skill : ScriptableObject
{
    public string SkillName;
    public Sprite SkillIcon;
    public Sprite SkillVFX;
    public float TimeAlive;
    public abstract void Activate(GameObject user);
    public abstract void Deactivate(GameObject user);
}
