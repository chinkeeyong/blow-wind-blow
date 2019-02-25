using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{

    public GameObject target;
    public float OffsetX;
    public float OffsetY;
    public float OffsetZ;
    public float LookAngle;

    void Update()
    {

        transform.position = target.transform.position + new Vector3(OffsetX, OffsetY, OffsetZ);
        transform.eulerAngles = new Vector3(LookAngle, 0, 0);

    }
}
