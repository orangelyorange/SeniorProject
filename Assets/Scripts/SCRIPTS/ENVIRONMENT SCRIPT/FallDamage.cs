using UnityEngine;
using UnityEngine.SceneManagement;

public class FallDamage : MonoBehaviour
{
  public HealthSystem playerHealth;
  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.CompareTag("Player"))
    {
      Destroy(other.gameObject);
      playerHealth.TakeDamage(5);
      playerHealth.Die();
      SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
  }
}
