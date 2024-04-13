using UnityEngine;

public class Explosion : MonoBehaviour, IPooledObject
{
    public ObjectType ObjectType => ObjectType.Explosion;
    public float delay;
    public float counter;
    [SerializeField] private ParticleSystem _particleSystem;

    void Start()
    {
        counter = delay;
    }


    void Update()
    {
        if (counter > 0)
        {
            counter -= Time.deltaTime;
        }
        else
        {
            counter = delay;
            DestroyObject();
        }
    }

    private void DestroyObject()
    {
        transform.position = ObjectPoolManager.Instance.transform.position;
        ObjectPoolManager.Instance.DestroyObject(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Brick"))
        {
            other.gameObject.GetComponent<Box>().node.SetState(State.Accessible);
            ObjectPoolManager.Instance.DestroyObject(other.gameObject);
        }

        if (other.gameObject.CompareTag("Player"))
        {
            if (delay - counter < 0.2f)
            {
                other.GetComponent<Bomberman>().Die();
            }
        }
        
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (delay - counter < 0.2f)
            {
                other.GetComponent<Enemy>().Die();
            }
        }
    }
    
    private void OnDisable()
    {
        _particleSystem.Stop();
    }
}