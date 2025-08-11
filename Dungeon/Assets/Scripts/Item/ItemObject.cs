using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour, IInteractable
{
    public ItemData data;

    public string GetInteractPrompt()
    {
        return $"{data.displayName}\n{data.description}";
    }

    public void OnInteract()
    {
        PlayerManager.Player.itemData = data;
        PlayerManager.Player.addItem?.Invoke();
        Destroy(gameObject);
    }
}
