using TMPro;
using UnityEngine;

public class TextParticleScript : MonoBehaviour
{
    public float velocity = 45.0f;
    public float alphaVelocity = 1.0f;
    public TextMeshProUGUI text;
    void Start()
    {
        text.alpha = 1.0f;
    }

    // Update is called once per frame
    void Update()
    {
        text.alpha -= alphaVelocity * Time.unscaledDeltaTime;
        transform.Translate(Vector3.up * velocity * Time.unscaledDeltaTime);
        velocity = Mathf.MoveTowards(velocity, 0f, 15.0f * Time.unscaledDeltaTime);
        if (text.alpha <= 0f) Destroy(gameObject);
    }
}
