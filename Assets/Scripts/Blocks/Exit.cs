using UnityEngine;

public class Exit : MonoBehaviour, IPooledObject
{
    public ObjectType ObjectType => ObjectType.Exit;
    private bool _isTriggered = false;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !_isTriggered)
        {
            _isTriggered = true;
            GameManager.Instance.EndStage();
        }
    }
    
}
