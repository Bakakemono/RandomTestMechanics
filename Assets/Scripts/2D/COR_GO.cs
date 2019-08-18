using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COR_GO : MonoBehaviour
{
    private Rigidbody2D rigidbody2D;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.velocity = new Vector2(1, 0);
    }

    void Update()
    {
        Debug.Log(Time.time);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector2(transform.position.x, 10), new Vector2(transform.position.x, -10));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(10, 10), new Vector2(10, -10));

    }
}
