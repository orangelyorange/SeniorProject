using UnityEngine;

public class EnemyPatrolAi : MonoBehaviour
{
    public GameObject EnemyPointA;
    public GameObject EnemyPointB;
    private Rigidbody2D rb;
    private Animator animator;
    private Transform currentPoint;
    public float speed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentPoint = EnemyPointB.transform;
        animator.SetBool("isRunning", true);
        
        //Finds the player at the start
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            
        }
    }

    void Update()
    {
        Vector2 point = currentPoint.position - transform.position; //give the direction for enemy
        if (currentPoint == EnemyPointB.transform)
        {
            rb.linearVelocity = new Vector2(speed, 0); //enemy goes to right
        }
        else
        {
            rb.linearVelocity = new Vector2(-speed, 0); // enemy goes to left
        }

        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == EnemyPointB.transform)
        {
            Flip();
            currentPoint = EnemyPointA.transform;
        }
        
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == EnemyPointA.transform)
        {
            Flip();
            currentPoint = EnemyPointB.transform;
        }
    }

    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
