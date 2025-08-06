using TMPro;
using UnityEngine;

public class SwitchPage : MonoBehaviour
{
    [SerializeField] int _changing = 0;
    [SerializeField] TMP_Text _paging;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void OnButtonClicked()
    {
        int currentPage = int.Parse(_paging.text.Substring(0, _paging.text.IndexOf("/")));
        int limitPage = int.Parse(_paging.text.Substring(_paging.text.IndexOf("/")));
        currentPage += _changing;
        if (currentPage <= limitPage && currentPage > 0)
        {
            _paging.text = currentPage.ToString() + _paging.text.Substring(_paging.text.IndexOf("/"));
            GameManager.Instance.LoadFriendList(currentPage);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
