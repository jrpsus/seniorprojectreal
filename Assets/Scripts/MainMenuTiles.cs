using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuTiles : MonoBehaviour
{
    public int menu = 1;
    public GameObject[] menus;
    public GameObject grid;
    public RectTransform[] transition;
    public float transitionPos = 5;
    public bool transitionOut = true;
    public string nextScene = "Game";
    public AudioSource audio;
    public AudioClip[] sfx;
    //public Animation anim;
    void Start()
    {
        audio.Play();
        Time.timeScale = 1.0f;
    }
    public void PlayGame()
    {
        transitionPos = 12;
        transitionOut = false;
        //SceneManager.LoadScene("Game");
    }
    public void LoadMenu(int m)
    {
        menu = m;
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    
    void Update()
    {
        if (transitionOut) transitionPos += 5f * Time.unscaledDeltaTime;
        else transitionPos -= 5f * Time.unscaledDeltaTime;

        transition[0].position = new Vector3(960, 540 + Mathf.Pow(transitionPos, 3), 0);
        transition[1].position = new Vector3(960, 540 - Mathf.Pow(transitionPos, 3), 0);

        if (transitionPos < 5 && !transitionOut) SceneManager.LoadScene(nextScene);

        for (int i = 0; i < menus.Length; i++)
        {
            menus[i].SetActive(false);
        }
        menus[menu - 1].SetActive(true);
        grid.SetActive(menu == 1);
    }
}
