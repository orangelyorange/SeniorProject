using Unity.VisualScripting;
using UnityEngine;

public class MovingPlatform_Vertical : MonoBehaviour

{
    public Transform pointC;
    public Transform pointD;
    public float speed;

    private Vector3 nextPosition;

    private void Start()
    {
       nextPosition = pointD.position;
    }

    //get platform moving
    private void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, nextPosition, speed * Time.deltaTime);

        if (transform.position == nextPosition)
        {
            nextPosition = (nextPosition == pointC.position) ? pointD.position : pointC.position;
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

