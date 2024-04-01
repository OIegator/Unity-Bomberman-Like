using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy : MonoBehaviour, IPooledObject
{
    public ObjectType ObjectType => objectType;
    [SerializeField] private ObjectType objectType;
    [SerializeField] private float dieTime = 0f;
    [SerializeField] private float defaultScale = 0.5f;
    private bool _isMoving;
    private Vector3 _origPos, _targetPos;
    public float timeToMove = 0.45f;
    public float timeToRotate = 0.2f;
    public float jumpHeight = 0.2f;
    public LayerMask wallLayer;
    public Node currentNode;
    private EnemyFactory _enemyFactory;

    private bool _isDead;
    private GameObject _player;
    private bool _findPlayer;

    private readonly Dictionary<Vector3, int> _directionToIndex = new()
    {
        {Vector3.forward, 0},
        {Vector3.left, 3},
        {Vector3.back, 1},
        {Vector3.right, 2}
    };

    private List<Node> _currentPath;
    private List<Node> _possiblePath;
    private int _currentNodeIndex;
    private Pathfinder _pathfinder;

    private void Update()
    {
        if (GameManager.Instance.currentState != GameState.Playing) return;
        if (_player == null || _isDead) return;
        if (!_findPlayer) LookingForPlayer();
        if (currentNode != _player.GetComponent<Bomberman>().currentNode && !_isMoving)
        {
            StartCoroutine(MakeStep());
            _pathfinder.FindPath(currentNode, _player.GetComponent<Bomberman>().currentNode, out _possiblePath);
            if (_possiblePath.Count != 0)
            {
                _currentPath = _possiblePath;
                _currentNodeIndex = 0;
            }
        }
    }

    void OnDrawGizmos()
    {
        if (_currentPath != null)
            foreach (var item in _currentPath)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(
                    new Vector3(item.transform.position.x, item.transform.position.y + 0.8f, item.transform.position.z),
                    0.2f);
            }
    }

    private IEnumerator MakeStep()
    {
        if (_currentPath != null && _currentPath.Count > 0 && _currentNodeIndex != _currentPath.Count)
        {
            var node = _currentPath[_currentNodeIndex];
            var direction = (node.transform.position - currentNode.transform.position).normalized;
            yield return StartCoroutine(MoveTo(direction));
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
            _currentNodeIndex++;
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
        return Physics.Raycast(transform.position + Vector3.up * 0.1f, direction, out var hit, 0.8f, wallLayer);
    }

    public void Setup(GameObject player, GameObject spawnPoint)
    {
        currentNode = spawnPoint.GetComponent<Node>();
        transform.position = spawnPoint.transform.position + Vector3.up * 0.8f;
        _player = player;
        _findPlayer = false;
        transform.localScale = new Vector3(defaultScale, defaultScale, defaultScale);
        _isDead = false;
        _pathfinder = GetComponent<Pathfinder>();
        _currentNodeIndex = 0;
        _enemyFactory = FindObjectOfType<EnemyFactory>();
    }

    private void LookingForPlayer()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) <= 0.5f)
        {
            _findPlayer = true;
            _player.GetComponent<Bomberman>().Die();
            Die();
        }
    }

    public void Die()
    {
        if (_isDead) return;
        _isDead = true;
        
        _isMoving = false;
        _currentNodeIndex = 0;
        _enemyFactory.DecreaseEnemyCount();

        StopAllCoroutines();
        StartCoroutine(DieCoroutine());
    }

    private IEnumerator DieCoroutine()
    {
        yield return new WaitForSeconds(dieTime);
        var obj = ObjectPoolManager.Instance.GetObject(ObjectType.Explosion);
        obj.transform.position = transform.position;
        ObjectPoolManager.Instance.DestroyObject(gameObject);
    }
}