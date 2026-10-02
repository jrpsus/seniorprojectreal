using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Interface : MonoBehaviour
{
    public int score = 0;
    public int level = 1;
    public int lives = 5;
    public int nextLevel = 200;
    const int levelPercentIncrease = 50;
    public TextMeshProUGUI[] text;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        text[0].text = score.ToString();
        text[1].text = level.ToString();
        text[2].text = lives.ToString();

        if (score >= nextLevel)
        {
            level++;
            nextLevel *= (100 + levelPercentIncrease);
            nextLevel /= 100;
            if (level % 10 == 0) lives++;
        }
        if (lives < 0)
        {
            lives = 0;
        }
    }
}
