using UnityEngine;

public class EnemyAOEAttack : MonoBehaviour
{
    [Header("AOE Wave Settings")] 
    public int damage = 1; //wave damage
    public float speed = 2f; //wave speed
    public float lifeTime = 3f; //how long the wave will last
    
    [Header("Player Targetting")]
    public string targetTag = "Player";
    
    public void Start()
    {
        //Destroys the wave after seconds so it does not fly forever
        Destroy(gameObject, lifeTime);
    }

    public void Update()
    {
        //Moves the wave forward to the right
        transform.Translate(Vector2.right * speed *Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        //checks if the game object hits the needed tag which is the player tag
        //make sure 'Player" tag is assigned to player
        if (other.CompareTag(targetTag))
        {
            HealthSystem playerHealth = other.GetComponent<HealthSystem>();//finds the health system attached to player

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log("Player has sustained damage");
                Destroy(gameObject); //destroys object after colliding with player
            }
        } 
    }

}
