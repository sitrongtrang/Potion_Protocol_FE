using UnityEngine;

[CreateAssetMenu(fileName = "BuffSpeedSkill", menuName = "Scriptable Objects/BuffSpeedSkill")]
public class BuffSpeedSkill : Skill
{
    public float buffRatio;
    public override void Activate(GameObject player)
    {
        Debug.Log(player);
        player.GetComponent<PlayerController>().Movement.SetSpeedMultiplier(buffRatio + 1);
        player.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = SkillVFX;
        player.transform.GetChild(1).gameObject.GetComponent<SkillRingController>().enabled = true;
    }
    public override void Deactivate(GameObject player)
    {
        player.GetComponent<PlayerController>().Movement.SetSpeedMultiplier(1f / (buffRatio + 1));
        player.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = null;
        player.transform.GetChild(1).gameObject.GetComponent<SkillRingController>().enabled = false;
    }
}