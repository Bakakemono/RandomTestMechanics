using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using UnityEngine;
using UnityEditor;

public class Triangulation : MonoBehaviour
{
    [SerializeField] private string seed = "seed";

    [SerializeField] private int pointNumber;
    [SerializeField] private Vector2Int areaSize = new Vector2Int(10, 10);
    [SerializeField] private float pointSize = 0.1f;

    private Vector2 equidistance = Vector2.zero;

    private Vector2[,] linePoints;
    private List<Vector2> originPoint;
    private List<Vector2> originDirection;


    private Vector2[] points;
    private bool draw = false;

    public void doTriangulation()
    {
        originPoint = new List<Vector2>();
        originDirection = new List<Vector2>();
        linePoints = new Vector2[3,2];
        Random.InitState(seed.GetHashCode());
        points = new Vector2[pointNumber];

        for (int i = 0; i < pointNumber; i++)
        {
            points[i] = new Vector2(Random.Range(0f, areaSize.x), Random.Range(0f, areaSize.y));
        }

        for (int i = 0; i < 3; i++)
        {
            int first = i;
            int second;
            if (i == 0)
            {
                second = 2;
            }
            else
            {
                second = i - 1;
            }

            Vector2 origin = (points[first] + points[second]) / 2;
            Vector2 direction = (points[second] - origin);
            originDirection.Add(direction.normalized);
            originPoint.Add(origin);
            
            linePoints[i, 0] = origin + new Vector2(direction.x, -direction.y);
            linePoints[i, 1] = origin + new Vector2(-direction.x, direction.y);


        }
        draw = true;
    }

    void OnDrawGizmos()
    {
        Draw();
    }

    private void Draw()
    {
        if(!draw)
            return;

        Gizmos.color = Color.black;
        Vector3 size = new Vector3(areaSize.x, areaSize.y);
        Gizmos.DrawWireCube(size / 2, size);

        Gizmos.color = Color.red;

        foreach (Vector2 point in points)
        {
            Gizmos.DrawSphere(point, pointSize);
        }

        //for (int i = 0; i < 3; i++)
        //{
        //    //Gizmos.DrawLine(linePoints[i, 0], linePoints[i, 1]);
        //}

        //Gizmos.DrawLine(points[0], points[2]);
        //Gizmos.DrawLine((points[0] + points[2]) / 2, new Vector2(((points[0] + points[2]) / 2 + originDirection[0]).x, -((points[0] + points[2]) / 2 + originDirection[0]).y));
        Gizmos.DrawLine(linePoints[0, 0], linePoints[0, 1]);


    }
}

[CustomEditor(typeof(Triangulation))]
public class TriangulationCustomInspector : Editor
{
    public override void OnInspectorGUI()
    {
        Triangulation triangulation = (Triangulation)target;

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate"))
        {
            triangulation.doTriangulation();
        }
        EditorGUILayout.EndHorizontal();

        DrawDefaultInspector();
    }
}