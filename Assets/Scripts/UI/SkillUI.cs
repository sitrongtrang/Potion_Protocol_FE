using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    [SerializeField] GameObject _skillUIPrefab;
    [SerializeField] List<GameObject> _skillUI = new List<GameObject>(GameConstants.NumSkills);
    float[] _timeRemaining = new float[GameConstants.NumSkills];
    float[] _maxCD = new float[GameConstants.NumSkills];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.InitSkillUI += Initialize;
        GameManager.Instance.StartCoolDown += StartCountDown;
    }
    void OnDisable()
    {
        GameManager.Instance.InitSkillUI -= Initialize;
        GameManager.Instance.StartCoolDown -= StartCountDown;
    }
    public void Initialize(Sprite skillIcon, float maxCooldown)
    {
        GameObject skillUI = Instantiate(_skillUIPrefab, transform);
        _skillUI.Add(skillUI);
        _skillUI[_skillUI.Count - 1].transform.GetChild(0).GetComponent<Image>().sprite = skillIcon;
        _skillUI[_skillUI.Count - 1].transform.GetChild(1).gameObject.SetActive(false); // panel
        _skillUI[_skillUI.Count - 1].transform.GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().text = maxCooldown.ToString();
        _timeRemaining[_skillUI.Count - 1] = maxCooldown;
        _maxCD[_skillUI.Count - 1] = maxCooldown;
    }
    IEnumerator StartCountDown(int skillNumber)
    {
        _skillUI[skillNumber].transform.GetChild(1).gameObject.SetActive(true);
        while (_timeRemaining[skillNumber] > 0)
        {
            _skillUI[skillNumber].transform.GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().text = _timeRemaining[skillNumber].ToString();
            yield return new WaitForSeconds(1);
            _timeRemaining[skillNumber]--;

        }
        _skillUI[skillNumber].transform.GetChild(1).gameObject.SetActive(false);
        _timeRemaining[skillNumber] = _maxCD[skillNumber];
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
