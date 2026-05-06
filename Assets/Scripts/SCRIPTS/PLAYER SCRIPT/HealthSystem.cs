using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HealthSystem : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Slider healthBar;

    private const string PendingRespawnSfxKey = "PendingRespawnSfx";

    public int PlayerHealth;
    public int PlayerMaxHealth = 4;

    private SpriteRenderer spriteRenderer;
    private Animator animator;

    private bool isDead = false;

    private void Start()
    {
        PlayerHealth = PlayerMaxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        UpdateHealthBar(); // ⭐ initialize UI
    }

    // ---------------- DAMAGE ----------------
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        PlayerHealth -= damage;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySfx(AudioManager.Instance.playerTakeDamage);

        StartCoroutine(BlinkRed());

        if (PlayerHealth <= 0)
        {
            PlayerHealth = 0;
            UpdateHealthBar();
            StartCoroutine(Die());
            return;
        }

        UpdateHealthBar();
    }

    // ---------------- HEAL ----------------
    public void TakeHealing(int healing)
    {
        if (PlayerHealth <= 0) return;

        PlayerHealth += healing;

        if (PlayerHealth > PlayerMaxHealth)
            PlayerHealth = PlayerMaxHealth;

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySfx(AudioManager.Instance.playerHeal);

        StartCoroutine(BlinkGreen());

        UpdateHealthBar();
    }

    // ---------------- DEATH ----------------
    public IEnumerator Die()
    {
        if (isDead) yield break;

        isDead = true;

        animator.SetBool("isDead", true);

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlaySfx(AudioManager.Instance.playerDeath);

        Player player = GetComponent<Player>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        player.enabled = false;
        rb.linearVelocity = Vector2.zero;

        yield return new WaitForSeconds(0.5f);

        // ⭐ RESPAWN
        player.Respawn();

        yield return new WaitForEndOfFrame();

        player.enabled = true;

        // reset animator fully
        animator.Rebind();
        animator.Update(0f);
        animator.SetBool("isDead", false);

        isDead = false;

        // ⭐ FULL HEALTH RESET ON RESPAWN
        ResetHealth();
    }

    // ---------------- RESPAWN RESET ----------------
    public void ResetHealth()
    {
        PlayerHealth = PlayerMaxHealth;
        isDead = false;

        if (animator != null)
            animator.SetBool("isDead", false);

        if (spriteRenderer != null)
            spriteRenderer.color = Color.white;

        UpdateHealthBar();
    }

    // ---------------- UI ----------------
    private void UpdateHealthBar()
    {
        if (healthBar == null) return;

        healthBar.value = (float)PlayerHealth / PlayerMaxHealth;
    }

    // ---------------- VISUAL FEEDBACK ----------------
    public IEnumerator BlinkRed()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }

    public IEnumerator BlinkGreen()
    {
        spriteRenderer.color = Color.green;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.color = Color.white;
    }

    // ---------------- SFX FLAG ----------------
    public static bool ConsumeRespawnSfxRequest()
    {
        if (PlayerPrefs.GetInt(PendingRespawnSfxKey, 0) == 0)
            return false;

        PlayerPrefs.SetInt(PendingRespawnSfxKey, 0);
        PlayerPrefs.Save();
        return true;
    }
}