using UnityEngine;

[CreateAssetMenu(fileName = "BuffDamageSkill", menuName = "Scriptable Objects/BuffDamageSkill")]
public abstract class BuffDamageSkill : Skill
{
    public float buffRatio;
    public override void Activate(GameObject player)
    {
        player.GetComponent<PlayerController>().Attack.SetDamageMultiplier(buffRatio + 1);
    }
}