using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    PlayerController controller;

    // Start is called before the first frame update
    void Start()
    {
        PlayerManager.Instance.InitPlayer(this);

        controller = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
