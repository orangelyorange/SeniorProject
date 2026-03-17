using UnityEngine;

public class MusicManager : MonoBehaviour
{
    // This creates a static reference to the manager that we can check
    public static MusicManager instance;

    void Awake()
    {
        // Check if an instance of the MusicManager already exists
        if (instance == null)
        {
            // If not, set this as the instance
            instance = this;
            
            // NOTE: If you want the main menu music to keep playing into your 
            // "Level 1 Test Mechanics" scene, uncomment the line below:
            // DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // If another one already exists (e.g., you returned to the main menu), 
            // destroy this duplicate so the music doesn't overlap
            Destroy(gameObject);
        }
    }
}