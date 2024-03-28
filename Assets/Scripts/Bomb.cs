using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour, IPooledObject
{
    public virtual ObjectType ObjectType => ObjectType.Bomb;
    public GameObject boom;
    public float delay;
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


    // Start is called before the first frame update
    void Start()
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

    void Blow()
    {
        EvaluateBoomRanges();
        Instantiate(boom, transform.position, transform.rotation);
        if (toBlowL.Count > 0)
            for (int i = 0; i < toBlowL.Count; i++)
            {
                if (i == toBlowL.Count - 1) Instantiate(boom, toBlowL[i], transform.rotation);
                else Instantiate(boom, toBlowL[i], transform.rotation);
            }

        //R
        if (toBlowR.Count > 0)
            for (int i = 0; i < toBlowR.Count; i++)
            {
                if (i == toBlowR.Count - 1) Instantiate(boom, toBlowR[i], transform.rotation);
                else Instantiate(boom, toBlowR[i], transform.rotation);
            }

        //U
        if (toBlowU.Count > 0)
            for (int i = 0; i < toBlowU.Count; i++)
            {
                if (i == toBlowU.Count - 1) Instantiate(boom, toBlowU[i], transform.rotation);
                else Instantiate(boom, toBlowU[i], transform.rotation);
            }

        //D
        if (toBlowD.Count > 0)
            for (int i = 0; i < toBlowD.Count; i++)
            {
                if (i == toBlowD.Count - 1) Instantiate(boom, toBlowD[i], transform.rotation);
                else Instantiate(boom, toBlowD[i], transform.rotation);
            }
        ObjectPoolManager.Instance.DestroyObject(gameObject);
    }

    void EvaluateBoomRanges()
    {
        if (_evaluated) return;
        // L
        for (float i = 1; i <= boomRange; i++)
        {
            if (Physics.Raycast(transform.position, Vector3.left, i, wallLayer))
            {
                break;
            }

            if (Physics.Raycast(transform.position, Vector3.left, i,blowableLayer))
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
        
            if (Physics.Raycast(transform.position, Vector3.right, i,blowableLayer))
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
            
            if (Physics.Raycast(transform.position, Vector3.forward, i,blowableLayer))
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
            
            if (Physics.Raycast(transform.position, Vector3.back, i,blowableLayer))
            {
                toBlowD.Add(new Vector3(transform.position.x, transform.position.y, transform.position.z - i));
                break;
            }
            
            toBlowD.Add(new Vector3(transform.position.x, transform.position.y, transform.position.z - i));
        }

        _evaluated = true;
    }
    
}