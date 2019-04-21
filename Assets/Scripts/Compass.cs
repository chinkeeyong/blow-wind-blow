using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{

    public GameObject player;
    public GameObject[] targetsInEditor;
    public GameObject mainCamera;
    public Graphic compassGraphic;
    public Text distanceText;
    public float fadeSpeed;
    public float fadeOutDistance;
    public float metersPerUnityUnit;

    public static GameObject[] targets;
    public static int currentTargetNo;

    private void Start()
    {
        targets = targetsInEditor;
        currentTargetNo = 0;
    }

    void Update()
    {
        if (currentTargetNo < 3)
        {
            if (!GamePauser.paused)
            {
                // Move compass to player location
                transform.position = player.transform.position;

                // Update distance to target
                int distance = Mathf.RoundToInt(Vector3.Distance(transform.position, targets[currentTargetNo].transform.position) * metersPerUnityUnit);

                // Fade out if we are closer than minimum distance
                if (distance < fadeOutDistance)
                {
                    compassGraphic.color = Color.Lerp(compassGraphic.color, new Color(1F, 1F, 1F, 0F), Time.deltaTime * fadeSpeed);
                    distanceText.color = Color.Lerp(compassGraphic.color, new Color(1F, 1F, 1F, 0F), Time.deltaTime * fadeSpeed);
                }
                else
                {
                    compassGraphic.color = Color.Lerp(compassGraphic.color, Color.white, Time.deltaTime);
                    distanceText.color = Color.Lerp(compassGraphic.color, Color.white, Time.deltaTime);
                }

                // Print distance
                if (distance < 1000)
                {
                    distanceText.text = distance + "m";
                }
                else
                {
                    distance /= 100;
                    int firstSignificantFigure = distance % 10;
                    distance /= 10;
                    distanceText.text = distance + "." + firstSignificantFigure + "km";
                }

                // Rotate compass
                transform.LookAt(targets[currentTargetNo].transform);
                transform.Rotate(90F, 0F, 0F);
                distanceText.transform.LookAt(mainCamera.transform, mainCamera.transform.up);
                distanceText.transform.Rotate(0F, 180F, 0F);
            }
        }
    }
}
