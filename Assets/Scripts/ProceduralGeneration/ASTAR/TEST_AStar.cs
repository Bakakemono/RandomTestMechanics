using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TEST_AStar : MonoBehaviour
{
    struct Node
    {
        public Vector2Int pos;

        public float distFromGoal;
        public int countTile;
        public int coastTile;

        public TileBase tile;

        public List<Node> parent;

        public float GetTileFullCoast()
        {
            return distFromGoal + countTile + coastTile;
        }
    }

    [SerializeField] private Tilemap tilemap;
    [SerializeField] private Tile groundTile;
    [SerializeField] private Tile wallTill;
    [SerializeField] private Tile goalTile;
    [SerializeField] private Tile wayTile;
    [SerializeField] private Tile startingTile;
    [SerializeField] private Tile neighborTile;

    private Vector2Int mapSize;

    private Vector2Int startPos;
    private Vector2Int goalPos;

    private Node[,] map;

    private bool isStarted = false;

    private List<Node> openList;
    private List<Node> closeList;


    Vector2Int UP_LEFT = new Vector2Int(-1, 1);
    Vector2Int UP = new Vector2Int(0, 1);
    Vector2Int UP_RIGHT = new Vector2Int(1, 1);

    Vector2Int LEFT = new Vector2Int(-1, 0);
    Vector2Int RIGHT = new Vector2Int(1, 0);

    Vector2Int DOWN_LEFT = new Vector2Int(-1, -1);
    Vector2Int DOWN = new Vector2Int(0, -1);
    Vector2Int DOWN_RIGHT = new Vector2Int(1, -1);


    void Start()
    {
        int xCount = 0;
        while (true)
        {
            if (tilemap.HasTile(new Vector3Int(xCount, 0, 0)))
            {
                xCount++;
                continue;
            }

            break;
        }
        mapSize.x = xCount;

        int yCount = 0;
        while (true)
        {
            if (tilemap.HasTile(new Vector3Int(0, yCount, 0)))
            {
                yCount++;
                continue;
            }

            mapSize.y = yCount;
            break;
        }

        Debug.Log("The size of the map is : " + mapSize);
        map = new Node[mapSize.x, mapSize.y];

        isStarted = true;

        openList = new List<Node>();
        closeList = new List<Node>();

        AnalyseMap();
        ShearchPath();
    }

    private void AnalyseMap()
    {
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                TileBase tile = tilemap.GetTile(new Vector3Int(x, y, 0));

                if (tile == groundTile)
                {
                    map[x, y].coastTile = 1;
                    map[x, y].pos = new Vector2Int(x, y);
                    map[x, y].tile = groundTile;
                }
                else if (tile == wallTill)
                {
                    map[x, y].coastTile = 9999;
                    map[x, y].pos = new Vector2Int(x, y);
                    map[x, y].tile = wallTill;
                }
                else if(tile == goalTile)
                {
                    map[x, y].coastTile = 0;
                    map[x, y].pos = new Vector2Int(x, y);
                    map[x, y].tile = goalTile;

                    goalPos = new Vector2Int(x, y);
                }
                else if(tile == startingTile)
                {
                    map[x, y].coastTile = 0;
                    map[x, y].pos = new Vector2Int(x, y);
                    map[x, y].tile = startingTile;
                    map[x, y].countTile = 0;
                    map[x, y].distFromGoal = 0.0f;

                    startPos = new Vector2Int(x, y);
                    openList.Add(map[x, y]);
                }
            }
        }

        Debug.Log("goal : " + goalPos);
        Debug.Log("Start : " + startPos);
    }

    private void ShearchPath()
    {
        bool goalReach = false;

        Node goal = new Node();
        int count = 0;
        while (!goalReach)
        {
            count++;
            if (count > 10000)
            { 
                Debug.Log("debug exit one");
                break;
            }

            List<Node> neighbor;

            Vector2Int neighborPos = openList[0].pos + UP_LEFT;
            if (neighborPos.x >= 0 && neighborPos.y < mapSize.y)
            {
                if (map[neighborPos.x, neighborPos.y].tile == goalTile)
                {
                    goalReach = true;
                    map[neighborPos.x, neighborPos.y].parent = new List<Node>();
                    map[neighborPos.x, neighborPos.y].parent.Add(openList[0]);
                    goal = map[neighborPos.x, neighborPos.y];
                    continue;
                }

                if (map[neighborPos.x, neighborPos.y].tile != wallTill || closeList.Contains(map[neighborPos.x, neighborPos.y]))
                {
                    map[neighborPos.x, neighborPos.y].countTile = openList[0].countTile + 1;
                    map[neighborPos.x, neighborPos.y].distFromGoal =
                        Vector2.Distance(map[neighborPos.x, neighborPos.y].pos, goalPos);
                    map[neighborPos.x, neighborPos.y].parent = new List<Node>();
                    map[neighborPos.x, neighborPos.y].parent.Add(openList[0]);
                    openList.Add(map[neighborPos.x, neighborPos.y]);
                }
            }

             neighborPos = openList[0].pos + UP;
            if (neighborPos.y < mapSize.y)
            {
                if (map[neighborPos.x, neighborPos.y].tile == goalTile)
                {
                    goalReach = true;
                    map[neighborPos.x, neighborPos.y].parent = new List<Node>();
                    map[neighborPos.x, neighborPos.y].parent.Add(openList[0]);
                    goal = map[neighborPos.x, neighborPos.y];
                    continue;
                }

                if (map[neighborPos.x, neighborPos.y].tile != wallTill || closeList.Contains(map[neighborPos.x, neighborPos.y]))
                {
                    map[neighborPos.x, neighborPos.y].countTile = openList[0].countTile + 1;
                    map[neighborPos.x, neighborPos.y].distFromGoal =
                        Vector2.Distance(map[neighborPos.x, neighborPos.y].pos, goalPos);
                    map[neighborPos.x, neighborPos.y].parent = new List<Node>();
                    map[neighborPos.x, neighborPos.y].parent.Add(openList[0]);
                    openList.Add(map[neighborPos.x, neighborPos.y]);
                }
            }

            neighborPos = openList[0].pos + UP_RIGHT;
            if (neighborPos.x < mapSize.x && neighborPos.y < mapSize.y)
            {
                if (map[neighborPos.x, neighborPos.y].tile == goalTile)
                {
                    goalReach = true;
                    map[neighborPos.x, neighborPos.y].parent = new List<Node>();
                    map[neighborPos.x, neighborPos.y].parent.Add(openList[0]);
                    goal = map[neighborPos.x, neighborPos.y];
                    continue;
                }

                if (map[neighborPos.x, neighborPos.y].tile != wallTill || closeList.Contains(map[neighborPos.x, neighborPos.y]))
                {
                    map[neighborPos.x, neighborPos.y].countTile = openList[0].countTile + 1;
                    map[neighborPos.x, neighborPos.y].distFromGoal =
                        Vector2.Distance(map[neighborPos.x, neighborPos.y].pos, goalPos);
                    map[neighborPos.x, neighborPos.y].parent = new List<Node>();
                    map[neighborPos.x, neighborPos.y].parent.Add(openList[0]);
                    openList.Add(map[neighborPos.x, neighborPos.y]);
                }
            }

            neighborPos = openList[0].pos + LEFT;
            if (neighborPos.x >= 0)
            {
                if (map[neighborPos.x, neighborPos.y].tile == goalTile)
                {
                    goalReach = true;
                    map[neighborPos.x, neighborPos.y].parent = new List<Node>();
                    map[neighborPos.x, neighborPos.y].parent.Add(openList[0]);
                    goal = map[neighborPos.x, neighborPos.y];
                    continue;
                }

                if (map[neighborPos.x, neighborPos.y].tile != wallTill || closeList.Contains(map[neighborPos.x, neighborPos.y]))
                {
                    map[neighborPos.x, neighborPos.y].countTile = openList[0].countTile + 1;
                    map[neighborPos.x, neighborPos.y].distFromGoal =
                        Vector2.Distance(map[neighborPos.x, neighborPos.y].pos, goalPos);
                    map[neighborPos.x, neighborPos.y].parent = new List<Node>();
                    map[neighborPos.x, neighborPos.y].parent.Add(openList[0]);
                    openList.Add(map[neighborPos.x, neighborPos.y]);
                }
            }

            neighborPos = openList[0].pos + RIGHT;
            if (neighborPos.x < mapSize.x)
            {
                if (map[neighborPos.x, neighborPos.y].tile == goalTile)
                {
                    goalReach = true;
                    map[neighborPos.x, neighborPos.y].parent = new List<Node>();
                    map[neighborPos.x, neighborPos.y].parent.Add(openList[0]);
                    goal = map[neighborPos.x, neighborPos.y];
                    continue;
                }

                if (map[neighborPos.x, neighborPos.y].tile != wallTill || closeList.Contains(map[neighborPos.x, neighborPos.y]))
                {
                    map[neighborPos.x, neighborPos.y].countTile = openList[0].countTile + 1;
                    map[neighborPos.x, neighborPos.y].distFromGoal =
                        Vector2.Distance(map[neighborPos.x, neighborPos.y].pos, goalPos);
                    map[neighborPos.x, neighborPos.y].parent = new List<Node>();
                    map[neighborPos.x, neighborPos.y].parent.Add(openList[0]);
                    openList.Add(map[neighborPos.x, neighborPos.y]);
                }
            }

            neighborPos = openList[0].pos + DOWN_LEFT;
            if (neighborPos.x >= 0 && neighborPos.y >= 0)
            {
                if (map[neighborPos.x, neighborPos.y].tile == goalTile)
                {
                    goalReach = true;
                    map[neighborPos.x, neighborPos.y].parent = new List<Node>();
                    map[neighborPos.x, neighborPos.y].parent.Add(openList[0]);
                    goal = map[neighborPos.x, neighborPos.y];
                    continue;
                }

                if (map[neighborPos.x, neighborPos.y].tile != wallTill || closeList.Contains(map[neighborPos.x, neighborPos.y]))
                {
                    map[neighborPos.x, neighborPos.y].countTile = openList[0].countTile + 1;
                    map[neighborPos.x, neighborPos.y].distFromGoal =
                        Vector2.Distance(map[neighborPos.x, neighborPos.y].pos, goalPos);
                    map[neighborPos.x, neighborPos.y].parent = new List<Node>();
                    map[neighborPos.x, neighborPos.y].parent.Add(openList[0]);
                    openList.Add(map[neighborPos.x, neighborPos.y]);
                }
            }

            neighborPos = openList[0].pos + DOWN;
            if (neighborPos.y >= 0)
            {
                if (map[neighborPos.x, neighborPos.y].tile == goalTile)
                {
                    goalReach = true;
                    map[neighborPos.x, neighborPos.y].parent = new List<Node>();
                    map[neighborPos.x, neighborPos.y].parent.Add(openList[0]);
                    goal = map[neighborPos.x, neighborPos.y];
                    continue;
                }

                if (map[neighborPos.x, neighborPos.y].tile != wallTill || closeList.Contains(map[neighborPos.x, neighborPos.y]))
                {
                    map[neighborPos.x, neighborPos.y].countTile = openList[0].countTile + 1;
                    map[neighborPos.x, neighborPos.y].distFromGoal =
                        Vector2.Distance(map[neighborPos.x, neighborPos.y].pos, goalPos);
                    map[neighborPos.x, neighborPos.y].parent = new List<Node>();
                    map[neighborPos.x, neighborPos.y].parent.Add(openList[0]);
                    openList.Add(map[neighborPos.x, neighborPos.y]);
                }
            }

            neighborPos = openList[0].pos + DOWN_RIGHT;
            if (neighborPos.x < mapSize.x && neighborPos.y >= 0)
            {
                if (map[neighborPos.x, neighborPos.y].tile == goalTile)
                {
                    goalReach = true;
                    map[neighborPos.x, neighborPos.y].parent = new List<Node>();
                    map[neighborPos.x, neighborPos.y].parent.Add(openList[0]);
                    goal = map[neighborPos.x, neighborPos.y];
                    continue;
                }

                if (map[neighborPos.x, neighborPos.y].tile != wallTill || closeList.Contains(map[neighborPos.x, neighborPos.y]))
                {
                    map[neighborPos.x, neighborPos.y].countTile = openList[0].countTile + 1;
                    map[neighborPos.x, neighborPos.y].distFromGoal =
                        Vector2.Distance(map[neighborPos.x, neighborPos.y].pos, goalPos);
                    map[neighborPos.x, neighborPos.y].parent = new List<Node>();
                    map[neighborPos.x, neighborPos.y].parent.Add(openList[0]);
                    openList.Add(map[neighborPos.x, neighborPos.y]);
                }
            }

            closeList.Add(openList[0]);
            openList.RemoveAt(0);

            openList = openList.OrderBy(n1 => n1.GetTileFullCoast()).ToList();
        }

        Node path = goal;
        bool backToStart = false;
        while (!backToStart)
        {
            if (map[path.pos.x, path.pos.y].parent[0].tile == startingTile)
            {
                backToStart = true;
                continue;
            }
            tilemap.SetTile(new Vector3Int(path.parent[0].pos.x, path.parent[0].pos.y, 0), wayTile);
            path = path.parent[0];
        }
    }



    void OnDrawGizmos()
    {
        if(!isStarted)
            return;

        Gizmos.color = Color.red;

        Gizmos.DrawLine(Vector3.zero, new Vector3(mapSize.x, 0));
        Gizmos.DrawLine(Vector3.zero, new Vector3(0, mapSize.y));
        Gizmos.DrawLine(new Vector3(mapSize.x, 0), new Vector3(mapSize.x, mapSize.y));
        Gizmos.DrawLine(new Vector3(0, mapSize.y), new Vector3(mapSize.x, mapSize.y));
    }
}
