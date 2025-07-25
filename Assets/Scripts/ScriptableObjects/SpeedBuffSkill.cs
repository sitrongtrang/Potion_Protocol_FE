using UnityEngine;

[CreateAssetMenu(fileName = "BuffSpeedSkill", menuName = "Scriptable Objects/BuffSpeedSkill")]
public abstract class BuffSpeedSkill : Skill
{
    public float buffRatio;
    public override void Activate(GameObject player)
    {
        player.GetComponent<PlayerController>().Movement.SetSpeedMultiplier(buffRatio + 1);
    }
}