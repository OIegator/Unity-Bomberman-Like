using UnityEngine;


public class InvisibleWall : MonoBehaviour, IPooledObject
{
    public GameObject visual;
    public ObjectType ObjectType => ObjectType.InvisibleWall;
    
}
