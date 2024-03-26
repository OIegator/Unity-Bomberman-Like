using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour, IPooledObject
{
    public ObjectType ObjectType => ObjectType.Box;

    public void DestroyObject(GameObject obj)
    {
        ObjectPoolManager.Instance.DestroyObject(obj);
    }
}
