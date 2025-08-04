using UnityEngine;

[CreateAssetMenu(fileName = "ScoreBuffSkillConfig", menuName = "Scriptable Objects/ScoreBuffSkillConfig")]
public class ScoreBuffSkillConfig : SkillConfig
{
    [SerializeField] private float _buffMultiplier;
    public float BuffMultiplier => _buffMultiplier;

    public override void Activate(PlayerController player)
    {
        player.Interaction.ScoreMultiplier /= _buffMultiplier;
        SkillRingController skillRing = player.GetComponentInChildren<SkillRingController>();
        skillRing.GetComponent<SpriteRenderer>().sprite = null;
        skillRing.enabled = false;
    }
    
    public override void Deactivate(PlayerController player)
    {
        player.Interaction.ScoreMultiplier /= _buffMultiplier;
        SkillRingController skillRing = player.GetComponentInChildren<SkillRingController>();
        skillRing.GetComponent<SpriteRenderer>().sprite = null;
        skillRing.enabled = false;
    }
}