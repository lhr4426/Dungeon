using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public static Player Player;

    public void InitPlayer(Player player)
    {
        Player = player;
    }
}
