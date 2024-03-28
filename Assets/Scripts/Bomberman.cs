using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomberman : MonoBehaviour
{
    public GameObject bomb;
    public ParticleSystem splash;
    private bool _isMoving;
    private Vector3 _origPos, _targetPos;
    public float timeToMove = 0.45f;
    public float timeToRotate = 0.2f;
    public float jumpHeight = 0.2f;
    public LayerMask wallLayer;
    public Node currentNode;

    private readonly Dictionary<Vector3, int> _directionToIndex = new()
    {
        { Vector3.forward, 0 },
        { Vector3.left, 3 },
        { Vector3.back, 1 },
        { Vector3.right, 2 }
    };
    private void Start()
    {
        splash.Pause();
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            PlaceBomb();

        if (Input.GetKey(KeyCode.W) && !_isMoving)
            StartCoroutine(MovePlayer(Vector3.forward));

        if (Input.GetKey(KeyCode.A) && !_isMoving)
            StartCoroutine(MovePlayer(Vector3.left));

        if (Input.GetKey(KeyCode.S) && !_isMoving)
            StartCoroutine(MovePlayer(Vector3.back));

        if (Input.GetKey(KeyCode.D) && !_isMoving)
            StartCoroutine(MovePlayer(Vector3.right));
    }

    private bool IsWallCollision(Vector3 direction)
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, out var hit, 0.8f, wallLayer);
    }

    private IEnumerator MovePlayer(Vector3 direction)
    {
        _isMoving = true;
        splash.Play();
        float elapsedTime = 0;

        _origPos = transform.position;

        if (IsWallCollision(direction))
        {
            _targetPos = _origPos;
        }
        else
        {
            _targetPos = _origPos + direction;
            if (_directionToIndex.TryGetValue(direction, out int index))
            {
                currentNode = currentNode.GetNeighbors()[index];
            }
        }

        float startSize = transform.localScale.x;
        float targetSize = startSize * 0.9f;
        Vector3 startPos = transform.position;

        Quaternion startRotation = transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        while (elapsedTime < timeToMove)
        {
            // Smoothly move towards the target position 
            transform.position = Vector3.Lerp(startPos, _targetPos, EaseInOutQuint(elapsedTime / timeToMove));
            transform.rotation =
                Quaternion.Slerp(startRotation, targetRotation, EaseInOutQuint(elapsedTime / timeToRotate));

            float yOffset = elapsedTime < timeToMove * 0.5f
                ? Mathf.Lerp(0f, jumpHeight, EaseOutBounce(elapsedTime / timeToMove * 2))
                : Mathf.Lerp(jumpHeight, 0f, EaseOutBounce(elapsedTime / timeToMove - 0.5f) * 2);

            float newSize = elapsedTime < timeToMove * 0.5f
                ? Mathf.Lerp(startSize, targetSize, elapsedTime / timeToMove * 2)
                : Mathf.Lerp(targetSize, startSize, (elapsedTime / timeToMove - 0.5f) * 2);
            transform.localScale = new Vector3(newSize, newSize, newSize);

            transform.position += Vector3.up * yOffset;


            elapsedTime += Time.deltaTime;

            yield return null;
        }

        transform.position = _targetPos;

        _isMoving = false;
    }

    private float EaseInOutQuint(float t)
    {
        return t < 0.5f ? 16 * t * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 5) / 2;
    }

    private float EaseOutBounce(float t)
    {
        return t switch
        {
            < 1f / 2.75f => 7.5625f * t * t,
            < 2f / 2.75f => 7.5625f * (t -= 1.5f / 2.75f) * t + 0.75f,
            < 2.5f / 2.75f => 7.5625f * (t -= 2.25f / 2.75f) * t + 0.9375f,
            _ => 7.5625f * (t -= 2.625f / 2.75f) * t + 0.984375f
        };
    }

    private void PlaceBomb()
    {
        if (bomb != null)
        {
            Vector3 bombPosition = transform.position;

            // Check if the player is inside the bomb collider
            Collider[] colliders = Physics.OverlapSphere(transform.position, 0.2f);
            foreach (Collider col in colliders)
            {
                if (col.gameObject.CompareTag("Bomb"))
                {
                    return;
                }
            }

            if (_isMoving)
            {
                float distanceToOrigPos = Vector3.Distance(transform.position, _origPos);
                float distanceToTargetPos = Vector3.Distance(transform.position, _targetPos);

                // Choose the closer position
                bombPosition = distanceToOrigPos < distanceToTargetPos ? _origPos : _targetPos;
            }

            bombPosition += Vector3.up * 0.3f;

            Instantiate(bomb, bombPosition, Quaternion.identity);
        }
        else
        {
            Debug.LogWarning("Prefab to instantiate is not assigned.");
        }
    }
}