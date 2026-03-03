using UnityEngine;

    public class NPCQuestTrigger : MonoBehaviour
    {
        public string[] questStartDialogue;
        public string[] questReminderDialogue;
        public string[] afterCompletionDialogue;
        
        private NPCDialogueTrigger dialogueTrigger; //reference for npc trigger

        private void Start()
        {
            dialogueTrigger = GetComponent<NPCDialogueTrigger>();
        }

        void Update()
        {
            //Triggers when player presses F 
            if (dialogueTrigger.enabled && Input.GetKeyDown(KeyCode.F))
            {
                if (!QuestManager.Instance.questActive && !QuestManager.Instance.questCompleted)
                { 
                    //Start quest
                    NPCDialogueManager.Instance.StartDialogue(questStartDialogue);
                    QuestManager.Instance.StartQuest();
                }
                
                else if (QuestManager.Instance.questActive)
                {
                    // Checks progress
                    QuestManager.Instance.QuestProgress();
                    NPCDialogueManager.Instance.StartDialogue(questReminderDialogue);
                }
                
                else if (QuestManager.Instance.questCompleted)
                {
                    // Dialogue after quest completion
                    NPCDialogueManager.Instance.StartDialogue(afterCompletionDialogue);
                }
            }
        }
    }
