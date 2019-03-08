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
    public float wave1DispMagnitude;
    public float wave2DispMagnitude;
    public float waveDisplacementZeroOffset;
    public float waveLerpAmount;

    SerialPort sp;

    private float inputHorizontal;
    private float inputVertical;

    private Cloth[] arrayOfCloths;

    void Start()
    {

        // Make an array of all Cloth objects that can be blown by wind
        arrayOfCloths = (Cloth[])GameObject.FindObjectsOfType(typeof(Cloth));

        if (usingArduinoInput)
        {
            sp = new SerialPort("COM3", 9600);
            sp.Open();
            sp.ReadTimeout = 1;
        }

        UnityStandardAssets.Water.Water.waveSpeed = stationaryWaveSpeed;
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

        float xDistanceFromStationaryWaveSpeed = Mathf.Abs(UnityStandardAssets.Water.Water.waveSpeed.x - stationaryWaveSpeed.x) + waveDisplacementZeroOffset;
        float yDistanceFromStationaryWaveSpeed = Mathf.Abs(UnityStandardAssets.Water.Water.waveSpeed.y - stationaryWaveSpeed.y) + waveDisplacementZeroOffset;
        float Wave1Displacement = -wave1DispMagnitude / xDistanceFromStationaryWaveSpeed;
        float Wave2Displacement = -wave2DispMagnitude / yDistanceFromStationaryWaveSpeed;
        Vector4 targetWaveSpeed = UnityStandardAssets.Water.Water.waveSpeed + 
            new Vector4(inputHorizontal * Wave1Displacement, inputVertical * Wave1Displacement, inputHorizontal * Wave2Displacement, inputVertical * Wave2Displacement);
        UnityStandardAssets.Water.Water.waveSpeed = Vector4.Lerp(UnityStandardAssets.Water.Water.waveSpeed, targetWaveSpeed, waveLerpAmount * Time.deltaTime);
        print(UnityStandardAssets.Water.Water.waveSpeed.x + ", " + UnityStandardAssets.Water.Water.waveSpeed.y);

    }
}
