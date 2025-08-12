using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    public float smallTime;
    public Vector3 smallSize;

    public void UseEffect(string effectName)
    {
        Invoke(effectName, 0);
    }

    public void Smaller()
    {
        StartCoroutine(SmallerTime());
    }

    IEnumerator SmallerTime()
    {
        Vector3 originalSize = PlayerManager.Player.transform.localScale;
        PlayerManager.Player.transform.localScale = smallSize;
        yield return new WaitForSeconds(smallTime);
        PlayerManager.Player.transform.localScale = originalSize;

    }
}
