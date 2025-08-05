using UnityEngine;

[CreateAssetMenu(fileName = "DamageBuffSkillConfig", menuName = "Scriptable Objects/DamageBuffSkillConfig")]
public class DamageBuffSkillConfig : SkillConfig
{
    [SerializeField] private float _buffMultiplier;
    public float BuffMultiplier => _buffMultiplier;

    public override void Activate(PlayerController player)
    {
        player.Attack.DamageMultiplier *= _buffMultiplier;
        SkillRingController skillRing = player.GetComponentInChildren<SkillRingController>();
        skillRing.GetComponent<SpriteRenderer>().sprite = SkillVFX;
        skillRing.enabled = true;
    }

    public override void Deactivate(PlayerController player)
    {
        player.Attack.DamageMultiplier /= _buffMultiplier;
        SkillRingController skillRing = player.GetComponentInChildren<SkillRingController>();
        skillRing.GetComponent<SpriteRenderer>().sprite = null;
        skillRing.enabled = false;
    }
}