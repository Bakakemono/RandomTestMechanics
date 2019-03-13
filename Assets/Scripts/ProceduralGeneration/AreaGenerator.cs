using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AreaGenerator : MonoBehaviour
{
    struct Point
    {
        public Vector2 postion;
        public List<Point> child;
    }

    [SerializeField] private string seed = "Hello";


    [SerializeField] private Vector2Int mapSize = new Vector2Int(100, 100);
    [SerializeField] private int areaNumber = 10;

    private List<Point> areaList = new List<Point>();
    private Point[,] map;

    private Color[] areaColor;

    public void clear()
    {
        areaList = new List<Point>();
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
                map[x, y].postion = new Vector2Int(x, y);
            }
        }

        areaColor = new Color[areaNumber];
        areaList = new List<Point>();
        for (int i = 0; i < areaNumber; i++)
        {
            Point area;
            area.postion = new Vector2Int(Random.Range(0, mapSize.x), Random.Range(0, mapSize.y));
            area.child = new List<Point>();
            
            areaList.Add(area);
            areaColor[i] = new Color(Random.value, Random.value, Random.value);
        }

        foreach (Point point in map)
        {
            float minimalDistance = float.MaxValue;
            int areaIndex = 0;
            for (int i = 0; i < areaNumber; i++)
            {
                float newDistance = Vector2.Distance(point.postion, areaList[i].postion);
                if (newDistance < minimalDistance)
                {
                    areaIndex = i;
                    minimalDistance = newDistance;
                }
            }
            areaList[areaIndex].child.Add(point);
        }
    }

    void OnDrawGizmos()
    {
        if(areaColor.Length == 0)
            return;
        for (int i = 0; i < areaNumber; i++)
        {
            Gizmos.color = areaColor[i];

            foreach (Point point in areaList[i].child)
            {
                Gizmos.DrawCube(point.postion + Vector2.one / 2, new Vector2(1, 1));
            }
            Gizmos.DrawWireSphere(areaList[i].postion + Vector2.one / 2, 1);
        }
    }


}

[CustomEditor(typeof(AreaGenerator))]
public class AreaGeneratorCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        AreaGenerator areaGenerator = (AreaGenerator)target;

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