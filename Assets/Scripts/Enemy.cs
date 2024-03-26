using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IPooledObject
{
    public ObjectType ObjectType => objectType;
    [SerializeField] private ObjectType objectType;
    [SerializeField] private float attackCooldownTime = 1f;
    [SerializeField] private float dieTime = 3f;

    private bool _isDead = false;
    private Bomberman _player;

    public void Setup(Bomberman player)
    {
        _player = player;
        _isDead = false;
    }

    private void DestroyObject()
    {
        ObjectPoolManager.Instance.DestroyObject(gameObject);
    }

    private void Die()
    {
        if (_isDead) return;
        _isDead = true;

        StopAllCoroutines();
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        yield return new WaitForSeconds(dieTime);
        DestroyObject();
    }

    private void Update()
    {
        //TODO
    }

}

