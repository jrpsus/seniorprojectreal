using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    public ParticleSystem particle;
    public float time;
    public float timeLimit;
    void Start()
    {
        particle.Play();
    }

    [System.Obsolete]
    void Update()
    {
        time += Time.unscaledDeltaTime;
        if (time > timeLimit)
        {
            Destroy(gameObject);
        }
    }
}
