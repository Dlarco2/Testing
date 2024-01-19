using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerDynamic : MonoBehaviour
{
    FootController foot;

    List<BulletController> bullets = new List<BulletController>();
    [SerializeField] GameObject bullet;

    Rigidbody2D rb;

    Vector2 moveDirection = Vector2.zero;

    float speed = 10;
    float verticalSpeed = 0;
    float gravity = 30;
    float jumpSpeed = 15;

    bool isGrounded = false;

    int bulletCounter = 0;

    float shootTimer = 0;
    int shootCounter = 0;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        foot = GetComponentInChildren<FootController>();

        for (int i = 0; i < 15; i++)
        {
            bullets.Add(Instantiate(bullet).GetComponent<BulletController>());
            //bullets[i].transform.SetParent(this.transform);
            bullets[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (shootTimer > 0) shootTimer -= Time.deltaTime;

        moveDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.D)) moveDirection += Vector2.right;
        if (Input.GetKey(KeyCode.A)) moveDirection += Vector2.left;
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) VerticalSpeed = jumpSpeed;

        if (isGrounded) rb.gravityScale = 1;
        else rb.gravityScale = 3;

        //if (Input.GetKeyDown(KeyCode.Mouse0)) Shoot(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        if (Input.GetKey(KeyCode.Mouse0) && shootTimer <= 0)
        {
            Shoot(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (shootCounter < 3)
            {
                shootTimer = 0.15f;
                shootCounter += 1;
            }
            if (shootCounter == 3)
            {
                shootTimer = 0.5f;
                shootCounter = 0;
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            shootCounter = 0;
            shootTimer = 0;
        }

    }

    private void FixedUpdate()
    {
        if (isGrounded && VerticalSpeed < 0) VerticalSpeed = 0;
        else if (isGrounded == false) VerticalSpeed -= gravity * Time.fixedDeltaTime;

        if (foot.IsGrounded) rb.velocity = moveDirection * Speed + Vector2.up * VerticalSpeed;
        else rb.velocity = moveDirection * Speed + Vector2.up * rb.velocityY;

    }

    void Shoot(Vector2 targetPosition)
    {
        if (bullets[bulletCounter].isActiveAndEnabled == false)
        {
            Vector2 direction = targetPosition - (Vector2)transform.position;
            direction = AdjustDirection(direction);

            bullets[bulletCounter].gameObject.SetActive(true);
            bullets[bulletCounter].transform.position = this.transform.position;
            bullets[bulletCounter].MoveDirection = direction;

            bulletCounter = bulletCounter == 14 ? 0 : bulletCounter + 1;
        }
    }

    float GetDistance(Vector2 point, float m)
    {
        float A, B, divider;

        if (m == Mathf.Infinity)
        {
            A = 1;
            B = 0;
        }
        else
        {
            A = m;
            B = -1;
        }

        divider = Mathf.Sqrt(A * A + B * B);

        return Mathf.Abs(A * point.x + B * point.y) / divider;
    }

    Vector2 AdjustDirection(Vector2 direction)
    {
        if (direction.x > 0 && direction.y > 0) // Primer Cuadrante
        {
            if (direction.x > direction.y)
            {
                if (GetDistance(direction, 0) < GetDistance(direction, 1)) direction = Vector2.right;
                else direction = Vector2.right + Vector2.up;
            }
            else //(direction.x <= direction.y)
            {
                if (GetDistance(direction, Mathf.Infinity) < GetDistance(direction, 1)) direction = Vector2.up;
                else direction = Vector2.right + Vector2.up;
            }
        }
        else if (direction.x < 0 && direction.y > 0) // Segundo Cuadrante
        {
            if (-direction.x > direction.y)
            {
                if (GetDistance(direction, 0) < GetDistance(direction, -1)) direction = Vector2.left;
                else direction = Vector2.left + Vector2.up;
            }
            else //(-direction.x <= direction.y)
            {
                if (GetDistance(direction, Mathf.Infinity) < GetDistance(direction, -1)) direction = Vector2.up;
                else direction = Vector2.left + Vector2.up;
            }
        }
        else if (direction.x < 0 && direction.y < 0) // Tercer Cuadrante
        {
            if (-direction.x > -direction.y)
            {
                if (GetDistance(direction, 0) < GetDistance(direction, 1)) direction = Vector2.left;
                else direction = Vector2.left + Vector2.down;
            }
            else //(-direction.x <= -direction.y)
            {
                if (GetDistance(direction, Mathf.Infinity) < GetDistance(direction, 1)) direction = Vector2.down;
                else direction = Vector2.left + Vector2.down;
            }
        }
        else if (direction.x > 0 && direction.y < 0) // Cuarto Cuadrante
        {
            if (direction.x > -direction.y)
            {
                if (GetDistance(direction, 0) < GetDistance(direction, -1)) direction = Vector2.right;
                else direction = Vector2.right + Vector2.down;
            }
            else //(direction.x <= -direction.y)
            {
                if (GetDistance(direction, Mathf.Infinity) < GetDistance(direction, -1)) direction = Vector2.down;
                else direction = Vector2.right + Vector2.down;
            }
        }
        else if (direction.x > 0 && direction.y == 0) direction = Vector2.right;
        else if (direction.x == 0 && direction.y > 0) direction = Vector2.up;
        else if (direction.x < 0 && direction.y == 0) direction = Vector2.left;
        else if (direction.x == 0 && direction.y < 0) direction = Vector2.down;
        else direction = Vector2.right; // x == 0; y == 0

        return direction.normalized;
    }

    internal float Speed
    {
        get
        {
            if (Input.GetKey(KeyCode.LeftShift)) return speed * 1.5f;
            else return speed;
        }
        set { speed = (value < 0 ? 0 : value); }
    }

    internal float VerticalSpeed
    {
        get { return verticalSpeed; }
        set { verticalSpeed = (value < -jumpSpeed ? -jumpSpeed : value); }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }









}
