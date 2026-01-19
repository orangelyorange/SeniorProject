using UnityEngine;
public class EnemyShooting : MonoBehaviour
{
    public GameObject Bullet;
    public Transform BulletPosition;
    private GameObject player; //reference the player game object   
    private float timer; // controls the frequency of the bullet spawning

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player"); //finds the player object with Player Tag
    }
    void Update()
    {
        //checks player distance to enemy
        float distance = Vector2.Distance(transform.position,  player.transform.position);
        
        //checks if player is in enemy range for detection
        if (distance < 10f)
        {
            timer += Time.deltaTime;
            
            if (timer >= 2)
            {
                timer = 0;
                shoot();
            }
        }
    }

    void shoot()
    {
        //spawn bullet
        GameObject newBullet = Instantiate(Bullet, BulletPosition.position, Quaternion.identity);
        
        Vector2 direction =  (player.transform.position - BulletPosition.position).normalized;
        float angle =  Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        newBullet.transform.rotation = Quaternion.Euler(0, 0, angle);
        
         
    }
}
