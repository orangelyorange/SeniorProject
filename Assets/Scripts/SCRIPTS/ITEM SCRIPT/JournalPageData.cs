using UnityEngine;

[CreateAssetMenu(menuName = "Journal/Page Data", fileName = "JournalPageData")]
public class JournalPageData : ScriptableObject
{
    [Header("Identity")]
    [SerializeField] private string id;

    [Header("Content")]
    [SerializeField] private string pageTitle;
    [TextArea(3, 10)]
    [SerializeField] private string pageBody;

    [Header("Organization")]
    [SerializeField] private JournalTab act = JournalTab.Act1;
    [SerializeField] private string levelName;
    [SerializeField] private int displayOrder;

    public string Id => id;
    public string PageTitle => pageTitle;
    public string PageBody => pageBody;
    public JournalTab Act => act;
    public string LevelName => levelName;
    public int DisplayOrder => displayOrder;

    public JournalCollectedPage ToCollectedPage(string fallbackLevelName)
    {
        return new JournalCollectedPage
        {
            pageId = string.IsNullOrWhiteSpace(id) ? name : id,
            title = pageTitle,
            body = pageBody,
            act = act,
            levelName = string.IsNullOrWhiteSpace(levelName) ? fallbackLevelName : levelName,
            displayOrder = displayOrder
        };
    }
}
