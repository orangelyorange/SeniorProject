using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    public HealthSystem playerHealth; //calls the health system

    // kapag natamaan ang player ng bullet
    private void OnTriggerEnter2D(Collider2D collision) 
    {
        EnemyBullet bullet = collision.gameObject.GetComponent<EnemyBullet>(); //calls enemy bullet prefab
        if (bullet != null)
        {
            playerHealth.TakeDamage(bullet.damage);
        }
    }
}
