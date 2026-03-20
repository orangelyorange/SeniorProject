using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [Header("Tutorial Settings")] [TextArea(2, 4)]
    public string tutorialMessage; //input the message you want to show in the inspector
    
    // this trtiggers when the player enters the trigger area
    private void OnTriggerEnter2D(Collider2D other)
    {
        //checks if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            //tells the manager to show the message
            TutorialUIManager.instance.ShowMessage(tutorialMessage);
        }
    }
    
    //this triggers when something leaves the collider
    private void OnTriggerExit2D(Collider2D other)
    {
        //checks if the colliding object is the player
        if (other.CompareTag("Player"))
        {
            //tells the manager to hide the message
            TutorialUIManager.instance.HideMessage();
        }
    }
}
