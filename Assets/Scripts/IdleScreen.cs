using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IdleScreen : MonoBehaviour
{

    public CanvasGroup idleScreen;
    public Text countdown;
    public float timeUntilIdleScreen;
    public float timeUntilGameReset;

    private float timeSinceLastInput;
    private bool loading;

    private void Start()
    {
        idleScreen.alpha = 0F;
        loading = false;
    }

    void Update() {

        // Instantly start loading and return if the game has ended
        if (Cutscene.okToLoadMainMenu)
        {
            StartCoroutine(LoadYourAsyncScene());
            loading = true;
            GamePauser.Pause();
            return;
        }

        // Return if we have an input or we are loading
        if (Wind.velocity.magnitude > 0F || loading || GamePauser.paused)
        {
            timeSinceLastInput = 0F;
        }
        else
        {   // We have no inputs and the game is not paused. Add delta time to timeSinceLastInput
            timeSinceLastInput += Time.deltaTime;
        }


        // Trigger idle screen?

        if (timeSinceLastInput > timeUntilIdleScreen)
        {
            if (timeSinceLastInput > timeUntilGameReset)
            {
                StartCoroutine(LoadYourAsyncScene());
                countdown.text = "Returning to title screen...";
                loading = true;
            }
            else
            {
                int _countdownNumber = Mathf.FloorToInt(timeUntilGameReset - timeSinceLastInput);
                countdown.text = _countdownNumber.ToString();
            }
            idleScreen.alpha = Mathf.Lerp(idleScreen.alpha, 1F, Time.deltaTime * 5);
        }
        else
        {
            idleScreen.alpha = Mathf.Lerp(idleScreen.alpha, 0F, Time.deltaTime * 5);
        }
    }

    void OnSceneLoaded(Scene scene)
    {
        SceneManager.SetActiveScene(scene);
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("MainScene"));
    }

    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainMenuScene");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
