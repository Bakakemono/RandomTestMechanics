using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COR_SpawnManager : MonoBehaviour
{
    [Header("Enemies Prefabs")]
    [SerializeField] private GameObject rats;
    
    [Header("RoadInfo")]
    [SerializeField] private SpriteRenderer roadSprite;
    
    private float roadLength;

    private bool isRunning = false;

    // Start is called before the first frame update
    void Start()
    {
        roadLength = roadSprite.bounds.size.y;
        isRunning = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnDrawGizmos()
    {
        if (!isRunning)
            return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(-10.0f, roadLength / 2), new Vector3(10.0f, roadLength / 2));
        Gizmos.DrawLine(new Vector3(-10.0f, -roadLength / 2), new Vector3(10.0f, -roadLength / 2));
    }
}
