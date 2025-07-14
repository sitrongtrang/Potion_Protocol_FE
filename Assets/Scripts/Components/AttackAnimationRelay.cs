using UnityEngine;

public class AttackAnimationRelay : MonoBehaviour
{
    private PlayerAttack _playerAttack;

    public void Initialize(PlayerController player)
    {
        _playerAttack = player.Attack;
    }

    // This method is called by the Animation Event
    public void FinishAttack()
    {
        _playerAttack?.FinishAttack();
    }
}
