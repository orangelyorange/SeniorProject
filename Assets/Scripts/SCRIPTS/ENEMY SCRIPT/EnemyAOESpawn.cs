using UnityEngine;

public class EnemyAOESpawn : MonoBehaviour
{
    [Header("Attack settings")] 
    public GameObject wavePrefab; //Wave Prefab
    public Transform spawnPoint; //wave spawn point
    public float attackCooldown = 3f; //time between attacks

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
        if (wavePrefab != null && spawnPoint != null)
        {
            //Creates the wave on the assigned spawn point
            Instantiate(wavePrefab, spawnPoint.position, spawnPoint.rotation);
            Debug.Log("Spawning wave");
        }
        else
        {
            Debug.Log("No wave, missing prefab or spawn point");
        }
    }
}
