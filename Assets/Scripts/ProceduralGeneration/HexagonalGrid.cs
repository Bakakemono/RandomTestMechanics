using System;
using System.Collections;
using UnityEngine;

public class HexagonalGrid : MonoBehaviour
{
    struct HexagonalCell
    {
        public Vector3Int gridPostion;
        public Vector2 worldPosition;

        public float[] corners;
    }

    [SerializeField] private float size = 1;
    private float SQRT_THREE = Mathf.Sqrt(3);
    private float width;
    private float height;


    [SerializeField] private Vector2Int mapSize = new Vector2Int(10 , 10);

    private HexagonalCell[,] map;

    void Start()
    {
        width = SQRT_THREE * size;
        height = 2 * size;

        map = new HexagonalCell[mapSize.x, mapSize.y];

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                if (y % 2 == 0)
                {
                    map[x, y].worldPosition = new Vector2(x * width, y * height);
                }
                else
                {
                    map[x, y].worldPosition = new Vector2(x * width + width / 2, y * height);
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        foreach (HexagonalCell hexagonalCell in map)
        {
            Gizmos.DrawWireSphere(hexagonalCell.worldPosition, width / 2);
        }
    }
}
