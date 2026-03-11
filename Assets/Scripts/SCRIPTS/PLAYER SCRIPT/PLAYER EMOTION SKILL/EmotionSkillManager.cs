using System;
using UnityEngine;

// Global Enums used by the Manager, Skills, and Attack script
public enum EmotionSkill { None, Joy, Sadness }
public enum EnemyType { RiceMouse, WaterGhost, FireSentinel }

[RequireComponent(typeof(JoySkill), typeof(SadnessSkill))]
public class EmotionSkillManager : MonoBehaviour
{
    private JoySkill joySkill;
    private SadnessSkill sadnessSkill;
    private Player player;

    [Header("Current State")]
    public EmotionSkill currentSkill = EmotionSkill.None;

    private void Start()
    {
        joySkill = GetComponent<JoySkill>();
        sadnessSkill = GetComponent<SadnessSkill>();
        player = GetComponent<Player>();
        
        if (player != null)
        {
            player.isSkillActive = false;
        }  
    }

    private void Update()
    {
        // Toggle Joy Skill (Z)
        if (Input.GetKeyDown(KeyCode.Z))
        {
            if (currentSkill == EmotionSkill.Joy)
            {
                // Turn OFF Joy
                currentSkill = EmotionSkill.None;
                joySkill.SetJoy(false);
            }
            else
            {
                // Turn ON Joy (and force Sadness off)
                currentSkill = EmotionSkill.Joy;
                joySkill.SetJoy(true);
                sadnessSkill.SetSadness(false);
            }
        }
        
        // Toggle Sadness Skill (X)
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (currentSkill == EmotionSkill.Sadness)
            {
                // Turn OFF Sadness
                currentSkill = EmotionSkill.None;
                sadnessSkill.SetSadness(false);
            }
            else
            {
                // Try to turn ON Sadness. It returns false if locked by scene index.
                bool success = sadnessSkill.SetSadness(true);
                if (success)
                {
                    currentSkill = EmotionSkill.Sadness;
                    joySkill.SetJoy(false); // Force Joy off
                }
            }
        }
    }
}