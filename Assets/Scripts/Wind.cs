using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;

/*
 * This script handles the wind force in the game. This means it is responsible for handling all player input.
 * The wind force is the Wind.velocity variable which other scripts can read.
 * This script additionally handles all cloth flapping and wave movement as a result of wind,
 * and keeps track of the prevailing wind.
*/

public class Wind : MonoBehaviour
{
    // Input and Arduino integration

    public bool usingArduinoInput;

    private SerialPort sp;
    private float inputHorizontal;
    private float inputVertical;

    // Movement

    public static Vector3 velocity;

    // Wind flapping

    public Vector3 baseRandomAccel;
    public float clothExternalAccelMultiplier;
    public float clothRandomAccelMultiplier;

    private Cloth[] arrayOfCloths;

    // Waves

    public Vector4 stationaryWaveSpeed;
    public float wave1DispMagnitude;
    public float wave2DispMagnitude;
    public float waveDisplacementZeroOffset;
    public float waveLerpAmount;

    // Prevailing wind

    public static Text WindText;
    public Light defaultLight;
    public Light northLight;
    public Light southLight;
    public Light eastLight;
    public Light westLight;
    public float defaultLightIntensity;
    public float baseLightIntensity;
    public float windLightIntensity;

    public Text WindTextInEditor; // We need this because unity Editor doesn't expose public static variables

    public static int prevailingWind = 0; // 0 = None; 1 = North; 2 = South; 3 = East; 4 = West

    void Start()
    {
        // Initialize serial port
        if (usingArduinoInput)
        {
            sp = new SerialPort("COM3", 9600);
            sp.Open();
            sp.ReadTimeout = 1;
        }

        // Make an array of all Cloth objects that can be blown by wind
        arrayOfCloths = (Cloth[])GameObject.FindObjectsOfType(typeof(Cloth));

        // Set initial water wave speed
        UnityStandardAssets.Water.Water.waveSpeed = stationaryWaveSpeed;

        // Set the public static WindText
        WindText = WindTextInEditor;
    }

    private void Update()
    {
        GetInput();

        if (!GamePauser.paused)
        {
            AdjustWorldLights();
        }
    }

    private void FixedUpdate()
    {
        BlowCloths();
        DisplaceWaterWaves();
    }

    private void GetInput()
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
            inputHorizontal = Input.GetAxisRaw("Horizontal");
        }
        inputVertical = Input.GetAxisRaw("Vertical");

        velocity = new Vector3(inputHorizontal, 0.0f, inputVertical);
    }

    private void BlowCloths()
    {
        foreach (Cloth thisCloth in arrayOfCloths)
        {
            thisCloth.externalAcceleration = (velocity * clothExternalAccelMultiplier);
            Vector3 targetRandomAcceleration = baseRandomAccel * velocity.magnitude * clothRandomAccelMultiplier;
            thisCloth.randomAcceleration = Vector3.Lerp(thisCloth.randomAcceleration, targetRandomAcceleration, 0.1F);
        }
    }

    private void DisplaceWaterWaves()
    {
        // We modify the wave speed at runtime to create the illusion that waves are being blown by the wind.

        // How the Water material waves work: There are 2 layers of waves which move based on "wave speed" and elapsed time. The "wave speed" is not actual speed, 
        // the waves do not keep track of their current position and instead calculate their position every frame based on wave speed and time elapsed.

        // This means that we can't translate wind speed directly to "wave speed" because that causes waves to "jump" suddenly to the new position
        // and jump back when the speed changes back (because their code was not initially designed to change at runtime).

        // Instead we are treating the wave speed as wave position and adding/subtracting constantly depending on the wind velocity.
        // This is a hack that makes it appear as though the waves are moving the way we want them to.
        // We add/subtract a decreasing amount the further we are from 0 so that we dont get increasingly high divergence and perceived wave speed
        // We also have to add a zero offset so we dont get a +- infinity asymptote at 0

        // This doesnt correspond perfectly to the actual wind speed but its less of a pain than extrapolating from the original wave speed formula.

        float xDistanceFromStationaryWaveSpeed = Mathf.Abs(UnityStandardAssets.Water.Water.waveSpeed.x - stationaryWaveSpeed.x) + waveDisplacementZeroOffset;
        float yDistanceFromStationaryWaveSpeed = Mathf.Abs(UnityStandardAssets.Water.Water.waveSpeed.y - stationaryWaveSpeed.y) + waveDisplacementZeroOffset;
        float Wave1Displacement = -wave1DispMagnitude / xDistanceFromStationaryWaveSpeed;
        float Wave2Displacement = -wave2DispMagnitude / yDistanceFromStationaryWaveSpeed;
        Vector4 targetWaveSpeed = UnityStandardAssets.Water.Water.waveSpeed +
            new Vector4(inputHorizontal * Wave1Displacement, inputVertical * Wave1Displacement, inputHorizontal * Wave2Displacement, inputVertical * Wave2Displacement);
        UnityStandardAssets.Water.Water.waveSpeed = Vector4.Lerp(UnityStandardAssets.Water.Water.waveSpeed, targetWaveSpeed, waveLerpAmount * Time.deltaTime);
        //print(UnityStandardAssets.Water.Water.waveSpeed.x + ", " + UnityStandardAssets.Water.Water.waveSpeed.y);
    }

    private void AdjustWorldLights()
    {
        switch (prevailingWind)
        {
            case 1:
                defaultLight.intensity = Mathf.Lerp(defaultLight.intensity, baseLightIntensity, Time.deltaTime);
                northLight.intensity = Mathf.Lerp(northLight.intensity, windLightIntensity, Time.deltaTime);
                southLight.intensity = Mathf.Lerp(southLight.intensity, 0F, Time.deltaTime);
                eastLight.intensity = Mathf.Lerp(eastLight.intensity, 0F, Time.deltaTime);
                westLight.intensity = Mathf.Lerp(westLight.intensity, 0F, Time.deltaTime);
                break;

            case 2:
                defaultLight.intensity = Mathf.Lerp(defaultLight.intensity, baseLightIntensity, Time.deltaTime);
                northLight.intensity = Mathf.Lerp(northLight.intensity, 0F, Time.deltaTime);
                southLight.intensity = Mathf.Lerp(southLight.intensity, windLightIntensity, Time.deltaTime);
                eastLight.intensity = Mathf.Lerp(eastLight.intensity, 0F, Time.deltaTime);
                westLight.intensity = Mathf.Lerp(westLight.intensity, 0F, Time.deltaTime);
                break;

            case 3:
                defaultLight.intensity = Mathf.Lerp(defaultLight.intensity, baseLightIntensity, Time.deltaTime);
                northLight.intensity = Mathf.Lerp(northLight.intensity, 0F, Time.deltaTime);
                southLight.intensity = Mathf.Lerp(southLight.intensity, 0F, Time.deltaTime);
                eastLight.intensity = Mathf.Lerp(eastLight.intensity, windLightIntensity, Time.deltaTime);
                westLight.intensity = Mathf.Lerp(westLight.intensity, 0F, Time.deltaTime);
                break;

            case 4:
                defaultLight.intensity = Mathf.Lerp(defaultLight.intensity, baseLightIntensity, Time.deltaTime);
                northLight.intensity = Mathf.Lerp(northLight.intensity, 0F, Time.deltaTime);
                southLight.intensity = Mathf.Lerp(southLight.intensity, 0F, Time.deltaTime);
                eastLight.intensity = Mathf.Lerp(eastLight.intensity, 0F, Time.deltaTime);
                westLight.intensity = Mathf.Lerp(westLight.intensity, windLightIntensity, Time.deltaTime);
                break;

            default:
                defaultLight.intensity = Mathf.Lerp(defaultLight.intensity, defaultLightIntensity, Time.deltaTime);
                northLight.intensity = Mathf.Lerp(northLight.intensity, 0F, Time.deltaTime);
                southLight.intensity = Mathf.Lerp(southLight.intensity, 0F, Time.deltaTime);
                eastLight.intensity = Mathf.Lerp(eastLight.intensity, 0F, Time.deltaTime);
                westLight.intensity = Mathf.Lerp(westLight.intensity, 0F, Time.deltaTime);
                break;
        }
    }

    public static void SetPrevailingWind(int wind)
    {
        prevailingWind = wind;
        switch (prevailingWind)
        {
            case 1:
                WindText.text = "Wind: North";
                break;

            case 2:
                WindText.text = "Wind: South";
                break;

            case 3:
                WindText.text = "Wind: East";
                break;

            case 4:
                WindText.text = "Wind: West";
                break;

            default:
                WindText.text = "Wind: None";
                break;
        }
    }
}
