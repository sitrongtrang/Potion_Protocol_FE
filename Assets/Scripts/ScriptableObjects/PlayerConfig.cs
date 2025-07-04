using UnityEngine;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "Scriptable Objects/PlayerConfig")]
public class PlayerConfig : ScriptableObject
{
    [field: SerializeField] public float MoveSpeed { get; private set; }
    [field: SerializeField] public float DashCooldown { get; private set; }
    [field: SerializeField] public float DashSpeed { get; private set; }
    [field: SerializeField] public float DashTime { get; private set; }
    [field: SerializeField] public float AttackCooldown { get; private set; }
    [field: SerializeField] public float[] SkillsCoolDown { get; private set; }
    
}
