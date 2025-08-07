using TMPro;
using UnityEngine;

public class SwitchPage : MonoBehaviour
{
    [SerializeField] int _changing = 0;
    [SerializeField] TMP_Text[] _paging;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void OnButtonClicked()
    {
        TMP_Text currentPaging = null;
        for (int i = 0; i < _paging.Length; i++)
        {
            if (_paging[i].enabled) currentPaging = _paging[i];
        }
        int currentPage = int.Parse(currentPaging.text.Substring(0, currentPaging.text.IndexOf("/")));
        int limitPage = int.Parse(currentPaging.text.Substring(currentPaging.text.IndexOf("/")));
        currentPage += _changing;
        if (currentPage <= limitPage && currentPage > 0)
        {
            currentPaging.text = currentPage.ToString() + currentPaging.text.Substring(currentPaging.text.IndexOf("/"));
            GameManager.Instance.LoadFriendList(FriendViewMode.FriendList, currentPage);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
