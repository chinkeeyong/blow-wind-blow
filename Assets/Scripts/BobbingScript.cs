using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingScript : MonoBehaviour
{

    private float bobDistance;
    private float bobRate;
    private float turnSpeed;

    private void Start()
    {
        bobDistance = 0.03F;
        bobRate = 3F;
        turnSpeed = Random.Range(50F, 70F);
    }
    
    void Update()
    {
        if (!GamePauser.paused)
        {
            float bobPhase = Mathf.Sin(bobRate * Time.timeSinceLevelLoad);
            transform.Translate(0F, bobPhase * bobDistance, 0F);
            transform.Rotate(0F, turnSpeed * Time.deltaTime, 0F);
        }
    }
}
