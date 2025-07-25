using UnityEngine;

[CreateAssetMenu(fileName = "BuffPotionSkill", menuName = "Scriptable Objects/BuffPotionSkill")]
public abstract class BuffPotionSkill : Skill
{
    public float buffRatio;
    public override void Activate(GameObject player)
    {
        player.GetComponent<PlayerController>().Movement.SetSpeedMultiplier(buffRatio + 1);
    }
}