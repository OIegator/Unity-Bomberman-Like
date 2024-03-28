using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class Enemy : MonoBehaviour, IPooledObject
{
    public ObjectType ObjectType => objectType;
    [SerializeField] private ObjectType objectType;
    [SerializeField] private float dieTime = 3f;
    private bool _isMoving;
    private Vector3 _origPos, _targetPos;
    public float timeToMove = 0.45f;
    public float timeToRotate = 0.2f;
    public float jumpHeight = 0.2f;
    public LayerMask wallLayer;
    public Node currentNode;

    private bool _isDead = false;
    private Bomberman _player;

    private readonly Dictionary<Vector3, int> _directionToIndex = new()
    {
        {Vector3.forward, 0},
        {Vector3.left, 3},
        {Vector3.back, 1},
        {Vector3.right, 2}
    };

    private List<Node> _currentPath;
    private int _currentPathIndex = 0;

    private Pathfinder _pathfinder;

    private void Start()
    {
        _pathfinder = GetComponent<Pathfinder>();
        _player = FindObjectOfType<Bomberman>();
    }

    private void Update()
    {
        _pathfinder.FindPath(currentNode, _player.currentNode, out _currentPath);
    }

    void OnDrawGizmos()
    {
        if(_currentPath !=null)
            foreach (var item in _currentPath)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(new Vector3(item.transform.position.x, item.transform.position.y + 0.8f, item.transform.position.z), 0.2f);
            }
    }

    private void FollowPath(List<Node> path)
    {
        _currentPath = path;
        _currentPathIndex = 0;
        StartCoroutine(MoveAlongPath());
    }

    private IEnumerator MoveAlongPath()
    {
        foreach (var direction in
                 _currentPath.Select(node => (node.transform.position - transform.position).normalized))
        {
            yield return StartCoroutine(MoveTo(direction));
            _currentPathIndex++;
        }
    }

    private IEnumerator MoveTo(Vector3 direction)
    {
        _isMoving = true;
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

    private static float EaseInOutQuint(float t)
    {
        return t < 0.5f ? 16 * t * t * t * t * t : 1 - Mathf.Pow(-2 * t + 2, 5) / 2;
    }

    private static float EaseOutBounce(float t)
    {
        return t switch
        {
            < 1f / 2.75f => 7.5625f * t * t,
            < 2f / 2.75f => 7.5625f * (t -= 1.5f / 2.75f) * t + 0.75f,
            < 2.5f / 2.75f => 7.5625f * (t -= 2.25f / 2.75f) * t + 0.9375f,
            _ => 7.5625f * (t -= 2.625f / 2.75f) * t + 0.984375f
        };
    }

    private bool IsWallCollision(Vector3 direction)
    {
        return Physics.Raycast(transform.position + Vector3.up * 0.5f, direction, out var hit, 0.8f, wallLayer);
    }

    public void Setup(Bomberman player, GameObject spawnPoint)
    {
        currentNode = spawnPoint.GetComponent<Node>();
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
}