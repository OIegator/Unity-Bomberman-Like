using UnityEngine;

public class Wall : MonoBehaviour, IPooledObject
{
    public ObjectType ObjectType => ObjectType.Wall;
    
}

