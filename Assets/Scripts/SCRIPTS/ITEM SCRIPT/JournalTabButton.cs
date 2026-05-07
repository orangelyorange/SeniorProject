using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class JournalTabButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private JournalTab tab;
    [SerializeField] private TextMeshProUGUI label;

    public void Initialize(JournalTab tabType, string labelText)
    {
        tab = tabType;
        if (label == null)
        {
            label = GetComponentInChildren<TextMeshProUGUI>(true);
        }

        if (label != null)
        {
            label.text = labelText;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SCRIPTS.ITEM_SCRIPT.JournalManager.Instance != null)
        {
            SCRIPTS.ITEM_SCRIPT.JournalManager.Instance.SetTab(tab);
        }
    }
}
