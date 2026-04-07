using UnityEngine;
using TMPro;

public class QuestUIManager : MonoBehaviour
{
    public static QuestUIManager Instance;

    [Header("UI Elements")]
    [Tooltip("The parent panel for the quest notification")]
    public GameObject questNotificationPanel;
   
    
    [Tooltip("Text to display the objective (e.g. 'Gather 5 Apples')")]
    public TextMeshProUGUI objectiveText;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // Make sure the panel is hidden when the game starts
        if (questNotificationPanel != null)
        {
            questNotificationPanel.SetActive(false);
        }
    }

    // Call this method from your QuestSystem
    public void DisplayNewQuest(QuestData newQuest)
    {
        if (questNotificationPanel == null)
        {
            Debug.LogWarning("QuestUIManager: Notification Panel is not assigned in the Inspector!");
            return;
        }

   
            
        if (objectiveText != null) 
            objectiveText.text = $"Gather {newQuest.requiredAmount} {newQuest.requiredItemName}(s) for {newQuest.npcName}.";

        // 2. Turn the panel on (it will now stay on indefinitely)
        questNotificationPanel.SetActive(true);
    }

    // Call this whenever you want the UI to disappear
    public void HideQuestNotification()
    {
        if (questNotificationPanel != null)
        {
            questNotificationPanel.SetActive(false);
        }
    }
}