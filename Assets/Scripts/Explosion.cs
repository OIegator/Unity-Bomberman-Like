using UnityEngine;

public class Explosion : MonoBehaviour, IPooledObject
{
    public ObjectType ObjectType => ObjectType.Explosion;
    public float delay;
    private float _counter;
    [SerializeField] private ParticleSystem _particleSystem;

    void Start()
    {
        _counter = delay;
    }


    void Update()
    {
        if (_counter > 0)
        {
            _counter -= Time.deltaTime;
        }
        else
        {
            _counter = delay;
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
            if (delay - _counter < 0.2f)
            {
                other.GetComponent<Bomberman>().Die();
            }
        }
    }
    
    private void OnDisable()
    {
        _particleSystem.Stop();
    }
}