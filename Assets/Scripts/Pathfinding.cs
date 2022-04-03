using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    const int diagonalWeight = 14;
    const int orthagonalWeight = 10;

    public NodeGrid pathGrid;
    
    public List<Vector2> FindPath( Vector2 start, Vector2 end )
    {
        Node startNode = pathGrid.NodeFromWorld(start);
        Node endNode = pathGrid.NodeFromWorld(end);

        //List<Node> openSet = new List<Node>();
        Heap<Node> openSet = new Heap<Node>( pathGrid.MaxLength);
        HashSet<Node> closedSet = new HashSet<Node>();

        openSet.Add(startNode);

        while( openSet.Count > 0 )
        {
            //Node currentNode = openSet[0];

            //for ( int i = 1; i < openSet.Count; i++ )
            //{
            //    if ( openSet[i].FCost < currentNode.FCost || 
            //            openSet[i].FCost == currentNode.FCost && openSet[i].hCost < currentNode.hCost )
            //        currentNode = openSet[i];
            //}
            //openSet.Remove(currentNode);
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);

            if( currentNode == endNode )
            {
                return RetracePath(startNode, endNode);
            }

            foreach ( Node neighbor in pathGrid.GetNeighbors(currentNode) )
            {
                if ( !neighbor.walkable || closedSet.Contains(neighbor) )
                    continue;

                int neighborMoveCost = GetDistance(currentNode, neighbor);
                int newMoveCost = currentNode.gCost + neighborMoveCost;
                if( newMoveCost < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newMoveCost;
                    neighbor.hCost = neighborMoveCost;
                    neighbor.parent = currentNode;

                    if ( !openSet.Contains(neighbor) )
                        openSet.Add(neighbor);
                }
            }
        }

        return new List<Vector2>();
    }

    private List<Vector2> RetracePath( Node start, Node end )
    {
        //List<Node> path = new List<Node>();
        List<Vector2> waypoints = new List<Vector2>();
        Node currentNode = end;

        while ( currentNode != start )
        {
            //path.Add(currentNode);
            waypoints.Add(currentNode.worldPos);
            currentNode = currentNode.parent;
        }
        //List<Vector2> waypoints = SimplifyPath(path);
        waypoints.Reverse();
        return waypoints;
    }

    private List<Vector2> SimplifyPath( List<Node> path )
    {
        List<Vector2> waypoints = new List<Vector2>();
        Vector2 previousDir = Vector2.zero;
        Vector2 direction;

        // Add Last Waypoint
        waypoints.Add(path[0].worldPos);
        for ( int i = 1; i < path.Count; i++ )
        {
            direction = path[i].worldPos - path[i - 1].worldPos;
            if ( direction != previousDir )
            {
                waypoints.Add(path[i].worldPos);
                Debug.Log($"Node {Mathf.Abs(path.Count - i)}: {direction} != {previousDir}");
            }
            else
                Debug.Log($"Node {Mathf.Abs(path.Count - i)}: {direction} == {previousDir}");
            previousDir = direction;
        }

        return waypoints;
    }

    private int GetDistance( Node a, Node b)
    {
        int distance = 0;

        int xDist = Mathf.Abs(a.gridX - b.gridX);
        int yDist = Mathf.Abs(a.gridY - b.gridY);
        if ( xDist < yDist )
            distance += diagonalWeight * xDist;
        else
            distance += diagonalWeight * yDist;
        distance += orthagonalWeight * Mathf.Abs(xDist - yDist);

        return distance;
    }
}
