using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This script handles the wind force in the game. This means it is responsible for handling all player input.
 * The wind force is the Wind.velocity variable which other scripts can read.
 * This script additionally handles all cloth flapping as a result of wind.
*/

public class Wind : MonoBehaviour
{

    public static Vector3 velocity;
    public Vector3 baseRandomAccel;
    public float clothExternalAccelMultiplier;
    public float clothRandomAccelMultiplier;
    public Vector4 stationaryWaveSpeed;

    private Cloth[] arrayOfCloths;
    private List<Cloth> cloths;

    void Start()
    {

        // Make an array of all Cloth objects that can be blown by wind
        arrayOfCloths = (Cloth[])GameObject.FindObjectsOfType(typeof(Cloth));
    }

    void Update()
    {
        
        // Get inputs and assign them to WindVector
        float inputHorizontal = Input.GetAxis("Horizontal");
        float inputVertical = Input.GetAxis("Vertical");
        velocity = new Vector3(inputHorizontal, 0.0f, inputVertical);

        // Apply wind force to all Cloth
        foreach (Cloth thisCloth in arrayOfCloths)
        {
            thisCloth.externalAcceleration = (velocity * clothExternalAccelMultiplier);
            thisCloth.randomAcceleration = (baseRandomAccel * velocity.magnitude * clothRandomAccelMultiplier);
        }

        // Apply wind force to the sea water script

        UnityStandardAssets.Water.Water.waveSpeed = stationaryWaveSpeed /*+ new Vector4(-inputHorizontal * 10, -inputVertical * 10, inputHorizontal * 10, inputVertical * 10)*/ ;

    }
}
