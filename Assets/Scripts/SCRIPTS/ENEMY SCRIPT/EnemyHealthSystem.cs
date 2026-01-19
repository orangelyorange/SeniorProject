using UnityEngine;
using System.Collections;
public class EnemyHealthSystem : MonoBehaviour
{
    public int EnemyHealth = 3;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void TakeDamage(int damage)
    {
        EnemyHealth -= damage;
        StartCoroutine(BlinkRed());
        if (EnemyHealth <= 0)
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
