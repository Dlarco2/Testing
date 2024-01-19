using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    ExternalFootController externalFoot;
    FootController foot;
    HeadController head;

    [SerializeField] GameObject bullet;

    Rigidbody2D rb;

    Vector2 contactDirection = Vector2.zero;
    Vector2 slideDirection = Vector2.zero;

    Vector2 moveDirection = Vector2.zero;

    float speed = 10;
    float verticalSpeed = 0;
    float gravity = 30;
    float jumpSpeed = 15;

    bool isGrounded = false;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Start is called before the first frame update
    void Start()
    {
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.useFullKinematicContacts = true;

        externalFoot = GetComponentInChildren<ExternalFootController>();
        foot = GetComponentInChildren<FootController>();
        head = GetComponentInChildren<HeadController>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection = Vector2.zero;
        if (Input.GetKey(KeyCode.D)) moveDirection += Vector2.right;
        if (Input.GetKey(KeyCode.A)) moveDirection += Vector2.left;
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded) VerticalSpeed = jumpSpeed;

        if (foot.IsGrounded && !head.IsGrounded) transform.position += Vector3.up * 0.02f;
        if (!foot.IsGrounded && head.IsGrounded) transform.position += Vector3.down * 0.02f;

        slideDirection = GetSlideVector(externalFoot.contactDirection);
        //slideDirection = GetSlideVector(contactDirection);
    }

    private void FixedUpdate()
    {
        if (isGrounded && VerticalSpeed < 0) VerticalSpeed = 0;
        else if (isGrounded == false) VerticalSpeed -= gravity * Time.fixedDeltaTime;
        rb.velocity = moveDirection * Speed + Vector2.up * VerticalSpeed + SlideVector;
        //rb.velocity = moveDirection * Speed + Vector2.up * VerticalSpeed + slideDirection;// * Mathf.Sin(GetSlideAngle(contactDirection));

        //if (foot.IsGrounded && !head.IsGrounded) rb.velocity = Vector2.right * rb.velocity.x + Vector2.up * 5f;
        //if (!foot.IsGrounded && head.IsGrounded) rb.velocity = Vector2.right * rb.velocity.x + Vector2.down * 5f;

    }

    Vector2 GetSlideVector(Vector2 normal)
    {
        Vector2 slider = Vector2.zero;

        normal = new Vector2(-normal.y, normal.x);

        if (normal.x > 0)
        {
            float angle = Mathf.Atan(normal.y / normal.x) * Mathf.Rad2Deg;
            print(angle);
            if (-90 < angle && angle < -25) slider = normal;
            if (25 < angle && angle < 90) slider = -normal;
        }
        else
        {
            if (normal.y > 0) slider = Vector2.left;
            if (normal.y < 0) slider = Vector2.right;
        }
        
        return slider;
    }

    float GetSlideAngle(Vector2 normal)
    {
        float angle = 0;
        normal = new Vector2(-normal.y, normal.x);

        if (normal.x > 0) angle = Mathf.Atan(normal.y / normal.x);
        return angle;
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

    Vector2 SlideVector
    {
        get { return 10 * Mathf.Sin(GetSlideAngle(contactDirection)) * slideDirection; }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            contactDirection = (collision.GetContact(0).point - (Vector2)transform.position).normalized;
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.collider.CompareTag("Ground"))
        {
            isGrounded = true;
            contactDirection = (collision.GetContact(0).point - (Vector2)transform.position).normalized;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            isGrounded = false;
            contactDirection = Vector2.zero;
        }
    }


}
