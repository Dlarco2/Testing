using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExternalFootController : MonoBehaviour
{
    internal Vector2 contactDirection = Vector2.zero;

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            contactDirection = (collision.GetContact(0).point - (Vector2)transform.position).normalized;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Ground"))
        {
            contactDirection = Vector2.zero;
        }
    }
}
