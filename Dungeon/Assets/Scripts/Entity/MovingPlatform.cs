using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public Transform platformPos;
    public Transform startPos;
    public Transform endPos;
    public bool goToEnd;
    public float speed;
    public float threshold;

    // Start is called before the first frame update
    void Start()
    {
        goToEnd = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(goToEnd)
        {
            platformPos.position = Vector3.MoveTowards(platformPos.position, endPos.position, Time.deltaTime * speed);
            if (Approximatlely(platformPos, endPos)) goToEnd = false;
        }
        else
        {
            platformPos.position = Vector3.MoveTowards(platformPos.position, startPos.position, Time.deltaTime * speed);
            if (Approximatlely(platformPos, startPos)) goToEnd = true;
        }
    }

    bool Approximatlely(Transform trans, Transform destination)
    {
        if (Mathf.Abs(trans.position.x - destination.position.x) > threshold) return false;
        if (Mathf.Abs(trans.position.y - destination.position.y) > threshold) return false;
        if (Mathf.Abs(trans.position.z - destination.position.z) > threshold) return false;

        return true;
    }
}
