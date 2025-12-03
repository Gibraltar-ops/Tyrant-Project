
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChangeButton : MonoBehaviour
{
    public string SceneName;
    
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(ChangeScene);
    }

    public void ChangeScene()
    {
        SceneManager.LoadScene(SceneName);
    }
}
