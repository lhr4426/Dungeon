using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Settings")]
    public ItemSlot[] slots;
    public GameObject inventoryWindow;
    public Transform slotParent;
    public GameObject slotPrefab;

    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDesc;
    public TextMeshProUGUI selectedItemStatName;
    public TextMeshProUGUI selectedItemStatValue;
    public GameObject useButton;
    public GameObject equipButton;
    public GameObject unequipButton;
    public GameObject dropButton;

    ItemSlot selectedItem;
    int selectedIndex;

    int curEquipIndex;

    private PlayerController controller;
    private PlayerCondition condition;

    private event Action itemAction;

    private void Start()
    {
        controller = PlayerManager.Player.controller;
        condition = PlayerManager.Player.condition;
        

        controller.inventory += Toggle;
        PlayerManager.Player.addItem += AddItem;
        slots = new ItemSlot[slotParent.childCount];
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i] = slotParent.GetChild(i).GetComponent<ItemSlot>();
            slots[i].index = i;
            slots[i].inventory = this;
           
        }
        inventoryWindow.SetActive(false);
        ClearSelectedItemWindow();
    }

    void ClearSelectedItemWindow()
    {
        selectedItemName.text = string.Empty;
        selectedItemDesc.text = string.Empty;
        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        useButton.gameObject.SetActive(false);
        equipButton.gameObject.SetActive(false);
        unequipButton.gameObject.SetActive(false);
        dropButton.gameObject.SetActive(false);
    }

    public bool IsOpen()
    {
        return inventoryWindow.activeInHierarchy;
    }

    public void Toggle()
    {
        inventoryWindow.SetActive(!IsOpen());            
    }

    public void AddItem()
    {
        ItemData data = PlayerManager.Player.itemData;

        if (data.canStack)
        {
            ItemSlot slot = GetItemStack(data);
            if (slot != null)
            {
                slot.quantity++;
                UpdateUI();
                PlayerManager.Player.itemData = null;
                return;
            }
        }

        ItemSlot emptySlot = GetEmptySlot();
        if (emptySlot != null)
        {
            emptySlot.item = data;
            emptySlot.quantity = 1;
            UpdateUI();
            PlayerManager.Player.itemData = null;
            return;
        }

        ThrowItem(data);
        PlayerManager.Player.itemData = null;
    }

    public void UpdateUI()
    {
        foreach(var slot in slots)
        {
            if (slot.item != null) slot.Set();
            else slot.Clear();
        }
    }

    ItemSlot GetItemStack(ItemData data)
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == data && slots[i].quantity < data.maxStackAmount)
            {
                return slots[i];
            }
        }
        return null;
    }

    ItemSlot GetEmptySlot()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item == null) return slots[i];
        }

        return null;
    }

    public void ThrowItem(ItemData data)
    {
        Instantiate(data.dropPrefab, PlayerManager.Player.dropPosition.position, Quaternion.Euler(Vector3.one * UnityEngine.Random.value * 360));
    }

    public void SelectItem(int index)
    {
        if (slots[index].item == null) return;

        selectedItem = slots[index];
        selectedIndex = index;

        selectedItemName.text = selectedItem.item.displayName;
        selectedItemDesc.text = selectedItem.item.description;

        selectedItemStatName.text = string.Empty;
        selectedItemStatValue.text = string.Empty;

        for(int i = 0; i < selectedItem.item.consumables.Length; i++)
        {
            selectedItemStatName.text += selectedItem.item.consumables[i].type.ToString() + "\n";
            selectedItemStatValue.text += selectedItem.item.consumables[i].value.ToString() + "\n";
        }

        useButton.SetActive(selectedItem.item.type == ItemType.Consumable);
        equipButton.SetActive(selectedItem.item.type == ItemType.Equipable && !selectedItem.equipped);
        unequipButton.SetActive(selectedItem.item.type == ItemType.Equipable && selectedItem.equipped);
        dropButton.SetActive(true);
    }

    public void OnUseButton()
    {
        if(selectedItem.item.type == ItemType.Consumable)
        {
            for(int i = 0; i < selectedItem.item.consumables.Length; i++)
            {
                switch (selectedItem.item.consumables[i].type)
                {
                    case ConsumableType.Health:
                        condition.Heal(selectedItem.item.consumables[i].value); break;
                    case ConsumableType.Hunger:
                        condition.Eat(selectedItem.item.consumables[i].value); break;
                    case ConsumableType.Stamina:
                        condition.Rest(selectedItem.item.consumables[i].value); break;
                
                }

            }
            
            if(selectedItem.item.actionName != String.Empty)
            {
                PlayerManager.Player.effect.UseEffect(selectedItem.item.actionName);           
            }

            RemoveSelectedItem();
        }
    }

    public void OnDropButton()
    {
        ThrowItem(selectedItem.item);
        if(selectedItem.equipped)
        {
            PlayerManager.Player.equipment.Unequip();
        }
        RemoveSelectedItem();
    }

    void RemoveSelectedItem()
    {
        selectedItem.quantity--;
        if(selectedItem.quantity <=0)
        {
            if (slots[selectedIndex].equipped)
            {
                Unequip(selectedIndex);
            }
            selectedItem.item = null;
            ClearSelectedItemWindow();
        }
        UpdateUI();
    }

    public bool HasItem(ItemData item, int quantity)
    {
        return false;
    }

    public void OnEquipButton()
    {
        if (slots[curEquipIndex].equipped)
        {
            Unequip(curEquipIndex);
        }

        slots[selectedIndex].equipped = true;
        curEquipIndex = selectedIndex;
        PlayerManager.Player.equipment.EquipNew(selectedItem.item);
        UpdateUI();

        SelectItem(selectedIndex);
    }

    void Unequip(int index)
    {
        slots[index].equipped = false;
        PlayerManager.Player.equipment.Unequip();
        UpdateUI();

        if(selectedIndex == index)
        {
            SelectItem(selectedIndex);
        }
    }

    public void OnUnequipButton()
    {
        Unequip(selectedIndex);
    }
}
