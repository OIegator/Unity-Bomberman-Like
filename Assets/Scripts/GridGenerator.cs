using System.Collections.Generic;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [SerializeField] private List<Node> nodes;
    [SerializeField] private GameObject playerSpawnPoint;

    private Stage _stage;
    private EnemyFactory _enemyFactory;

    public void Setup(Stage currentStage, EnemyFactory enemyFactory)
    {
        _stage = currentStage;
        _enemyFactory = enemyFactory;

        nodes = new List<Node>();

        for (int i = 0; i < _stage.size.x; i++)
        {
            for (int j = 0; j < _stage.size.y; j++)
            {
                var cell = ObjectPoolManager.Instance.GetObject(_stage.blocks[i * _stage.size.y + j].objectType);
                nodes.Add(cell.GetComponent<Node>());
                cell.GetComponent<Node>().DeleteNeighbors();

                if (cell.TryGetComponent(out Ground ground))
                {
                    SetupGroundBlock(ground, i, j);
                }

                if (_stage.start.x == i && _stage.start.y == j)
                {
                    playerSpawnPoint = cell;
                }
            }
        }

        SetupNeighbours();
    }

    private void SetupGroundBlock(Ground ground, int i, int j)
    {
        ground.transform.position = new Vector3(transform.position.x + i, 0, transform.position.z + j);

        switch (_stage.blocks[i * _stage.size.y + j].objectAbove)
        {
            case AboveObjectType.Box:
                var obj = CreateObject(ObjectType.Box, ground.transform, 0.95f);
                obj.GetComponent<Box>().node = ground.GetComponent<Node>();
                ground.GetComponent<Node>().SetState(State.Inaccessible);
                break;
            case AboveObjectType.Wall:
                CreateObject(ObjectType.Wall, ground.transform, 1f);
                ground.GetComponent<Node>().SetState(State.Inaccessible);
                break;
            case AboveObjectType.InvisibleWall:
                CreateObject(ObjectType.InvisibleWall, ground.transform, 1f);
                ground.GetComponent<Node>().SetState(State.Inaccessible);
                break;
            case AboveObjectType.EnemySpawner:
                CreateObject(ObjectType.EnemySpawner, ground.transform, 1f);
                _enemyFactory.AddSpawnPoint(ground.gameObject);
                break;
            case AboveObjectType.Exit:
                CreateObject(ObjectType.Exit, ground.transform, 1f);
                break;
        }
    }

    private static GameObject CreateObject(ObjectType type, Transform place, float offsetY)
    {
        var obj = ObjectPoolManager.Instance.GetObject(type);
        obj.transform.position = new Vector3(place.position.x, place.position.y + offsetY, place.position.z);
        return obj;
    }

    private void SetupNeighbours()
    {
        for (int i = 0; i < nodes.Count; i++)
        {
            int idUp = i + 1;
            if (idUp % _stage.size.y != 0 && idUp < nodes.Count)
            {
                nodes[i].AddNeighbor(nodes[idUp]);
            }

            int idDown = i - 1;
            if (i % _stage.size.y != 0)
            {
                nodes[i].AddNeighbor(nodes[idDown]);
            }

            int idRight = i + _stage.size.y;
            if (idRight < nodes.Count)
            {
                nodes[i].AddNeighbor(nodes[idRight]);
            }

            int idLeft = i - _stage.size.y;
            if (idLeft >= 0)
            {
                nodes[i].AddNeighbor(nodes[idLeft]);
            }
        }
    }

    public GameObject GetPlayerSpawnPoint()
    {
        return playerSpawnPoint;
    }
}
