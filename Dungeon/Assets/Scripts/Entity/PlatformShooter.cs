using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformShooter : MonoBehaviour
{
    public float shootPower;
    public void Shoot()
    {
        Rigidbody rb = PlayerManager.Player.controller.rb;
        Debug.Log(rb.velocity);
        rb.AddForce(rb.velocity * shootPower + Vector3.up * shootPower, ForceMode.Impulse);
    }
}
