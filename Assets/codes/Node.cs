using System.Collections.Generic;
using UnityEngine;
public class Node
{
    public float gCost;
    public float hCost;
    public float fCost => gCost + hCost; // f(x) = g(x) + h(x)
    public Node parent;


    private Vector2Int boardPosition;
    public Vector3 worldPosition;
    public bool isWalkable=true;
    public List<Node> Neighbors { get; private set; } = new List<Node>();
    public Vector2Int BoardPosition
    {
        get { return boardPosition; }
        set { boardPosition = value; }
    }

    public Vector3 WorldPosition
    {
        get { return worldPosition; }
        set { worldPosition = value; }
    }

    public void InitializeNode(Vector2Int boardPos, Vector3 worldPos)
    {
        boardPosition = boardPos;
        worldPosition = worldPos;
        isWalkable = true;
    }

    public void SetPiece(bool isOccupied)
    {
        isWalkable = isOccupied;
    }

    public void AddNeighbor(Node neighbor)
    {
        if (neighbor != null && !Neighbors.Contains(neighbor))
        {
            Neighbors.Add(neighbor);
        }
    }
    public Node GetNeighbor(Vector2Int direction)
    {
        foreach (Node neighbor in Neighbors)
        {
            if (neighbor.BoardPosition == BoardPosition + direction)
            {
                return neighbor;
            }
        }
        return null;
    }

}