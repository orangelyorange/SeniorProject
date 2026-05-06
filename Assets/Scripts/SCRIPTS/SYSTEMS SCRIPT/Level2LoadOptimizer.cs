using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class Level2LoadOptimizer : MonoBehaviour
{
    [Header("Enemy Activation")]
    [SerializeField] private bool delayEnemyActivation = true;
    [SerializeField] private float enemyActivationDelay = 0.5f;
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private bool reactivateEnemiesAfterDelay = true;

    [Header("Optional Non-Critical Objects")]
    [SerializeField] private GameObject[] deactivateOnLoad;
    [SerializeField] private bool reactivateObjectsAfterDelay = true;

    private readonly List<GameObject> delayedEnemies = new List<GameObject>();
    private readonly List<GameObject> delayedObjects = new List<GameObject>();

    private void Awake()
    {
        if (delayEnemyActivation)
        {
            CacheAndDeactivateByTag(enemyTag, delayedEnemies);
        }

        if (deactivateOnLoad != null)
        {
            foreach (GameObject target in deactivateOnLoad)
            {
                if (target == null || !target.activeSelf) continue;
                target.SetActive(false);
                delayedObjects.Add(target);
            }
        }
    }

    private IEnumerator Start()
    {
        if (enemyActivationDelay > 0f)
        {
            yield return new WaitForSeconds(enemyActivationDelay);
        }

        if (reactivateEnemiesAfterDelay)
        {
            ReactivateList(delayedEnemies);
        }

        if (reactivateObjectsAfterDelay)
        {
            ReactivateList(delayedObjects);
        }
    }

    private static void CacheAndDeactivateByTag(string tag, List<GameObject> cache)
    {
        if (string.IsNullOrWhiteSpace(tag)) return;

        GameObject[] taggedObjects;
        try
        {
            taggedObjects = GameObject.FindGameObjectsWithTag(tag);
        }
        catch (UnityException)
        {
            Debug.LogWarning($"Level2LoadOptimizer: Tag '{tag}' not found. Skipping delayed activation.");
            return;
        }

        foreach (GameObject obj in taggedObjects)
        {
            if (obj == null || !obj.activeSelf) continue;
            obj.SetActive(false);
            cache.Add(obj);
        }
    }

    private static void ReactivateList(List<GameObject> cached)
    {
        foreach (GameObject obj in cached)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        cached.Clear();
    }
}
