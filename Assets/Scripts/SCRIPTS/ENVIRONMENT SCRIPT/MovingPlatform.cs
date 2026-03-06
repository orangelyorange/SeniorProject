using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Waypoints")]
    public GameObject pointA;
    public GameObject pointB;
    
    [Header("Movement")]
    public float speed;

    private Rigidbody2D rb;
    private Transform currentPoint;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        currentPoint = pointB.transform; // Start by moving towards Point B
    }

    // Changed to FixedUpdate because we are manipulating Rigidbody physics
    void FixedUpdate() 
    {
        // Calculate the direction towards the current target point
        Vector2 direction = (currentPoint.position - transform.position).normalized;
        
        // Move the platform using the Rigidbody velocity
        rb.linearVelocity = direction * speed;

        // Check if the platform has reached the current point
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f)
        {
            // Swap the target point
            if (currentPoint == pointB.transform)
            {
                currentPoint = pointA.transform;
            }
            else
            {
                currentPoint = pointB.transform;
            }
        }
    }

    // Parent the player to the platform so they move with it
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    // Unparent the player when they leave the platform
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}