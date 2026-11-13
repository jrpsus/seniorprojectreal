using UnityEngine;

public class ParticleScript : MonoBehaviour
{
    public ParticleSystem particle;
    public float time;
    public float timeLimit;
    public float speed = 10f;
    public Vector3 destination;
    void Start()
    {
        particle.Play();
    }

    [System.Obsolete]
    void Update()
    {
        time += Time.unscaledDeltaTime;
        if (destination == Vector3.zero)
        {
            if (time > timeLimit)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * time * Time.unscaledDeltaTime);
            if (Vector3.Distance(transform.position, destination) <= 0.5f)
            {
                Destroy(gameObject);
            }
        }
    }
}
