using UnityEngine;

public class Exit : MonoBehaviour, IPooledObject
{
    public ObjectType ObjectType => ObjectType.Exit;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            GameManager.Instance.EndStage();
        }
    }
    
}
