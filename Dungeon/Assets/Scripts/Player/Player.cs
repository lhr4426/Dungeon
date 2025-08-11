using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController controller;
    public PlayerCondition condition;
    public Interaction interaction;
    public Inventory inventory;

    public ItemData itemData;
    public Action addItem;

    void Awake()
    {
        PlayerManager.Instance.InitPlayer(this);

        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
        interaction = GetComponent<Interaction>();
        inventory = GetComponent<Inventory>();
    }

}
