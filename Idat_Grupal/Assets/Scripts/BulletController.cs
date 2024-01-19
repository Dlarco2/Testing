using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    CircleCollider2D col;

    Vector2 moveDirection = Vector2.zero;

    float lifeTimer = 0;

    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        col.isTrigger = true;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += 15 * Time.deltaTime * (Vector3) moveDirection;

        lifeTimer += Time.deltaTime;
        if (lifeTimer >= 3) ResetBullet();
    }

    void ResetBullet()
    {
        lifeTimer = 0;
        this.gameObject.SetActive(false);
    }

    public Vector2 MoveDirection
    {
        get { return moveDirection; }
        set { moveDirection = value; }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            ResetBullet();
        }
    }
}
