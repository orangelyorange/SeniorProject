using UnityEngine;

public class EnemyAOESpawn : MonoBehaviour
{
    [Header("Attack settings")] 
    public GameObject wavePrefab; //Wave Prefab
    public float attackCooldown = 3f; //time between attacks

    [Header("Spawn settings")] 
    public float undergroundYPosition = -6f;
    public string targetTag = "Player";

    private float nextAttackTime = 0f;

    public void Update()
    {
        //Timer to spawn the wave automatically
        if (Time.time >= nextAttackTime)
        {
            SpawnWave();
            nextAttackTime = Time.time + attackCooldown;
        }
    }

    public void SpawnWave()
    {
        //Find the player position
        GameObject player = GameObject.FindGameObjectWithTag(targetTag);
        
        if (player != null && wavePrefab != null)
        {
            //creates new position for the wave to spawn directly under the player
            Vector2 spawnPosition = new Vector2(player.transform.position.x, undergroundYPosition);
            //spawns the wave at the new position
            Instantiate(wavePrefab, spawnPosition, Quaternion.identity);
            Debug.Log("Wave spawned!");
        }
    }
}
