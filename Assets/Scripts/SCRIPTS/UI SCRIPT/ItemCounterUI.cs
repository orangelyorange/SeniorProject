using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ItemCounterUI : MonoBehaviour
{
    public static ItemCounterUI Instance;

    [System.Serializable]
    public class ItemUI
    {
        public string itemName;
        public TMP_Text counterText;
        [HideInInspector] public int count;
    }

    [Header("Registered UI Counters")]
    public List<ItemUI> items = new List<ItemUI>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void AddToCounter(string itemName, int amount)
    {
        foreach (ItemUI item in items)
        {
            if (item.itemName == itemName)
            {
                item.count += amount;
                item.counterText.text = item.count.ToString();
                return;
            }
        }

        Debug.LogWarning($"[ItemCounterUI] No UI counter found for item name: {itemName}");
    }
}