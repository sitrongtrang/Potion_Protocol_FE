using System;
using System.Collections;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private PlayerAttribute _playerAttribute; // player attribute from data asset
    // bool _isAttacking = false;
    bool _canAttack = true;
    bool[] _canUseSkills = new bool[3];
    bool _isInAction = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            _canUseSkills[i] = true;
        }
    }

    // Update is called once per frame
    public void MyUpdate()
    {
        
        if (!_isInAction)
        {
            
            if (_canUseSkills[0] && Input.GetKeyDown(KeyCode.U))
            {
                StartCoroutine(UseSkill(1));
            }
            if (_canUseSkills[1] && Input.GetKeyDown(KeyCode.I))
            {
                StartCoroutine(UseSkill(2));
            }
            if (_canUseSkills[2] && Input.GetKeyDown(KeyCode.O))
            {
                StartCoroutine(UseSkill(3));
            }
        }
    }
    
    // Can not perform any action when an action is active by variable _isInAction
    public IEnumerator Attack()
    {
        if (_canAttack && !_isInAction && Input.GetKeyDown(KeyCode.J))
        {
            _canAttack = false;
            // _isInAction = true;
            Debug.Log("Player Attacked");
            yield return new WaitForSeconds(_playerAttribute.AttackCooldown);
            // _isInAction = false;
            _canAttack = true;
        }
        
    }
    
    IEnumerator UseSkill(int skillNumber)
    {
        _canUseSkills[skillNumber - 1] = false;
        // _isInAction = true;
        Debug.Log($"Using ability {skillNumber}");
        yield return new WaitForSeconds(_playerAttribute.SkillsCoolDown[skillNumber - 1]);
        // _isInAction = false;
        _canUseSkills[skillNumber - 1] = true; ;
    }
}
