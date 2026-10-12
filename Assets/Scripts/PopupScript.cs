using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupScript : MonoBehaviour
{
    public bool active = true;
    public bool[] subactive;
    public int startingLevel = 1;
    public int startingLives = 5;
    //public ShapeSpawner game;
    public Interface game;
    public TextMeshProUGUI[] text;
    public Slider[] slider;
    void Start()
    {
        game = GameObject.FindGameObjectWithTag("Canvas").GetComponent<Interface>();
        subactive[0] = true;
        subactive[1] = false;
        subactive[2] = false;
    }
    public void SetStartingLevel()
    {
        startingLevel = (int)slider[0].value;
    }
    public void SetStartingLives()
    {
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
        active = false;
        gameObject.SetActive(false);
    }
    void Update()
    {
        text[0].text = "Starting Level: " + startingLevel;
        text[1].text = "Starting Lives: " + startingLives;
    }
}
