using UnityEngine;

    public class QuestSystem : MonoBehaviour
    {
        public string[] questStartDialogue;
        public string[] questReminderDialogue;
        public string[] questSuccessDialogue;
        public string[] afterCompletionDialogue;

        public string questItemID = "Rice Stalk";
        public int amountRequired = 5;
        
        private NPCDialogueTrigger dialogueTrigger; //reference for npc trigger
        private PlayerQuestItemInventory playerQuestInventory;
        private void Start()
        {
            dialogueTrigger = GetComponent<NPCDialogueTrigger>();
            if (playerQuestInventory == null) playerQuestInventory = FindFirstObjectByType<PlayerQuestItemInventory>();

        }

        void Update()
        {
            //Triggers when player presses F 
            if (dialogueTrigger.enabled && dialogueTrigger.isPlayerInRange && Input.GetKeyDown(KeyCode.F))
            {
                HandleInteraction();
            }
        }

        void HandleInteraction()
        {
            //if quest has not started
            if (!QuestManager.Instance.questActive && !QuestManager.Instance.questCompleted)
            {
                NPCDialogueManager.Instance.StartDialogue(questStartDialogue);
                QuestManager.Instance.StartQuest();
            }
            
            //quest in progress
            else if (QuestManager.Instance.questActive)
            {
                // check if player has sufficient items
                int currentAmount = playerQuestInventory.GetItemCount(questItemID); //to check if player has collected items
                if (currentAmount >= amountRequired)
                {
					//Remove quest items to deliver to NPC
                    playerQuestInventory.RemoveItem(questItemID, amountRequired);
					
					//To complete the quest
					QuestManager.Instance.CompleteQuest();

					NPCDialogueManager.Instance.StartDialogue(questSuccessDialogue);
                }
                else
                {
                    // Reminder Sequence if Player does not enough items
                    
                    NPCDialogueManager.Instance.StartDialogue(questReminderDialogue);
                }
            }
            else if (QuestManager.Instance.questCompleted)
            {
                NPCDialogueManager.Instance.StartDialogue(afterCompletionDialogue);
            }
        }
    }
