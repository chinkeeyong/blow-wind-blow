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
    public float shakeLerpAmount;

    private float shakeAmount;

    void Update()
    {
        if (!GamePauser.paused)
        {
            transform.position = target.transform.position + new Vector3(OffsetX, OffsetY, OffsetZ);
            transform.eulerAngles = new Vector3(LookAngle + Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount), Random.Range(-shakeAmount, shakeAmount));
            shakeAmount = Mathf.Lerp(shakeAmount, 0, Time.deltaTime * shakeLerpAmount);
        }

    }

    public void shake(float magnitude)
    {
        shakeAmount += magnitude;
    }
}
