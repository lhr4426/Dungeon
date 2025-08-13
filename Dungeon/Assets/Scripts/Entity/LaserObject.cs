using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserObject : MonoBehaviour
{
    public Transform startObj;
    public Transform endObj;
    public int damage;
    public float damageRate;

    Vector3 startCenter;
    Vector3 endCenter;
    Vector3 dir;
    float distance;
    Ray ray;
    float lastDamageTime;

    private void Start()
    {
        startCenter = GetCenter(startObj);
        endCenter = GetCenter(endObj);
        dir = (endCenter - startCenter).normalized;
        distance = Vector3.Distance(startObj.position, endObj.position);

        ray = new Ray(startCenter, dir);

        Debug.Log($"Start Center : {startCenter}");
        Debug.Log($"End Center : {endCenter}");
        Debug.Log($"Direction : {dir}");
        Debug.Log($"Distance : {distance}");

    }

    void Update()
    {
        CheckRay();
    }

    void CheckRay()
    {
        // Debug.DrawRay(startCenter, dir, Color.red);

        RaycastHit hit;
        if(Physics.Raycast(ray, out hit, distance))
        {
            if(hit.collider.TryGetComponent(out IDamagable damagable))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    if(Time.time - lastDamageTime > damageRate)
                    {
                        lastDamageTime = Time.time;
                        damagable.TakeDamage(damage);
                    }
                }
                else
                {
                    damagable.TakeDamage(damage);
                }
            }
        }
    }

    Vector3 GetCenter(Transform tf)
    {
        return tf.GetComponent<Renderer>().bounds.center;
    }
}
