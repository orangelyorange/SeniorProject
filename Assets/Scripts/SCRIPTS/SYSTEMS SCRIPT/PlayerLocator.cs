using UnityEngine;

public static class PlayerLocator
{
    private static Transform cachedPlayer;

    public static Transform GetPlayerTransform()
    {
        if (cachedPlayer == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            cachedPlayer = playerObject != null ? playerObject.transform : null;
        }

        return cachedPlayer;
    }

    public static GameObject GetPlayerGameObject()
    {
        Transform playerTransform = GetPlayerTransform();
        return playerTransform != null ? playerTransform.gameObject : null;
    }

    public static void ClearCache()
    {
        cachedPlayer = null;
    }
}
