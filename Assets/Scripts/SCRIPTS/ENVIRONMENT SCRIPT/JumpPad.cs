using UnityEngine;

public class JumpPad: MonoBehaviour
{
    [SerializeField] float bounce = 20f;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(Vector2.up * bounce, ForceMode2D.Impulse);
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.PlaySfx(AudioManager.Instance.jumpPad);
            }

        }
    }
}
