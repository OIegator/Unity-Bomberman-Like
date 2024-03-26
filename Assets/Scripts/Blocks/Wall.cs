using UnityEngine;

public class Wall : MonoBehaviour, IPooledObject
{
    public ObjectType ObjectType => ObjectType.Wall;

    public void DestroyObject(GameObject obj)
    {
        ObjectPoolManager.Instance.DestroyObject(obj);
    }
}

