using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cutscene : MonoBehaviour
{
    
    public Image northWindCutout;
    public Image southWindCutout;
    public Image eastWindCutout;
    public Image westWindCutout;
    public Image northWindImage;
    public Image southWindImage;
    public Image eastWindImage;
    public Image westWindImage;

    public Sprite mural0;
    public Sprite mural11;
    public Sprite mural12;
    public Sprite mural13;
    public Sprite mural14;

    public int frameWidth;
    public float fadeDuration;
    public float timeUntilDismissable;

    public static Image staticNorthWindCutout;
    public static Image staticSouthWindCutout;
    public static Image staticEastWindCutout;
    public static Image staticWestWindCutout;
    public static Image staticNorthWindImage;
    public static Image staticSouthWindImage;
    public static Image staticEastWindImage;
    public static Image staticWestWindImage;

    public static Sprite staticMural0;
    public static Sprite staticMural11;
    public static Sprite staticMural12;
    public static Sprite staticMural13;
    public static Sprite staticMural14;

    public static int staticFrameWidth;
    public static float staticFadeDuration;
    public static float timePaused;
    public static Image img;
    public static Animator animator;
    public static bool active;
    
    void Start()
    {
        active = false;

        img = gameObject.GetComponent(typeof(Image)) as Image;
        animator = gameObject.GetComponent(typeof(Animator)) as Animator;

        staticFrameWidth = frameWidth;
        staticFadeDuration = fadeDuration;

        staticNorthWindCutout = northWindCutout;
        staticSouthWindCutout = southWindCutout;
        staticEastWindCutout = eastWindCutout;
        staticWestWindCutout = westWindCutout;
        staticNorthWindImage = northWindImage;
        staticSouthWindImage = southWindImage;
        staticEastWindImage = eastWindImage;
        staticWestWindImage = westWindImage;

        staticMural0 = mural0;
        staticMural11 = mural11;
        staticMural12 = mural12;
        staticMural13 = mural13;
        staticMural14 = mural14;

        img.color = new Color(1F, 1F, 1F, 1F);
        staticNorthWindCutout.canvasRenderer.SetAlpha(0F);
        staticSouthWindCutout.canvasRenderer.SetAlpha(0F);
        staticEastWindCutout.canvasRenderer.SetAlpha(0F);
        staticWestWindCutout.canvasRenderer.SetAlpha(0F);
        staticNorthWindImage.canvasRenderer.SetAlpha(0F);
        staticSouthWindImage.canvasRenderer.SetAlpha(0F);
        staticEastWindImage.canvasRenderer.SetAlpha(0F);
        staticWestWindImage.canvasRenderer.SetAlpha(0F);

        Show(0);
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

    public static void Show(int muralId)
    {
        GamePauser.Pause();

        float screenHeight;
        float heightRatio;
        float widthRatio;
        float newHeight;
        float newWidth;

        // Size sprite to screen
        switch(muralId)
        {
            case 0: // Intro cutscene
                // Set the mural
                img.sprite = staticMural0;
                // Get ratio of screen height to mural height
                screenHeight = Screen.height - (2 * staticFrameWidth);
                heightRatio = screenHeight / img.mainTexture.height;
                newWidth = img.mainTexture.width * heightRatio;
                // Scale entire image
                img.rectTransform.sizeDelta = new Vector2(newWidth, screenHeight);
                // Play animation
                animator.Play("Mural0");
                break;

            case 11: // Reached 1st city as North
                // Set the mural
                img.sprite = staticMural11;
                // Get ratio of screen height to mural height
                screenHeight = Screen.height - (2 * staticFrameWidth);
                heightRatio = screenHeight / img.mainTexture.height;
                newWidth = img.mainTexture.width * heightRatio;
                // Scale entire image
                img.rectTransform.sizeDelta = new Vector2(newWidth, screenHeight);
                // Play animation
                animator.Play("Mural1");
                break;

            case 12: // Reached 1st city as South
                // Set the mural
                img.sprite = staticMural12;
                // Get ratio of screen height to mural height
                screenHeight = Screen.height - (2 * staticFrameWidth);
                heightRatio = screenHeight / img.mainTexture.height;
                newWidth = img.mainTexture.width * heightRatio;
                // Scale entire image
                img.rectTransform.sizeDelta = new Vector2(newWidth, screenHeight);
                // Play animation
                animator.Play("Mural1");
                break;

            case 13: // Reached 1st city as East
                // Set the mural
                img.sprite = staticMural13;
                // Get ratio of screen height to mural height
                screenHeight = Screen.height - (2 * staticFrameWidth);
                heightRatio = screenHeight / img.mainTexture.height;
                newWidth = img.mainTexture.width * heightRatio;
                // Scale entire image
                img.rectTransform.sizeDelta = new Vector2(newWidth, screenHeight);
                // Play animation
                animator.Play("Mural1");
                break;

            case 14: // Reached 1st city as West
                // Set the mural
                img.sprite = staticMural14;
                // Get ratio of screen height to mural height
                screenHeight = Screen.height - (2 * staticFrameWidth);
                heightRatio = screenHeight / img.mainTexture.height;
                newWidth = img.mainTexture.width * heightRatio;
                // Scale entire image
                img.rectTransform.sizeDelta = new Vector2(newWidth, screenHeight);
                // Play animation
                animator.Play("Mural1");
                break;

            default:
                break;
        }

        timePaused = Time.realtimeSinceStartup;
        active = true;
        img.CrossFadeAlpha(1F, staticFadeDuration, true);
        staticNorthWindCutout.CrossFadeAlpha(0F, staticFadeDuration, true);
        staticSouthWindCutout.CrossFadeAlpha(0F, staticFadeDuration, true);
        staticEastWindCutout.CrossFadeAlpha(0F, staticFadeDuration, true);
        staticWestWindCutout.CrossFadeAlpha(0F, staticFadeDuration, true);
        staticNorthWindImage.CrossFadeAlpha(0F, staticFadeDuration, true);
        staticSouthWindImage.CrossFadeAlpha(0F, staticFadeDuration, true);
        staticEastWindImage.CrossFadeAlpha(0F, staticFadeDuration, true);
        staticWestWindImage.CrossFadeAlpha(0F, staticFadeDuration, true);
    }

    public static void Hide()
    {
        GamePauser.Resume();
        active = false;
        img.CrossFadeAlpha(0F, staticFadeDuration, true);
        staticNorthWindCutout.CrossFadeAlpha(1F, staticFadeDuration, true);
        staticSouthWindCutout.CrossFadeAlpha(1F, staticFadeDuration, true);
        staticEastWindCutout.CrossFadeAlpha(1F, staticFadeDuration, true);
        staticWestWindCutout.CrossFadeAlpha(1F, staticFadeDuration, true);
        staticNorthWindImage.CrossFadeAlpha(1F, staticFadeDuration, true);
        staticSouthWindImage.CrossFadeAlpha(1F, staticFadeDuration, true);
        staticEastWindImage.CrossFadeAlpha(1F, staticFadeDuration, true);
        staticWestWindImage.CrossFadeAlpha(1F, staticFadeDuration, true);
    }
}
