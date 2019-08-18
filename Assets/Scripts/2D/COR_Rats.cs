using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COR_Rats : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private Transform playerTransform;

    private Transform ATransform;

    private bool isRunning = false;

    private float distBeforeMerge = 2.0f;
    private float speedX = 1.0f;
    private float speed = 2.0f;



    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerTransform =  FindObjectOfType<COR_PlayerController>().transform;
        ATransform = transform;
        isRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(ATransform.position.y * 1000.0f);
        MoveToPlayer();
    }

    void MoveToPlayer()
    {
        if(Vector2.Distance(transform.position, playerTransform.position) < 1)
            return;

        if (Mathf.Abs(playerTransform.position.x - transform.position.x) > distBeforeMerge)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(playerTransform.position.x, transform.position.y), speedX * Time.deltaTime);
            return;
        }

        transform.position = Vector2.MoveTowards(transform.position, playerTransform.position, speed * Time.deltaTime);
    }

    void OnDrawGizmos()
    {
        if (!isRunning)
            return;

        Gizmos.color = Color.white;
        Gizmos.DrawSphere(ATransform.position, 0.1f);
    }
}
