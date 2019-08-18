using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class COR_PlayerController : MonoBehaviour
{
    [SerializeField] private float yPos = 0;
    [SerializeField] private float speed = 10;

    [SerializeField] private Vector2 startPosAttack= new Vector2(1, 0);
    [SerializeField] private float rangeAttack = 1;
    [SerializeField] private float attackScope = 1;

    private Transform playerTransform;

    private SpriteRenderer spriteRenderer;

    private bool isRunning = false;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        isRunning = true;
    }

    // Update is called once per frame
    void Update()
    {
        spriteRenderer.sortingOrder = -Mathf.RoundToInt(playerTransform.position.y * 1000.0f);
        Vector2 pos = playerTransform.position;
        playerTransform.position += new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * Time.deltaTime * speed;
    }

    void OnDrawGizmos()
    {
        if (!isRunning)
            return;

        Gizmos.color = Color.white;

        Gizmos.DrawLine((Vector2)playerTransform.position + startPosAttack + new Vector2(0, attackScope / 2), (Vector2)playerTransform.position + startPosAttack + new Vector2(rangeAttack, attackScope / 2));
        Gizmos.DrawLine((Vector2)playerTransform.position + startPosAttack + new Vector2(0, -attackScope / 2), (Vector2)playerTransform.position + startPosAttack + new Vector2(rangeAttack, -attackScope / 2));

    }
}
