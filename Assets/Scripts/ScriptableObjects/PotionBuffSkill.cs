using UnityEngine;

[CreateAssetMenu(fileName = "BuffPotionSkill", menuName = "Scriptable Objects/BuffPotionSkill")]
public class BuffPotionSkill : Skill
{
    public float buffRatio;
    public override void Activate(GameObject player)
    {
        player.GetComponent<PlayerController>().Interaction.SetPointMultiplier(buffRatio + 1);
        player.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = SkillVFX;
        player.transform.GetChild(1).gameObject.GetComponent<SkillRingController>().enabled = true;
    }
    public override void Deactivate(GameObject player)
    {
        player.GetComponent<PlayerController>().Interaction.SetPointMultiplier(1f / (buffRatio + 1));
        player.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = null;
        player.transform.GetChild(1).gameObject.GetComponent<SkillRingController>().enabled = false;
    }
}