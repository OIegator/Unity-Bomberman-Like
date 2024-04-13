using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    public Transform Container { get; private set; }

    public readonly Queue<GameObject> Objects;

    public ObjectPool(Transform container)
    {
        Container = container;
        Objects = new Queue<GameObject>();
    }
}
