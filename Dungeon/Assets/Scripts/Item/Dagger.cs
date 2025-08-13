using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Dagger : EquipTool
{
    public float changeSpeed;

    public override void OnEquip()
    {
        PlayerManager.Player.controller.ChangeSpeed(changeSpeed);
    }

    public override void OnUnequip()
    {
        PlayerManager.Player.controller.ResetSpeed();
    }
}
