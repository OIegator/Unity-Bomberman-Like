using UnityEngine;

public class Box : MonoBehaviour, IPooledObject
{
    public ObjectType ObjectType => ObjectType.Box;
    public Node node;

    public void DestroyObject(GameObject obj)
    {
        ObjectPoolManager.Instance.DestroyObject(obj);
    }
}
