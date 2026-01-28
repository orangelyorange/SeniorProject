using UnityEngine;
using System.Collections;
public class EnemyHealthSystem : MonoBehaviour
{
    public int enemyHealth = 3;
    private int currentHealth;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        currentHealth = enemyHealth;
    }

    public void TakeDamage(int damage)
    {
        enemyHealth -= damage;
        StartCoroutine(BlinkRed());
        if (enemyHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }

    private IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.5f);
        spriteRenderer.color = Color.white;
    }
}
