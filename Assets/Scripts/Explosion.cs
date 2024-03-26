using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    public float delay;
    private float _counter;

    void Start()
    {
        _counter = delay;
    }

    void Update()
    {
        if (_counter > 0) _counter -= Time.deltaTime;
        else Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Brick"))
        {
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Player"))
        {
            if (delay - _counter < 0.2f)
            {
                other.GetComponent<Bomberman>().Die();
            }
        }
    }
}