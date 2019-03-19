using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cutscene : MonoBehaviour
{

    public float fadeDuration;
    public float timeUntilDismissable;

    public static float staticFadeDuration;
    public static float timePaused;
    public static Image img;
    public static bool active;
    
    void Start()
    {
        img = gameObject.GetComponent(typeof(Image)) as Image;
        staticFadeDuration = fadeDuration;
        active = false;

        img.canvasRenderer.SetAlpha(0F);
        img.color = new Color(1F, 1F, 1F, 1F);
    }
    
    void Update()
    {
        if (active)
        {
            if (Time.realtimeSinceStartup > timePaused + timeUntilDismissable)
            {
                if (Wind.velocity.magnitude > 0F)
                {
                    Hide();
                }
            }
        } else
        {
        }
    }

    public static void Show(/*Sprite cutsceneToShow*/)
    {
        GamePauser.Pause();
        //img.sprite = cutsceneToShow;
        img.SetNativeSize();
        timePaused = Time.realtimeSinceStartup;
        active = true;
        img.CrossFadeAlpha(1F, staticFadeDuration, true);
    }

    public static void Hide()
    {
        GamePauser.Resume();
        active = false;
        img.CrossFadeAlpha(0F, staticFadeDuration, true);
    }
}
