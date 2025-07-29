using UnityEngine;

[CreateAssetMenu(fileName = "BuffDamageSkill", menuName = "Scriptable Objects/BuffDamageSkill")]
public class BuffDamageSkill : Skill
{
    public float buffRatio;
    public override void Activate(GameObject player)
    {
        player.GetComponent<PlayerController>().Attack.SetDamageMultiplier(buffRatio + 1);
        player.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = SkillVFX;
        player.transform.GetChild(1).gameObject.GetComponent<SkillRingController>().enabled = true;
    }
    public override void Deactivate(GameObject player)
    {
        player.GetComponent<PlayerController>().Attack.SetDamageMultiplier(1f / (buffRatio + 1));
        player.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().sprite = null;
        player.transform.GetChild(1).gameObject.GetComponent<SkillRingController>().enabled = false;
    }
}