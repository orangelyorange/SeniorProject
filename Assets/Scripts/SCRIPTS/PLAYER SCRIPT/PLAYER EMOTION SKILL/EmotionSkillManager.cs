using System;
using UnityEngine;

[RequireComponent(typeof(JoySkill), typeof(SadnessSkill))] //automatically adds the required components to the GameObject
public class NewMonoBehaviourScript : MonoBehaviour
{
  private JoySkill joySkill;
  private SadnessSkill sadnessSkill;
  private Player player;

  private void Start()
  {
    //Grab referenmces to the JoySkill and SadnessSkill components
    joySkill = GetComponent<JoySkill>();
    sadnessSkill = GetComponent<SadnessSkill>();
    player = GetComponent<Player>();
    
    //ensures skills are off at start
    if (player != null)
    {
        player.isSkillActive = false;
    }  
  }

  private void Update()
  {
      //Toggles Joy Skill (Z)
      if (Input.GetKeyDown(KeyCode.Z))
      {
          if (joySkill != null)
          {
              joySkill.ToggleJoy();
          }
      }
      
      //Toggles Sadness Skill (x)
      if (Input.GetKeyDown(KeyCode.X))
      {
          if (sadnessSkill != null)
          {
              sadnessSkill.ToggleSadness();
          }
      }
  }
}
