using UnityEngine;

    public class QuestSystem : MonoBehaviour
    {
        public string[] questStartDialogue;
        public string[] questReminderDialogue;
        public string[] questSuccessDialogue;
        public string[] afterCompletionDialogue;

        public bool isTalking = false;
        
        private NPCDialogueTrigger dialogueTrigger; //reference for npc trigger
        
        private void Start()
        {
            dialogueTrigger = GetComponent<NPCDialogueTrigger>();
            
        }

        void Update()
        {
            //Triggers when player presses F and is inside the collider
            if (Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("1. F was pressed");

                if (dialogueTrigger == null)
                {
                    Debug.LogError("2. Dialogue Trigger was null. Make sure Quest System and Dialogue Trigger are on the same Game Object");
                    return; //stops the code
                }
                
                Debug.Log("3. Player is in collider range, Checking isTalking state.");

                if (!isTalking)
                {
                    Debug.Log("4. Starting new conversation");
                    HandleInteraction();
                    isTalking = true;
                }
                else
                {
                    Debug.Log("4. Already talking, trying to show the next line");
                    NPCDialogueManager.Instance.DisplayNextLine();
                }
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
                //will return true if items are delivered, false if the player does not have enough items
                bool isDelivered = QuestManager.Instance.QuestProgress();
                if (isDelivered)
                {
                    NPCDialogueManager.Instance.StartDialogue(questSuccessDialogue);
                }
                else
                {
                    // Reminder Sequence if Player does not enough items
                    NPCDialogueManager.Instance.StartDialogue(questReminderDialogue);
                }
            }
            //if the quest is completed
            else if (QuestManager.Instance.questCompleted)
            {
                NPCDialogueManager.Instance.StartDialogue(questSuccessDialogue);
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag("Player"))
            {
                isTalking = false;
            }
        }
    }
