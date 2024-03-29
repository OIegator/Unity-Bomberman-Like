using UnityEngine;

public class Ground : MonoBehaviour, IPooledObject
{
    [SerializeField] private AboveObjectType aboveObjectType;

    public ObjectType ObjectType => ObjectType.Ground;

    public void DestroyObject(GameObject obj)
    {
        ObjectPoolManager.Instance.DestroyObject(obj);
    }

    public AboveObjectType GetGroundObjectType() 
    {
        return aboveObjectType;
    }
}

public enum AboveObjectType 
{
    None,
    Box,
    Wall,
    EnemySpawner,
    InvisibleWall,
    Exit,
}
