using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Random = UnityEngine.Random;

public class VoronoiAreaGenerator : MonoBehaviour
{
    struct Point
    {
        public Vector2 position;
        public List<Point> child;
        public int areaIndex;
        public Color color;

        public List<int> neighborArea;

    }

    [SerializeField] private string seed = "Hello";
    [SerializeField] private bool drawAreaCenter = false;
    [SerializeField] private bool drawTriangulation = false;

    private bool draw = false;
    [SerializeField] private bool drawMap = false;
    [SerializeField] private bool drawShape = false;

    [SerializeField] private Vector2Int mapSize = new Vector2Int(100, 100);
    [SerializeField] private int areaNumber = 10;

    private Point[] areaList;
    private Point[,] map;

    private Color[] areaColor;

    public void clear()
    {
        draw = false;   
        areaList = new Point[areaNumber];
        areaColor = new Color[0];
    }

    public void StartGeneration()
    {
        Random.InitState(seed.GetHashCode());
        map = new Point[mapSize.x, mapSize.y];
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                map[x, y].position = new Vector2Int(x, y);
            }
        }

        areaColor = new Color[areaNumber];
        areaList = new Point[areaNumber];
        for (int i = 0; i < areaNumber; i++)
        {
            Point area;
            area.position = new Vector2Int(Random.Range(0, mapSize.x), Random.Range(0, mapSize.y));
            area.child = new List<Point>();
            area.areaIndex = areaNumber;
            area.color = new Color(Random.value, Random.value, Random.value);
            area.neighborArea = new List<int>();

            areaList[i] = area;
        }

        #region Sort area by X
        Point[] areaSorted = new Point[areaNumber];
        List<int> areaAlreadySort = new List<int>();
        areaSorted[0] = areaList[0];
        for (int i = 0; i < areaNumber; i++)
        {
            float x = 0;
            Point newLow = new Point();
            int lowestArea = 0;

            for (int j = 0; j < areaNumber; j++)
            {
                bool isAlreadyUse = false;
                foreach (int i1 in areaAlreadySort)
                {
                    if (i1 == j)
                    {
                        isAlreadyUse = true;
                        break;
                    }
                }

                if (isAlreadyUse)
                {
                    continue;
                }
                x = areaList[j].position.x;
                newLow = areaList[j];
                lowestArea = j;
            }

            for (int j = 0; j < areaNumber; j++)
            {
                bool isAlreadyUse = false;
                foreach (int i1 in areaAlreadySort)
                {
                    if (i1 == j)
                    {
                        isAlreadyUse = true;
                        break;
                    }
                }
                if(isAlreadyUse)
                    continue;

                if (x > areaList[j].position.x)
                {
                    x = areaList[j].position.x;
                    lowestArea = j;
                    newLow = areaList[j];
                }
            }
            areaSorted[i] = newLow;
            areaAlreadySort.Add(lowestArea);
        }
        areaList = areaSorted;
        #endregion

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                float minimalDistance = float.MaxValue;
                int areaIndex = 0;
                for (int i = 0; i < areaNumber; i++)
                {
                    float newDistance = Vector2.Distance(map[x, y].position, areaList[i].position);
                    if (newDistance < minimalDistance)
                    {
                        areaIndex = i;
                        minimalDistance = newDistance;
                    }
                }

                map[x, y].areaIndex = areaIndex;
                areaList[areaIndex].child.Add(map[x, y]);
            }
        }

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                List<int> neighborList = new List<int>();

                if (x > 0 && map[x, y].areaIndex != map[x - 1, y].areaIndex)
                {
                    neighborList.Add(map[x - 1, y].areaIndex);
                }
                if (x < mapSize.x - 1 && map[x, y].areaIndex != map[x + 1, y].areaIndex)
                {
                    neighborList.Add(map[x + 1, y].areaIndex);
                }
                if (y > 0 && map[x, y].areaIndex != map[x, y - 1].areaIndex)
                {
                    neighborList.Add(map[x, y - 1].areaIndex);
                }
                if (y < mapSize.y - 1 && map[x, y].areaIndex != map[x, y + 1].areaIndex)
                {
                    neighborList.Add(map[x, y + 1].areaIndex);
                }

                foreach (int neighborIndex in neighborList)
                {
                    bool isAlreadyRegister = false;
                    foreach (int neighborAreaIndex in areaList[map[x, y].areaIndex].neighborArea)
                    {
                        if (neighborIndex == neighborAreaIndex)
                        {
                            isAlreadyRegister = true;
                            break;
                        }
                    }

                    if(!isAlreadyRegister)
                        areaList[map[x, y].areaIndex].neighborArea.Add(neighborIndex);
                }
            }
        }

        for (int i = 0; i < areaNumber; i++)
        {
            Vector2 addPosAllChild = Vector2.zero;
            foreach (Point point in areaList[i].child)
            {
                addPosAllChild += point.position;
            }
            areaList[i].position = addPosAllChild / areaList[i].child.Count;
        }



        // Create water area for fun :D
        //for (int x = 0; x < mapSize.x; x++)
        //{
        //    areaList[map[x, 0].areaIndex].color = Color.blue;
        //    areaList[map[x, mapSize.y - 1].areaIndex].color = Color.blue;
        //}
        //for (int y = 0; y < mapSize.y; y++)
        //{
        //    areaList[map[0, y].areaIndex].color = Color.blue;
        //    areaList[map[mapSize.x - 1, y].areaIndex].color = Color.blue;
        //}
        draw = true;
    }

    void OnDrawGizmos()
    {
        Draw();
    }


    void Draw()
    {
        if(!draw)
            return;

        if (drawMap)
        {
            if (areaColor.Length == 0)
                return;

            for (int i = 0; i < areaNumber; i++)
            {
                Gizmos.color = areaList[i].color;

                foreach (Point point in areaList[i].child)
                {
                    Gizmos.DrawCube(point.position, Vector3.one);
                }
            }
        }

    Gizmos.color = Color.black;
        if (drawAreaCenter)
            for (int i = 0; i < areaNumber; i++)
            {
                Gizmos.DrawSphere(areaList[i].position, 0.5f);
                Handles.Label(areaList[i].position + Vector2.one, i.ToString());

            }

        Gizmos.color = Color.red;
        if (drawTriangulation)
        {
            for (int i = 0; i < areaNumber; i++)
            {
                foreach (int neighbors in areaList[i].neighborArea)
                {
                    Gizmos.DrawLine(areaList[i].position, areaList[neighbors].position);
                }
            }
        }
    }
}



[CustomEditor(typeof(VoronoiAreaGenerator))]
public class AreaGeneratorCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        VoronoiAreaGenerator areaGenerator = (VoronoiAreaGenerator)target;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate"))
        {
            areaGenerator.StartGeneration();
        }

        if (GUILayout.Button("Reset"))
        {
            areaGenerator.clear();
        }
        EditorGUILayout.EndHorizontal();

        DrawDefaultInspector();
    }
}