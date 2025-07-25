using UnityEngine;

[CreateAssetMenu(fileName = "ISkill", menuName = "Scriptable Objects/ISkill")]
public abstract class Skill : ScriptableObject
{
    public string skillName;
    public float cooldown;
    public float skillIcon;
    public float skillVFX;
    public abstract void Activate(GameObject user);
}
