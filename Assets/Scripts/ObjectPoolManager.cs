using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    public static ObjectPoolManager Instance;

    [SerializeField] private List<ObjectTypeData> objectTypes;

    private Dictionary<ObjectType, ObjectPool> _pool;
    private readonly List<GameObject> _activeObjects = new();

    private void Awake()
    {
        Instance = this;
    }

    public void InitializePool()
    {
        _pool = new Dictionary<ObjectType, ObjectPool>();

        var emptyGameObject = new GameObject();

        foreach (var objectType in objectTypes)
        {
            var container = Instantiate(emptyGameObject, transform, false);
            container.name = objectType.objectType.ToString();
            _pool[objectType.objectType] = new ObjectPool(container.transform);
            for (int i = 0; i < objectType.initialCount; i++)
            {
                var obj = InstantiateObject(objectType.objectType, container.transform);
                _pool[objectType.objectType].Objects.Enqueue(obj);
            }
        }

        Destroy(emptyGameObject);
    }

    public GameObject GetObject(ObjectType objectType)
    {
        GameObject obj;
        if (_pool[objectType].Objects.Count > 0)
        {
            obj = _pool[objectType].Objects.Dequeue();
        }
        else
        {
            obj = InstantiateObject(objectType, _pool[objectType].Container);
        }
        obj.SetActive(true);
        _activeObjects.Add(obj);
        return obj;
    }

    public void DestroyObject(GameObject obj)
    {
        _pool[obj.GetComponent<IPooledObject>().ObjectType].Objects.Enqueue(obj);
        _activeObjects.Remove(obj);
        obj.SetActive(false);
    }

    public void DestroyAllObjects()
    {
        foreach (GameObject obj in _activeObjects)
        {
            _pool[obj.GetComponent<IPooledObject>().ObjectType].Objects.Enqueue(obj);
            obj.SetActive(false);
        }

        _activeObjects.Clear();
    }

    private GameObject InstantiateObject(ObjectType objectType, Transform parent)
    {
        var obj = Instantiate(objectTypes.Find(x => x.objectType == objectType).prefab, parent);
        obj.SetActive(false);
        return obj;
    }
}

[Serializable]
public struct ObjectTypeData
{
    public ObjectType objectType;
    public GameObject prefab;
    public int initialCount;
}

public enum ObjectType
{
    Ground,
    Bomb,
    Wall,
    Box,
    Enemy,
    InvisibleWall,
    EnemySpawner,
    Explosion,
    Exit
}

