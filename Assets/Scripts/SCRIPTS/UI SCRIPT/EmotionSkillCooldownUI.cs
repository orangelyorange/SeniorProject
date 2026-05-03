using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EmotionSkillCooldownUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EmotionSkillManager skillManager;
    [SerializeField] private JoySkill joySkill;
    [SerializeField] private SadnessSkill sadnessSkill;
    [SerializeField] private RageSkill rageSkill;

    [Header("UI Elements")]
    [SerializeField] private GameObject container;
    [SerializeField] private Image iconImage;
    [SerializeField] private Image cooldownFillImage;
    [SerializeField] private TMP_Text cooldownText;

    [Header("Placeholder Colors")]
    [SerializeField] private Color joyColor = new Color(0.98f, 0.86f, 0.2f, 1f);
    [SerializeField] private Color sadnessColor = new Color(0.3f, 0.6f, 1f, 1f);
    [SerializeField] private Color rageColor = new Color(0.95f, 0.25f, 0.25f, 1f);

    private void Awake()
    {
        if (container == null)
        {
            container = gameObject;
        }

        if (cooldownFillImage != null)
        {
            cooldownFillImage.type = Image.Type.Filled;
        }
    }

    private void Start()
    {
        ResolveReferencesIfNeeded();
    }

    private void Update()
    {
        if (skillManager == null)
        {
            ResolveReferencesIfNeeded();
        }

        if (skillManager == null)
        {
            SetVisible(false);
            return;
        }

        switch (skillManager.currentSkill)
        {
            case EmotionSkill.Joy:
                UpdateJoyCooldown();
                break;
            case EmotionSkill.Sadness:
                UpdateSadnessCooldown();
                break;
            case EmotionSkill.Rage:
                UpdateRageCooldown();
                break;
            default:
                SetVisible(false);
                break;
        }
    }

    private void ResolveReferencesIfNeeded()
    {
        if (skillManager == null)
        {
            skillManager = FindObjectOfType<EmotionSkillManager>();
        }

        if (skillManager != null)
        {
            if (joySkill == null)
            {
                joySkill = skillManager.GetComponent<JoySkill>();
            }

            if (sadnessSkill == null)
            {
                sadnessSkill = skillManager.GetComponent<SadnessSkill>();
            }

            if (rageSkill == null)
            {
                rageSkill = skillManager.GetComponent<RageSkill>();
            }
        }
    }

    private void UpdateJoyCooldown()
    {
        if (joySkill == null)
        {
            SetVisible(false);
            return;
        }

        UpdateUI(joySkill.HasSkillBeenUsed, joySkill.CooldownRemaining, joySkill.CooldownDuration, joyColor);
    }

    private void UpdateSadnessCooldown()
    {
        if (sadnessSkill == null)
        {
            SetVisible(false);
            return;
        }

        UpdateUI(sadnessSkill.HasSkillBeenUsed, sadnessSkill.CooldownRemaining, sadnessSkill.CooldownDuration, sadnessColor);
    }

    private void UpdateRageCooldown()
    {
        if (rageSkill == null)
        {
            SetVisible(false);
            return;
        }

        UpdateUI(rageSkill.HasSkillBeenUsed, rageSkill.CooldownRemaining, rageSkill.CooldownDuration, rageColor);
    }

    private void UpdateUI(bool hasUsed, float remaining, float duration, Color iconColor)
    {
        if (!hasUsed)
        {
            SetVisible(false);
            return;
        }

        SetVisible(true);

        if (iconImage != null)
        {
            iconImage.color = iconColor;
        }

        if (cooldownFillImage != null)
        {
            float fillAmount = duration > 0f ? Mathf.Clamp01(remaining / duration) : 0f;
            cooldownFillImage.fillAmount = fillAmount;
        }

        if (cooldownText != null)
        {
            cooldownText.text = remaining > 0f ? Mathf.CeilToInt(remaining).ToString() : string.Empty;
        }
    }

    private void SetVisible(bool isVisible)
    {
        if (container != null && container != gameObject)
        {
            if (container.activeSelf != isVisible)
            {
                container.SetActive(isVisible);
            }
            return;
        }

        if (iconImage != null)
        {
            iconImage.enabled = isVisible;
        }

        if (cooldownFillImage != null)
        {
            cooldownFillImage.enabled = isVisible;
        }

        if (cooldownText != null)
        {
            cooldownText.enabled = isVisible;
        }
    }
}
