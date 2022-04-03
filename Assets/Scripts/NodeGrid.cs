using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector2 worldPos;

    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public int FCost => ( gCost + hCost );
    public Node parent;

    public Node( bool walk, Vector2 pos, int x, int y )
    {
        walkable = walk;
        worldPos = pos;
        gridX = x;
        gridY = y;
    }
    public int HeapIndex { get; set; }

    public int CompareTo(Node other)
    {
        int compare = FCost.CompareTo(other.FCost);
        if ( compare == 0 )
            compare = hCost.CompareTo(other.hCost);

        return -compare;
    }
}

public class NodeGrid : MonoBehaviour
{
    public LayerMask obstacleMask;
    public Vector2 gridStart;
    public Vector2 gridEnd;
    private Vector2 gridSize;
    public float nodeSize;

    public int MaxLength => ( gridSizeX * gridSizeY / 2 );

    private int gridSizeX;
    private int gridSizeY;
    private Node[,] nodes;

    // Start is called before the first frame update
    void Start()
    {
        CreateGrid();
    }

    // Update is called once per frame
    //void Update()
    //{
    //}

    public Node NodeFromWorld( Vector2 pos )
    {
        float percentX = Mathf.Clamp01((pos.x + gridSize.x / 2) / gridSize.x );
        float percentY = Mathf.Clamp01(( pos.y + gridSize.y / 2 ) / gridSize.y);

        int x = Mathf.CeilToInt(( gridSizeX-1 ) * percentX);
        int y = Mathf.CeilToInt(( gridSizeY-1 ) * percentY);

        return nodes[x, y];
    }

    public List<Node> GetNeighbors( Node node )
    {
        List<Node> neighbors = new List<Node>();
        int nodeX, nodeY;

        for ( int x = -1; x <= 1; x++ )
        {
            for ( int y = -1; y <= 1; y++ )
            {
                if ( x == 0 && y == 0 )
                    continue;

                nodeX = node.gridX + x;
                nodeY = node.gridY + y;

                if( nodeX >= 0 && nodeX < gridSizeX && nodeY >= 0 && nodeY < gridSizeY )
                {
                    neighbors.Add(nodes[nodeX, nodeY]);
                }
            }
        }
        

        return neighbors;
    }

    private void CreateGrid()
    {
        gridSize = gridEnd - gridStart;
        gridSizeX = Mathf.CeilToInt(gridSize.x / nodeSize);
        gridSizeY = Mathf.CeilToInt(gridSize.y / nodeSize);
        Vector2 boxSize = new Vector2(nodeSize * 0.5f, nodeSize * 0.5f);
        Vector3 pos;
        nodes = new Node[gridSizeX, gridSizeY];

        for ( int x = 0; x < gridSizeX; x++ )
        {
            for ( int y = 0; y < gridSizeY; y++ )
            {
                pos = gridStart + new Vector2(x * nodeSize, y * nodeSize);
                bool walkable = !Physics2D.OverlapBox(pos, boxSize, 0, obstacleMask);
                nodes[x, y] = new Node(walkable, pos, x, y);
            }
        }
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireCube(( gridStart + gridEnd ) / 2, gridEnd - gridStart );
        Vector2 size = new Vector3(nodeSize, nodeSize, 1);

        if ( nodes != null )
        {
            foreach ( Node node in nodes )
            {
                Gizmos.color = node.walkable ? Color.gray : Color.red;
                Gizmos.DrawWireCube(node.worldPos, size);
            }
        }
    }
}
