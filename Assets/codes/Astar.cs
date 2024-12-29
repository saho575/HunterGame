using System.Collections.Generic;
using UnityEngine;

public class Astar
{
    private GridManager gridManager;

    /*
    public Astar(GridManager gridManager)
    {
        this.gridManager = gridManager;
    }
    */

    public List<Node> FindPath(Node startNode, Node targetNode)
    {
        if (startNode == null || targetNode == null || !targetNode.isWalkable)
        {
            Debug.LogWarning("Invalid start or target node.");
            return null;
        }

        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();

        while (openSet.Count > 0)
        {
            // Get the node with the lowest F cost
            Node currentNode = GetLowestFCostNode(openSet);

            // Target node reached
            if (currentNode == targetNode)
            {
                return RetracePath(startNode, targetNode);
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            foreach (Node neighbor in currentNode.Neighbors)
            {
                // Skip non-walkable or already processed nodes
                if (!neighbor.isWalkable || closedSet.Contains(neighbor))
                    continue;

                float tentativeGCost = currentNode.gCost + Vector2Int.Distance(currentNode.BoardPosition, neighbor.BoardPosition);

                if (tentativeGCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    // Update node costs and set parent
                    neighbor.gCost = tentativeGCost;
                    neighbor.hCost = Vector2Int.Distance(neighbor.BoardPosition, targetNode.BoardPosition);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        Debug.LogWarning("Path not found.");
        return null;
    }

    private Node GetLowestFCostNode(List<Node> nodes)
    {
        Node lowest = nodes[0];
        foreach (Node node in nodes)
        {
            // Compare F cost and use H cost as a tiebreaker
            if (node.fCost < lowest.fCost || (node.fCost == lowest.fCost && node.hCost < lowest.hCost))
                lowest = node;
        }
        return lowest;
    }

    private List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse(); // Reverse to get path from start to end
        return path;
    }
}