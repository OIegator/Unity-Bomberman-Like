using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    private List<Node> _openList;
    private List<Node> _closedList;

    public void FindPath(Node startNode, Node endNode, out List<Node> path)
    {
        _openList = new List<Node> { startNode };
        _closedList = new List<Node>();
        path = new List<Node>();

        int iterations = 0;

        while (_openList.Count > 0 && iterations < 5)
        {
            Node currentNode = _openList[0];
            
            foreach (var node in _openList)
            {
                if (Vector3.Distance(node.transform.position, endNode.transform.position) <
                    Vector3.Distance(currentNode.transform.position, endNode.transform.position))
                {
                    currentNode = node;
                }
            }

            _openList.Remove(currentNode);
            _closedList.Add(currentNode);

            if (currentNode == endNode)
            {
                ReconstructPath(startNode, endNode, path);
                return;
            }

            foreach (var neighbor in currentNode.GetNeighbors())
            {
                if (neighbor.GetState() != State.Accessible || _closedList.Contains(neighbor))
                    continue;

                float newCostToNeighbor = Vector3.Distance(neighbor.transform.position, endNode.transform.position);
                float currentCost = Vector3.Distance(currentNode.transform.position, endNode.transform.position);

                bool inOpenList = _openList.Contains(neighbor);

                if (!inOpenList || newCostToNeighbor < currentCost)
                {
                    neighbor.Parent = currentNode;
                    if (!inOpenList)
                        _openList.Add(neighbor);
                }
            }

            iterations++;
        }
    }

    private void ReconstructPath(Node startNode, Node endNode, List<Node> path)
    {
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
    }
}

