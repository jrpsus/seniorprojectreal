using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupScript : MonoBehaviour
{
    public bool active = true;
    public bool[] subactive;
    public GameObject[] popups;
    public int startingLevel = 1;
    public int startingLives = 5;
    //public ShapeSpawner game;
    public Interface game;
    public TextMeshProUGUI[] text;
    public Slider[] slider;
    public AudioSource audio;
    public AudioClip[] sfx;
    void Start()
    {
        game = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Interface>();
        subactive[0] = true;
        subactive[1] = false;
        subactive[2] = false;
    }
    public void SetStartingLevel()
    {
        if (startingLevel != (int)slider[0].value) audio.PlayOneShot(sfx[(startingLevel < (int)slider[0].value) ? 0 : 1]);
        startingLevel = (int)slider[0].value;
    }
    public void SetStartingLives()
    {
        if (startingLives != (int)slider[1].value) audio.PlayOneShot(sfx[(startingLives < (int)slider[1].value) ? 0 : 1]);
        startingLives = (int)slider[1].value;
    }
    public void ClosePopup()
    {
        if (subactive[0])
        {
            game.level = startingLevel;
            game.lives = startingLives;
            subactive[0] = false;
        }
        if (subactive[1])
        {
            subactive[1] = false;
        }
        audio.PlayOneShot(sfx[3]);
        active = false;
        game.transitionOut2 = true;
        //gameObject.SetActive(false);
    }
    void Update()
    {
        game.transitionScale += ((game.transitionOut2 ? 0 : 1) - game.transitionScale) / 6;
        transform.localScale = new Vector3(game.transitionScale, game.transitionScale, 1);
        text[0].text = "Starting Level: " + startingLevel;
        text[1].text = "Starting Lives: " + startingLives;
        for (int i = 0; i < popups.Length; i++)
        {
            popups[i].SetActive(subactive[i]);
        }
    }
}
