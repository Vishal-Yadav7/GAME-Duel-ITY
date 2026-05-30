using UnityEngine;

public class ObjectBounce : MonoBehaviour
{
    public float pushBackForce = 180f;   // adjust in Inspector
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Boundary"))
        {
            // Debug log to confirm collision
            Debug.Log(name + " hit the boundary!");

            // Direction away from wall
            Vector2 pushDir = (rb.position - collision.contacts[0].point).normalized;

            // Stop movement and push back
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(pushDir * pushBackForce, ForceMode2D.Impulse);
        }
    }
}
