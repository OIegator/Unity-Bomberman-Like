using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    [SerializeField] private State state;
    [SerializeField] private List<Node> neighbors = new List<Node>();
    public Node Parent { get; set; }

    public State GetState()
    {
        return state;
    }

    public void SetState(State newState)
    {
        state = newState;
    }

    public void AddNeighbor(Node neighbor)
    {
        neighbors.Add(neighbor);
    }

    public List<Node> GetNeighbors()
    {
        return neighbors;
    }

    public void DeleteNeighbors()
    {
        neighbors.Clear();
    }
    
}

public enum State
{
    Accessible,
    Inaccessible
}