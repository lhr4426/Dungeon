using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    public Transform dropPosition;

    private Dictionary<ItemData, int> inventory;

    public void AddItem(ItemData item, int quantity)
    {
        if(inventory.ContainsKey(item))
        {
            inventory[item] = Mathf.Min(inventory[item] + quantity, item.maxStackAmount);
        }
        else
        {
            inventory[item] = quantity;
        }
    }

    public void RemoveItem(ItemData item, int quantity)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item] = Mathf.Max(inventory[item] - quantity, 0);
            if (inventory[item] <= 0)
            {
                inventory.Remove(item);
            }
        }
    }
}
