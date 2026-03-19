using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatform_Horizontal : MonoBehaviour

/* first draft
{
    public Transform PointA, PointB;
    public float speed;
    Vector3 targetPoint;

    private void Start()
    { 
        targetPoint = PointB.position;
    }

    private void Update()
    {
        //when player moves close enought to pointA, its new target will be pointB
        if (Vector2.Distance(transform.position, PointA.position) < 0.05f)
        {
            targetPoint = PointB.position;
        }

        //when player moves close enoughto pointB, its new target will be pointA
        if (Vector2.Distance(transform.position, PointB.position) < 0.05f)
        {
            targetPoint = PointA.position;
        }

    //move platform using MoveTowards
        transform.position = Vector3.MoveTowards(transform.position, targetPoint, speed * Time.deltaTime);
    }

    //when player touches platform, it becomes the child of platform and move with it
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = this.transform;
        }
    }

    //when player stop touching platform, player becomes free
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }

}

*/

{
    public Transform pointA;
    public Transform pointB;
    public float speed;

    private Vector3 nextPosition;

   //starting point si B
    private void Start()
    {
       nextPosition = pointB.position;
    }

    //get platform moving
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);

        if (transform.position == nextPosition)
        {
            nextPosition = (nextPosition == pointA.position) ? pointB.position : pointA.position;
        }
    }

    //parents platform to player when player touches it
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = transform;
        }
    }

    //unparents platform to player when player leaves
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.parent = null;
        }
    }

}

