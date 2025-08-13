using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Resource : MonoBehaviour
{
    public ItemData itemToGive;
    public int quantityPerHit = 1; // �� �� ĥ������ ������ ������ ��
    public int capacity; // �� �� �� ĥ �� �ִ���

    public void Gather(Vector3 hitPoint, Vector3 hitNormal)
    {
        for (int i = 0; i < quantityPerHit; i++)
        {
            if (capacity <= 0) break;
            capacity -= 1;
            Instantiate(itemToGive.dropPrefab, hitPoint + Vector3.up, Quaternion.LookRotation(hitNormal));
        }

        if (capacity <= 0)
        {
            Destroy(gameObject);
        }
    }

}
