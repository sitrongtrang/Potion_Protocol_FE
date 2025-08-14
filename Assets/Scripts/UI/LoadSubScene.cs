using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSubScene : MonoBehaviour
{
    [SerializeField] string _sceneName = "FriendListScene";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void OnClicked()
    {
        SceneManager.LoadScene(_sceneName, LoadSceneMode.Additive);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
