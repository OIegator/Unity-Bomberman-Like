using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour, IPooledObject
{
    public ObjectType ObjectType => ObjectType.Bomb;
    public float delay;
    public Node node;
    private float _counter;
    private bool _evaluated;
    private bool _canTick;
    public float boomRange;

    public LayerMask wallLayer;
    public LayerMask blowableLayer;

    public List<Vector3> toBlowR;
    public List<Vector3> toBlowL;
    public List<Vector3> toBlowU;
    public List<Vector3> toBlowD;


    void OnEnable()
    {
        _canTick = true;
        _evaluated = false;
        _counter = delay;
        toBlowR = new List<Vector3>();
        toBlowL = new List<Vector3>();
        toBlowU = new List<Vector3>();
        toBlowD = new List<Vector3>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Boom"))
        {
            Blow();
        }
    }

    void Update()
    {
        if (_counter > 0)
        {
            if (_canTick) _counter -= Time.deltaTime;
        }
        else
        {
            Blow();
            CameraShake.Instance.ShakeCamera(0.1f);
        }
    }

    private static void CreateExplosion(Vector3 position, Quaternion rotation)
    {
        GameObject obj = ObjectPoolManager.Instance.GetObject(ObjectType.Explosion);
        obj.transform.position = position;
        obj.transform.rotation = rotation;
    }

    private void Blow()
    {
        EvaluateBoomRanges();
        CreateExplosion(transform.position, transform.rotation);

        if (toBlowL.Count > 0)
            foreach (var position in toBlowL)
            {
                CreateExplosion(position, transform.rotation);
            }

        if (toBlowR.Count > 0)
            foreach (var position in toBlowR)
            {
                CreateExplosion(position, transform.rotation);
            }


        if (toBlowU.Count > 0)
            foreach (var position in toBlowU)
            {
                CreateExplosion(position, transform.rotation);
            }


        if (toBlowD.Count > 0)
            foreach (var position in toBlowD)
            {
                CreateExplosion(position, transform.rotation);
            }

        node.SetState(State.Accessible);
        ObjectPoolManager.Instance.DestroyObject(gameObject);
    }

    private void EvaluateBoomRanges()
    {
        if (_evaluated) return;
        // L
        for (float i = 1; i <= boomRange; i++)
        {
            if (Physics.Raycast(transform.position, Vector3.left, i, wallLayer))
            {
                break;
            }

            if (Physics.Raycast(transform.position, Vector3.left, i, blowableLayer))
            {
                toBlowL.Add(new Vector3(transform.position.x - i, transform.position.y, transform.position.z));
                break;
            }

            toBlowL.Add(new Vector3(transform.position.x - i, transform.position.y, transform.position.z));
        }

        // R
        for (float i = 1; i <= boomRange; i++)
        {
            if (Physics.Raycast(transform.position, Vector3.right, i, wallLayer))
            {
                break;
            }

            if (Physics.Raycast(transform.position, Vector3.right, i, blowableLayer))
            {
                toBlowR.Add(new Vector3(transform.position.x + i, transform.position.y, transform.position.z));
                break;
            }

            toBlowR.Add(new Vector3(transform.position.x + i, transform.position.y, transform.position.z));
        }

        // U
        for (float i = 1; i <= boomRange; i++)
        {
            if (Physics.Raycast(transform.position, Vector3.forward, i, wallLayer))
            {
                break;
            }

            if (Physics.Raycast(transform.position, Vector3.forward, i, blowableLayer))
            {
                toBlowU.Add(new Vector3(transform.position.x, transform.position.y, transform.position.z + i));
                break;
            }

            toBlowU.Add(new Vector3(transform.position.x, transform.position.y, transform.position.z + i));
        }

        // D
        for (float i = 1; i <= boomRange; i++)
        {
            if (Physics.Raycast(transform.position, Vector3.back, i, wallLayer))
            {
                break;
            }

            if (Physics.Raycast(transform.position, Vector3.back, i, blowableLayer))
            {
                toBlowD.Add(new Vector3(transform.position.x, transform.position.y, transform.position.z - i));
                break;
            }

            toBlowD.Add(new Vector3(transform.position.x, transform.position.y, transform.position.z - i));
        }

        _evaluated = true;
    }
}