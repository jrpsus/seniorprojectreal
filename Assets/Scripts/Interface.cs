using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Interface : MonoBehaviour
{
    public int score = 0;
    public int level = 1;
    public int lives = 5;
    public int spades = 0;
    public int drills = 0;
    public int nextLevel = 200;
    public float musicVolume = 0.5f;
    public float soundVolume = 0.5f;
    const int levelPercentIncrease = 50;
    public GameObject textObject;
    public GameObject bigTextObject;
    public TextMeshProUGUI[] text;
    public RectTransform[] transition;
    public PopupScript popup;
    public AudioSource audio;
    public AudioClip[] sfx;
    public float transitionPos = 6;
    public float transitionScale = 1;
    public bool transitionOut = true;
    public bool transitionOut2 = false;
    public string nextScene = "Main Menu";
    void Start()
    {
        popup = GameObject.FindGameObjectWithTag("Popup").GetComponent<PopupScript>();
    }
    public void setMusicVolume(float v)
    {
        if (musicVolume != (float)(v / 100f)) audio.PlayOneShot(popup.sfx[(musicVolume < (float)(v / 100f)) ? 0 : 1]);
        musicVolume = (float)(v / 100f);
        popup.text[2].text = "Music Volume: " + Mathf.Round(v);
    }
    public void setSoundVolume(float v)
    {
        if (soundVolume != (float)(v / 100f)) audio.PlayOneShot(popup.sfx[(soundVolume < (float)(v / 100f)) ? 0 : 1]);
        soundVolume = (float)(v / 100f);
        popup.text[3].text = "Sound Volume: " + Mathf.Round(v);
    }
    public void SpawnText(Color color, string text, Vector3 pos)
    {
        GameObject thisText = Instantiate(textObject, pos, Quaternion.identity, gameObject.transform);
        thisText.GetComponent<TextMeshProUGUI>().text = text;
        thisText.GetComponent<TextMeshProUGUI>().color = color;
    }
    public void SpawnBigText(Color color, string text, Vector3 pos)
    {
        GameObject thisText = Instantiate(bigTextObject, pos, Quaternion.identity, gameObject.transform);
        thisText.GetComponent<TextMeshProUGUI>().text = text;
        thisText.GetComponent<TextMeshProUGUI>().color = color;
    }
    // Update is called once per frame
    void Update()
    {
        if (transitionOut) transitionPos += 5f * Time.unscaledDeltaTime;
        else transitionPos -= 5f * Time.unscaledDeltaTime;

        transition[0].position = new Vector3(960, 540 + Mathf.Pow(transitionPos, 3), 0);
        transition[1].position = new Vector3(960, 540 - Mathf.Pow(transitionPos, 3), 0);

        if (transitionPos < 5 && !transitionOut) SceneManager.LoadScene(nextScene);

        text[0].text = score.ToString();
        text[1].text = level.ToString();
        text[2].text = lives.ToString();
        text[3].text = spades.ToString();
        text[4].text = drills.ToString();

        if (score >= nextLevel)
        {
            level++;
            audio.PlayOneShot(sfx[0]);
            SpawnText(new Color(0.1f, 0.2f, 0.9f), "+1 level", text[1].rectTransform.position + Vector3.left * 250);
            nextLevel *= (100 + levelPercentIncrease);
            nextLevel /= 100;
            if (level % 3 == 0)
            {

            }
            if (level % 5 == 0)
            {
                lives++;
                SpawnText(new Color(0.9f, 0.2f, 0.2f), "+1 life", text[2].rectTransform.position + Vector3.left * 250);
            }
        }
        if (lives < 0)
        {
            lives = 0;
        }
    }
}
