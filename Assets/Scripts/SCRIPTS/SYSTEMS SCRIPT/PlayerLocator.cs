using UnityEngine;

public static class PlayerLocator
{
    private static Transform cachedPlayer;
    private static float nextLookupTime;
    private const float LookupCooldownSeconds = 0.25f;

    public static Transform GetPlayerTransform()
    {
        if (cachedPlayer == null)
        {
            if (Time.unscaledTime < nextLookupTime)
            {
                return null;
            }

            nextLookupTime = Time.unscaledTime + LookupCooldownSeconds;
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
}
