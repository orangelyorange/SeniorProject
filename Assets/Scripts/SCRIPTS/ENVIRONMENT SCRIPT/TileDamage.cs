using UnityEngine;
using System.Collections.Generic;

public class TileDamage : MonoBehaviour
{
   public HealthSystem playerHealth;

   private void OnCollisionEnter2D(Collision2D collision)
   {
      if (collision.collider.tag == "Player")
      {
         playerHealth.TakeDamage(1);
         StartCoroutine(playerHealth.BlinkRed());
      }
      }
   }
