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

    public Image blowToContinueSign;

    public Sprite mural0;
    public Sprite mural11;
    public Sprite mural12;
    public Sprite mural13;
    public Sprite mural14;
    public Sprite mural21;
    public Sprite mural22;
    public Sprite mural23;
    public Sprite mural24;
    public Sprite mural31;
    public Sprite mural32;
    public Sprite mural33;
    public Sprite mural34;

    public Text credits;

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

    public static Image staticBlowToContinueSign;

    public static Sprite staticMural0;
    public static Sprite staticMural11;
    public static Sprite staticMural12;
    public static Sprite staticMural13;
    public static Sprite staticMural14;
    public static Sprite staticMural21;
    public static Sprite staticMural22;
    public static Sprite staticMural23;
    public static Sprite staticMural24;
    public static Sprite staticMural31;
    public static Sprite staticMural32;
    public static Sprite staticMural33;
    public static Sprite staticMural34;

    public static int staticFrameWidth;
    public static float staticFadeDuration;
    public static float timePaused;
    public static Image img;
    public static Animator animator;
    public static bool active;

    public static bool endingReached;
    
    void Start()
    {
        active = false;
        endingReached = false;

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

        staticBlowToContinueSign = blowToContinueSign;

        staticMural0 = mural0;
        staticMural11 = mural11;
        staticMural12 = mural12;
        staticMural13 = mural13;
        staticMural14 = mural14;
        staticMural21 = mural21;
        staticMural22 = mural22;
        staticMural23 = mural23;
        staticMural24 = mural24;
        staticMural31 = mural31;
        staticMural32 = mural32;
        staticMural33 = mural33;
        staticMural34 = mural34;

        img.color = new Color(1F, 1F, 1F, 1F);
        credits.color = new Color(1F, 1F, 1F, 1F);
        staticBlowToContinueSign.color = new Color(1F, 1F, 1F, 1F);

        staticNorthWindCutout.canvasRenderer.SetAlpha(0F);
        staticSouthWindCutout.canvasRenderer.SetAlpha(0F);
        staticEastWindCutout.canvasRenderer.SetAlpha(0F);
        staticWestWindCutout.canvasRenderer.SetAlpha(0F);
        staticNorthWindImage.canvasRenderer.SetAlpha(0F);
        staticSouthWindImage.canvasRenderer.SetAlpha(0F);
        staticEastWindImage.canvasRenderer.SetAlpha(0F);
        staticWestWindImage.canvasRenderer.SetAlpha(0F);
        staticBlowToContinueSign.canvasRenderer.SetAlpha(0F);

        credits.canvasRenderer.SetAlpha(0F);

        Show(0);
    }
    
    void Update()
    {
        if (active)
        {
            if (Time.realtimeSinceStartup > timePaused + timeUntilDismissable)
            {
                if (Compass.currentTargetNo < 3)
                {

                    if (staticBlowToContinueSign.canvasRenderer.GetAlpha() < 0.00001F)
                    {
                        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                        {
                            staticBlowToContinueSign.CrossFadeAlpha(1F, staticFadeDuration, true);
                        }
                    }

                    if (Wind.velocity.magnitude > 0F)
                    {
                        Hide();
                    }
                }
                else
                {
                    if (credits.canvasRenderer.GetAlpha() < 0.00001F)
                    {
                        if (!endingReached)
                        {
                            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.75)
                            {
                                endingReached = true;
                            }
                        }
                        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1)
                        {
                            credits.CrossFadeAlpha(1F, staticFadeDuration, true);
                        }
                    }
                }
            }
        }
    }

    public static void Show(int muralId)
    {
        GamePauser.Pause();

        float screenHeight;
        float screenWidth;
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

            case 21: // Reached 2nd city as North
                // Set the mural
                img.sprite = staticMural21;
                // Get ratio of screen height to mural height
                screenHeight = Screen.height - (2 * staticFrameWidth);
                heightRatio = screenHeight / img.mainTexture.height;
                newWidth = img.mainTexture.width * heightRatio;
                // Scale entire image
                img.rectTransform.sizeDelta = new Vector2(newWidth, screenHeight);
                // Play animation
                animator.Play("Mural2");
                break;

            case 22: // Reached 2nd city as South
                // Set the mural
                img.sprite = staticMural22;
                // Get ratio of screen height to mural height
                screenHeight = Screen.height - (2 * staticFrameWidth);
                heightRatio = screenHeight / img.mainTexture.height;
                newWidth = img.mainTexture.width * heightRatio;
                // Scale entire image
                img.rectTransform.sizeDelta = new Vector2(newWidth, screenHeight);
                // Play animation
                animator.Play("Mural2");
                break;

            case 23: // Reached 2nd city as East
                // Set the mural
                img.sprite = staticMural23;
                // Get ratio of screen height to mural height
                screenHeight = Screen.height - (2 * staticFrameWidth);
                heightRatio = screenHeight / img.mainTexture.height;
                newWidth = img.mainTexture.width * heightRatio;
                // Scale entire image
                img.rectTransform.sizeDelta = new Vector2(newWidth, screenHeight);
                // Play animation
                animator.Play("Mural2");
                break;

            case 24: // Reached 2nd city as West
                // Set the mural
                img.sprite = staticMural24;
                // Get ratio of screen height to mural height
                screenHeight = Screen.height - (2 * staticFrameWidth);
                heightRatio = screenHeight / img.mainTexture.height;
                newWidth = img.mainTexture.width * heightRatio;
                // Scale entire image
                img.rectTransform.sizeDelta = new Vector2(newWidth, screenHeight);
                // Play animation
                animator.Play("Mural2");
                break;

            case 31: // Reached 3rd city as North
                // Set the mural
                img.sprite = staticMural31;
                // Get ratio of screen width to mural width
                screenWidth = Screen.width - (2 * staticFrameWidth);
                widthRatio = screenWidth / img.mainTexture.width;
                newHeight = img.mainTexture.height * widthRatio;
                // Scale entire image
                img.rectTransform.sizeDelta = new Vector2(screenWidth, newHeight);
                // Play animation
                animator.Play("Mural3");
                break;

            case 32: // Reached 3rd city as South
                // Set the mural
                img.sprite = staticMural32;
                // Get ratio of screen width to mural width
                screenWidth = Screen.width - (2 * staticFrameWidth);
                widthRatio = screenWidth / img.mainTexture.width;
                newHeight = img.mainTexture.height * widthRatio;
                // Scale entire image
                img.rectTransform.sizeDelta = new Vector2(screenWidth, newHeight);
                // Play animation
                animator.Play("Mural3");
                break;

            case 33: // Reached 3rd city as East
                // Set the mural
                img.sprite = staticMural33;
                // Get ratio of screen width to mural width
                screenWidth = Screen.width - (2 * staticFrameWidth);
                widthRatio = screenWidth / img.mainTexture.width;
                newHeight = img.mainTexture.height * widthRatio;
                // Scale entire image
                img.rectTransform.sizeDelta = new Vector2(screenWidth, newHeight);
                // Play animation
                animator.Play("Mural3");
                break;

            case 34: // Reached 3rd city as West
                // Set the mural
                img.sprite = staticMural34;
                // Get ratio of screen width to mural width
                screenWidth = Screen.width - (2 * staticFrameWidth);
                widthRatio = screenWidth / img.mainTexture.width;
                newHeight = img.mainTexture.height * widthRatio;
                // Scale entire image
                img.rectTransform.sizeDelta = new Vector2(screenWidth, newHeight);
                // Play animation
                animator.Play("Mural3");
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
        staticBlowToContinueSign.CrossFadeAlpha(0F, staticFadeDuration, true);
    }
}
