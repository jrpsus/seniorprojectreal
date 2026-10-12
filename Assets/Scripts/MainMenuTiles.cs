using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuTiles : MonoBehaviour
{
    void Start()
    {
        
    }
    public void PlayGame()
    {
        SceneManager.LoadScene("Game");
    }
    public void Help()
    {

    }
    public void QuitGame()
    {
        Application.Quit();
    }
    void Update()
    {
        
    }
}
