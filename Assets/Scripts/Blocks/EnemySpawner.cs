using TMPro;
using UnityEngine;

public class EnemySpawner : MonoBehaviour, IPooledObject
{
    public ObjectType ObjectType => ObjectType.EnemySpawner;

    [SerializeField]
    public TextMeshProUGUI text;

}