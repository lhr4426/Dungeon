using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public PlayerController controller;
    public PlayerCondition condition;
    public Interaction interaction;
    public PlayerEffect effect;
    public Equipment equipment;

    public ItemData itemData;
    public Action addItem;

    public Transform dropPosition;

    void Awake()
    {
        PlayerManager.Instance.InitPlayer(this);

        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
        interaction = GetComponent<Interaction>();
        effect = GetComponent<PlayerEffect>();
        equipment =GetComponent<Equipment>();
    }

}
