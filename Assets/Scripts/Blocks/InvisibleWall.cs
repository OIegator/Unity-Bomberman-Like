using UnityEngine;


public class InvisibleWall : MonoBehaviour, IPooledObject
{
    public ObjectType ObjectType => ObjectType.InvisibleWall;
    
}
