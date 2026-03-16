using System;
using UnityEngine;

// Global Enums used by the Manager, Skills, and Attack script
public enum EmotionSkill { None, Joy, Sadness, Rage }
public enum EnemyType { RiceMouse, WaterGhost, FireSentinel }

[RequireComponent(typeof(JoySkill), typeof(SadnessSkill), typeof(RageSkill))]
public class EmotionSkillManager : MonoBehaviour
{
    private JoySkill joySkill;
    private SadnessSkill sadnessSkill;
    private RageSkill rageSkill;
    private Player player;

    [Header("Current State")] public EmotionSkill currentSkill = EmotionSkill.None;

    private void Start()
    {
        joySkill = GetComponent<JoySkill>();
        sadnessSkill = GetComponent<SadnessSkill>();
        rageSkill = GetComponent<RageSkill>();
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

                if (player != null) player.isSkillActive = false;
            }
            else
            {
                // Turn ON Joy (and force Sadness off)
                currentSkill = EmotionSkill.Joy;
                joySkill.SetJoy(true);
                sadnessSkill.SetSadness(false);
                rageSkill.SetRage(false);

                if (player != null) player.isSkillActive = true;
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
                player.isSkillActive = false;
            }
            else
            {
                // Try to turn ON Sadness. It returns false if locked by scene index.
                bool success = sadnessSkill.SetSadness(true);
                if (success)
                {
                    currentSkill = EmotionSkill.Sadness;
                    joySkill.SetJoy(false); // Force Joy off
                    rageSkill.SetRage(false);
                    player.isSkillActive = true;
                }
            }
        }

        //Toggle Rage Skill (C)
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (currentSkill == EmotionSkill.Rage)
            {
                //turn off rage
                currentSkill = EmotionSkill.None;
                rageSkill.SetRage(false);
                Debug.Log("Rage Deactivated");

                if (player != null) player.isSkillActive = false;
            }

            else
            {
                //turn on rage (and force joy and sadness off)
                currentSkill = EmotionSkill.Rage;
                rageSkill.SetRage(true);
                joySkill.SetJoy(false);
                sadnessSkill.SetSadness(false);
                Debug.Log("Rage Activated");

                if (player != null) player.isSkillActive = true;
            }
        }
    }
}