using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JournalEntry : MonoBehaviour, IPointerClickHandler
{
    public TextMeshProUGUI journalTitle;
    public TextMeshProUGUI journalContent;

    private JournalCollectedPage pageData;
    private Action<JournalCollectedPage> onSelected;

    // called by the manager when prefab is spawned
    public void Setup(string newTitle, string newContent)
    {
        if (journalTitle != null)
        {
            journalTitle.text = newTitle;
        }

        if (journalContent != null)
        {
            journalContent.text = newContent;
        }
    }

    public void Setup(JournalCollectedPage page, Action<JournalCollectedPage> onSelect)
    {
        pageData = page;
        onSelected = onSelect;
        Setup(page?.title ?? string.Empty, page?.body ?? string.Empty);
        EnsureClickableBackground();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (pageData != null)
        {
            onSelected?.Invoke(pageData);
        }
    }

    private void EnsureClickableBackground()
    {
        if (GetComponent<Image>() == null)
        {
            Image background = gameObject.AddComponent<Image>();
            background.color = new Color(1f, 1f, 1f, 0f);
            background.raycastTarget = true;
        }
    }
}
