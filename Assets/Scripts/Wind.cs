﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using System.IO.Ports;

/*
 * This script handles the wind force in the game. This means it is responsible for handling all player input.
 * The wind force is the Wind.velocity variable which other scripts can read.
 * This script additionally handles all cloth flapping and wave movement as a result of wind,
 * and keeps track of the prevailing wind and associated music/light changes.
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

    public static int prevailingWind = 0; // 0 = None; 1 = North; 2 = South; 3 = East; 4 = West

    private int lastPrevailingWind = 0;

    public static Text WindText;

    public Text WindTextInEditor; // We need this because unity Editor doesn't expose public static variables

    public Light defaultLight;
    public Light northLight;
    public Light southLight;
    public Light eastLight;
    public Light westLight;

    public Material waterMaterial;
    public Color defaultWater;
    public Color northWater;
    public Color southWater;
    public Color eastWater;
    public Color westWater;

    public Animator northAnimator;
    public Animator southAnimator;
    public Animator eastAnimator;
    public Animator westAnimator;

    public float defaultLightIntensity;
    public float baseLightIntensity;
    public float windLightIntensity;

    // Music and audio

    public AudioMixer musicMixer;
    public AudioSource waveSound;

    void Start()
    {
        // Initialize serial port
        if (usingArduinoInput)
        {
            try
            {
                sp = new SerialPort("COM3", 9600);
                sp.Open();
                sp.ReadTimeout = 1;
            }
            catch
            {
                print("No Arduino input detected");
                usingArduinoInput = false;
            }
        }

        // Make an array of all Cloth objects that can be blown by wind
        arrayOfCloths = (Cloth[])GameObject.FindObjectsOfType(typeof(Cloth));

        // Set initial water wave speed
        UnityStandardAssets.Water.Water.waveSpeed = stationaryWaveSpeed;

        // Set the public static WindText
        WindText = WindTextInEditor;

        // Set the default water color
        waterMaterial.SetColor("_RefrColor", defaultWater);
    }

    private void Update()
    {
        GetInput();
        AdjustMusic();
        AdjustWaveVolume();

        if (!Cutscene.active)
        {

            AdjustWorldLights();

            if (lastPrevailingWind != prevailingWind)
            {
                lastPrevailingWind = prevailingWind;
                AdjustHeadAnimations();
            }
        }
    }

    private void FixedUpdate()
    {
        BlowCloths();
        DisplaceWaterWaves();
    }

    private void GetInput()
    {
        inputHorizontal = 0;
        inputVertical = 0;
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

                    case -2: // All winds blowing
                        inputHorizontal = 0;
                        inputVertical = 0;
                        break;

                    case -1: // Two winds blowing at odds
                        inputHorizontal = 0;
                        inputVertical = 0;
                        break;

                    case 2: // Only East blowing
                        inputHorizontal = -1;
                        inputVertical = 0;
                        break;

                    case 3:
                        inputHorizontal = 1;
                        inputVertical = 0;
                        break;

                    case 5:
                        inputHorizontal = 0;
                        inputVertical = -1;
                        break;

                    case 6:
                        inputHorizontal = -1;
                        inputVertical = -1;
                        break;

                    case 7:
                        inputHorizontal = 0;
                        inputVertical = 1;
                        break;

                    case 14:
                        inputHorizontal = -1;
                        inputVertical = 1;
                        break;

                    case 15:
                        inputHorizontal = 1;
                        inputVertical = -1;
                        break;

                    case 21:
                        inputHorizontal = 1;
                        inputVertical = 1;
                        break;

                    case 30:
                        inputHorizontal = 0;
                        inputVertical = -1;
                        break;

                    case 42:
                        inputHorizontal = 0;
                        inputVertical = 1;
                        break;

                    case 70:
                        inputHorizontal = -1;
                        inputVertical = 0;
                        break;

                    case 105:
                        inputHorizontal = 1;
                        inputVertical = 0;
                        break;

                    default:
                        inputHorizontal = 0;
                        inputVertical = 0;
                        break;
                }
            }
        }
        if (inputHorizontal == 0)
        {
            inputHorizontal = Input.GetAxisRaw("Horizontal");
        }
        if (inputVertical == 0)
        {
            inputVertical = Input.GetAxisRaw("Vertical");
        }

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
                waterMaterial.SetColor("_RefrColor", Color.Lerp(waterMaterial.GetColor("_RefrColor"), northWater, Time.deltaTime));
                break;

            case 2:
                defaultLight.intensity = Mathf.Lerp(defaultLight.intensity, baseLightIntensity, Time.deltaTime);
                northLight.intensity = Mathf.Lerp(northLight.intensity, 0F, Time.deltaTime);
                southLight.intensity = Mathf.Lerp(southLight.intensity, windLightIntensity, Time.deltaTime);
                eastLight.intensity = Mathf.Lerp(eastLight.intensity, 0F, Time.deltaTime);
                westLight.intensity = Mathf.Lerp(westLight.intensity, 0F, Time.deltaTime);
                waterMaterial.SetColor("_RefrColor", Color.Lerp(waterMaterial.GetColor("_RefrColor"), southWater, Time.deltaTime));
                break;

            case 3:
                defaultLight.intensity = Mathf.Lerp(defaultLight.intensity, baseLightIntensity, Time.deltaTime);
                northLight.intensity = Mathf.Lerp(northLight.intensity, 0F, Time.deltaTime);
                southLight.intensity = Mathf.Lerp(southLight.intensity, 0F, Time.deltaTime);
                eastLight.intensity = Mathf.Lerp(eastLight.intensity, windLightIntensity, Time.deltaTime);
                westLight.intensity = Mathf.Lerp(westLight.intensity, 0F, Time.deltaTime);
                waterMaterial.SetColor("_RefrColor", Color.Lerp(waterMaterial.GetColor("_RefrColor"), eastWater, Time.deltaTime));
                break;

            case 4:
                defaultLight.intensity = Mathf.Lerp(defaultLight.intensity, baseLightIntensity, Time.deltaTime);
                northLight.intensity = Mathf.Lerp(northLight.intensity, 0F, Time.deltaTime);
                southLight.intensity = Mathf.Lerp(southLight.intensity, 0F, Time.deltaTime);
                eastLight.intensity = Mathf.Lerp(eastLight.intensity, 0F, Time.deltaTime);
                westLight.intensity = Mathf.Lerp(westLight.intensity, windLightIntensity, Time.deltaTime);
                waterMaterial.SetColor("_RefrColor", Color.Lerp(waterMaterial.GetColor("_RefrColor"), westWater, Time.deltaTime));
                break;

            default:
                defaultLight.intensity = Mathf.Lerp(defaultLight.intensity, defaultLightIntensity, Time.deltaTime);
                northLight.intensity = Mathf.Lerp(northLight.intensity, 0F, Time.deltaTime);
                southLight.intensity = Mathf.Lerp(southLight.intensity, 0F, Time.deltaTime);
                eastLight.intensity = Mathf.Lerp(eastLight.intensity, 0F, Time.deltaTime);
                westLight.intensity = Mathf.Lerp(westLight.intensity, 0F, Time.deltaTime);
                waterMaterial.SetColor("_RefrColor", Color.Lerp(waterMaterial.GetColor("_RefrColor"), defaultWater, Time.deltaTime));
                break;
        }
    }

    private void AdjustHeadAnimations()
    {
        switch (prevailingWind)
        {
            case 1:
                northAnimator.Play("Glowing", 0, 0F);
                southAnimator.Play("Static", 0, 0F);
                eastAnimator.Play("Static", 0, 0F);
                westAnimator.Play("Static", 0, 0F);
                break;

            case 2:
                northAnimator.Play("Static", 0, 0F);
                southAnimator.Play("Glowing", 0, 0F);
                eastAnimator.Play("Static", 0, 0F);
                westAnimator.Play("Static", 0, 0F);
                break;

            case 3:
                northAnimator.Play("Static", 0, 0F);
                southAnimator.Play("Static", 0, 0F);
                eastAnimator.Play("Glowing", 0, 0F);
                westAnimator.Play("Static", 0, 0F);
                break;

            case 4:
                northAnimator.Play("Static", 0, 0F);
                southAnimator.Play("Static", 0, 0F);
                eastAnimator.Play("Static", 0, 0F);
                westAnimator.Play("Glowing", 0, 0F);
                break;

            default:
                northAnimator.Play("Static", 0, 0F);
                southAnimator.Play("Static", 0, 0F);
                eastAnimator.Play("Static", 0, 0F);
                westAnimator.Play("Static", 0, 0F);
                break;
        }
    }

    private void AdjustMusic()
    {
        AudioMixerSnapshot[] snapshots = new AudioMixerSnapshot[1];
        float[] weights = new float[1];
        weights[0] = 1F;
        if (Cutscene.active)
        {
            if (Compass.currentTargetNo == 0)
            {
                snapshots[0] = musicMixer.FindSnapshot("Intro");
            }
            else if (Cutscene.endingReached)
            {
                switch (lastPrevailingWind)
                {
                    case 1:
                        snapshots[0] = musicMixer.FindSnapshot("North Ending");
                        break;

                    case 2:
                        snapshots[0] = musicMixer.FindSnapshot("South Ending");
                        break;

                    case 3:
                        snapshots[0] = musicMixer.FindSnapshot("East Ending");
                        break;

                    case 4:
                        snapshots[0] = musicMixer.FindSnapshot("West Ending");
                        break;

                    default:
                        snapshots[0] = musicMixer.FindSnapshot("Intro");
                        break;
                }
            }
            else
            {
                switch (lastPrevailingWind)
                {
                    case 1:
                        snapshots[0] = musicMixer.FindSnapshot("North Win");
                        break;

                    case 2:
                        snapshots[0] = musicMixer.FindSnapshot("South Win");
                        break;

                    case 3:
                        snapshots[0] = musicMixer.FindSnapshot("East Win");
                        break;

                    case 4:
                        snapshots[0] = musicMixer.FindSnapshot("West Win");
                        break;

                    default:
                        snapshots[0] = musicMixer.FindSnapshot("Intro");
                        break;
                }
            }
        } else
        {
            switch (prevailingWind)
            {
                case 1:
                    snapshots[0] = musicMixer.FindSnapshot("North");
                    break;

                case 2:
                    snapshots[0] = musicMixer.FindSnapshot("South");
                    break;

                case 3:
                    snapshots[0] = musicMixer.FindSnapshot("East");
                    break;

                case 4:
                    snapshots[0] = musicMixer.FindSnapshot("West");
                    break;

                default:
                    snapshots[0] = musicMixer.FindSnapshot("Default");
                    break;
            }
        }
        musicMixer.TransitionToSnapshots(snapshots, weights, 0.5F);
    }

    private void AdjustWaveVolume()
    {
        float newVolume = Mathf.Lerp(waveSound.volume, velocity.magnitude / 2, 0.01F);
        waveSound.volume = newVolume;
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
