using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public GameObject target;
    public float offsetZ = -10f;
    void Update()
    {

        if (target == null) return;

        Vector3 pos = target.transform.position;

        pos.x = Mathf.Clamp(pos.x, -2f, 2f);
        pos.y = Mathf.Clamp(pos.y, -1.8f, 10f);
        pos.z = offsetZ;

        transform.position = pos;
    }
}