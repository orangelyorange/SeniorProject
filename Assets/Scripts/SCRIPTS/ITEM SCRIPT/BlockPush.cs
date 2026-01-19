using UnityEngine;

public class BlockPush : MonoBehaviour
{
   public int speed = 2;
   public int distance = 2;
   public bool isPushing = false;
   public KeyCode pushKey = KeyCode.F;

   private Rigidbody2D objectToPush;

   private void Start()
   {
      objectToPush = GetComponent<Rigidbody2D>();
   }
   private void Update()
   {
      if (Input.GetKeyDown(pushKey))
      {
         if (!isPushing)
         {
            Push();
         }
         else
         {
            stopPushing();
         }
      }

      if (isPushing && objectToPush != null)
      {
         Vector3 movement = transform.right * speed * Time.deltaTime;
         objectToPush.AddForce(movement);
      }
   }

   private void Push()
   {
    RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, distance); 
    if (hit.collider != null && hit.collider.CompareTag("PushableObject"))
    {
       Rigidbody2D rb = hit.collider.attachedRigidbody;

       if (objectToPush != null)
       {
          objectToPush = rb;
          isPushing = true;
       }
    }
   }

   private void stopPushing()
   {
      isPushing = false;
      objectToPush = null;
   }
   
   void FixedUpdate()
   {
      if (isPushing && objectToPush != null)
      {
         Vector2 forceDirection = transform.right;
         objectToPush.AddForce(forceDirection * speed);
      }
   }

}
