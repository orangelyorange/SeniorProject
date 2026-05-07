using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JournalProgressManager : MonoBehaviour
{
    public static JournalProgressManager Instance { get; private set; }

    [SerializeField] private List<JournalCollectedPage> collectedPages = new List<JournalCollectedPage>();

    public event Action JournalUpdated;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static JournalProgressManager GetOrCreate()
    {
        if (Instance != null)
        {
            return Instance;
        }

        GameObject managerObject = new GameObject("JournalProgressManager");
        return managerObject.AddComponent<JournalProgressManager>();
    }

    public bool TryAddPage(JournalCollectedPage page)
    {
        if (page == null || string.IsNullOrWhiteSpace(page.pageId))
        {
            return false;
        }

        if (HasPage(page.pageId))
        {
            return false;
        }

        collectedPages.Add(page);
        JournalUpdated?.Invoke();
        return true;
    }

    public bool HasPage(string pageId)
    {
        return collectedPages.Exists(page => page.pageId == pageId);
    }

    public List<JournalCollectedPage> GetCollectedPages()
    {
        return new List<JournalCollectedPage>(collectedPages);
    }

    public List<JournalCollectedPage> GetCollectedPages(JournalTab tab)
    {
        return collectedPages
            .Where(page => page.act == tab)
            .OrderBy(page => page.displayOrder)
            .ThenBy(page => page.title)
            .ToList();
    }

    public List<JournalCollectedPage> GetCollectedPagesForLevel(string levelName)
    {
        return collectedPages
            .Where(page => string.Equals(page.levelName, levelName, StringComparison.OrdinalIgnoreCase))
            .OrderBy(page => page.displayOrder)
            .ThenBy(page => page.title)
            .ToList();
    }

    public void SetCollectedPages(List<JournalCollectedPage> pages)
    {
        collectedPages = new List<JournalCollectedPage>();

        if (pages != null)
        {
            HashSet<string> seenIds = new HashSet<string>();
            foreach (JournalCollectedPage page in pages)
            {
                if (page == null || string.IsNullOrWhiteSpace(page.pageId))
                {
                    continue;
                }

                if (!seenIds.Add(page.pageId))
                {
                    Debug.LogWarning($"JournalProgressManager: Duplicate pageId '{page.pageId}' detected during load.");
                    continue;
                }

                collectedPages.Add(page);
            }
        }

        JournalUpdated?.Invoke();
    }
}
