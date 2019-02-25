using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;

/*
 * This script handles the wind force in the game. This means it is responsible for handling all player input.
 * The wind force is the Wind.velocity variable which other scripts can read.
 * This script additionally handles all cloth flapping as a result of wind.
*/

public class Wind : MonoBehaviour
{
    public bool usingArduinoInput;
    public static Vector3 velocity;
    public Vector3 baseRandomAccel;
    public float clothExternalAccelMultiplier;
    public float clothRandomAccelMultiplier;
    public Vector4 stationaryWaveSpeed;

    SerialPort sp;

    private float inputHorizontal;
    private float inputVertical;

    private Cloth[] arrayOfCloths;

    void Start()
    {

        // Make an array of all Cloth objects that can be blown by wind
        arrayOfCloths = (Cloth[])GameObject.FindObjectsOfType(typeof(Cloth));

        sp = new SerialPort("COM3", 9600);
        sp.Open();
        sp.ReadTimeout = 1;
    }

    void Update()
    {
        if (usingArduinoInput)
        {
            if (sp.IsOpen)
            {
                int direction;
                try
                {
                    direction = sp.ReadByte();
                }
                catch
                {
                    direction = 0;
                }
                switch (direction)
                {
                    case 1:
                        inputHorizontal = 1;
                        break;

                    case 2:
                        inputHorizontal = -1;
                        break;

                    default:
                        inputHorizontal = 0;
                        break;
                }
            }
        }
        else
        {
            inputHorizontal = Input.GetAxis("Horizontal");
        }
        inputVertical = Input.GetAxis("Vertical");

        // Get inputs and assign them to WindVector
        velocity = new Vector3(inputHorizontal, 0.0f, inputVertical);

        // Apply wind force to all Cloth
        foreach (Cloth thisCloth in arrayOfCloths)
        {
            thisCloth.externalAcceleration = (velocity * clothExternalAccelMultiplier);
            thisCloth.randomAcceleration = (baseRandomAccel * velocity.magnitude * clothRandomAccelMultiplier);
        }

        // Apply wind force to the sea water script

        //UnityStandardAssets.Water.Water.waveSpeed = stationaryWaveSpeed /*+ new Vector4(-inputHorizontal * 10, -inputVertical * 10, inputHorizontal * 10, inputVertical * 10)*/ ;

    }
}
