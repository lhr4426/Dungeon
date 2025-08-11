using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Settings")]
    public Transform slotParent;
    public GameObject slotPrefab;

    public TextMeshProUGUI selectedItemName;
    public TextMeshProUGUI selectedItemDesc;
    public TextMeshProUGUI selectedItemStatName;
    public TextMeshProUGUI selectedItemStatValue;

    public Button useButton;
    public Button equipButton;
    public Button unequipButton;
    public Button dropButton;

    ItemData selectedItem;
    int selectedIndex;

    int curEquipIndex;
}
