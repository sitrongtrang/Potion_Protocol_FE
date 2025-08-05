using UnityEngine;

[CreateAssetMenu(fileName = "SpeedBuffSkillConfig", menuName = "Scriptable Objects/SpeedBuffSkillConfig")]
public class SpeedBuffSkillConfig : SkillConfig
{
    [SerializeField] private float _buffMultiplier;
    public float BuffMultiplier => _buffMultiplier;

    public override void Activate(PlayerController player)
    {
        player.Movement.SpeedMultiplier *= _buffMultiplier;
        SkillRingController skillRing = player.GetComponentInChildren<SkillRingController>();
        skillRing.GetComponent<SpriteRenderer>().sprite = null;
        skillRing.enabled = false;
    }
    public override void Deactivate(PlayerController player)
    {
        player.Movement.SpeedMultiplier /= _buffMultiplier;
        SkillRingController skillRing = player.GetComponentInChildren<SkillRingController>();
        skillRing.GetComponent<SpriteRenderer>().sprite = null;
        skillRing.enabled = false;
    }
}