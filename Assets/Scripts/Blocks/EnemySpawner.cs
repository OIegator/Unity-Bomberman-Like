using UnityEngine;

public class EnemySpawner : MonoBehaviour, IPooledObject
{
    public ObjectType ObjectType => ObjectType.EnemySpawner;
    
}