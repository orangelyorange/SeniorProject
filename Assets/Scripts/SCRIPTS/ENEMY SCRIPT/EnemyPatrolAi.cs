using UnityEngine;

public class EnemyPatrolAi : MonoBehaviour
{
    public GameObject enemyPointA;
    public GameObject enemyPointB;
    private Rigidbody2D rb;
    private Animator animator;
    private Transform currentPoint;
    public float speed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentPoint = enemyPointB.transform;
        animator.SetBool("isRunning", true);
    }

    void Update()
    {
        Vector2 point = currentPoint.position - transform.position; //give the direction for enemy
        if (currentPoint == enemyPointB.transform)
        {
            rb.linearVelocity = new Vector2(speed, 0); //enemy goes to right
        }
        else
        {
            rb.linearVelocity = new Vector2(-speed, 0); // enemy goes to left
        }

        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == enemyPointB.transform)
        {
            Flip();
            currentPoint = enemyPointA.transform;
        }
        
        if (Vector2.Distance(transform.position, currentPoint.position) < 0.5f && currentPoint == enemyPointA.transform)
        {
            Flip();
            currentPoint = enemyPointB.transform;
        }
    }

    private void Flip()
    {
        Vector3 localScale = transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }
}
