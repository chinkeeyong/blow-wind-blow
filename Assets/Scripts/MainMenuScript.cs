using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public Text subtitletext;
    private bool loading;

    void Start()
    {
        loading = false;
    }

    private void Update()
    {
        if (loading)
        {
            subtitletext.color = new Color(subtitletext.color.r, subtitletext.color.g, subtitletext.color.b, Mathf.PingPong(Time.time, 1));
        } else
        {
            if (Input.anyKey)
            {
                loading = true;
                subtitletext.text = "Loading...";

                // Use a coroutine to load the Scene in the background
                StartCoroutine(LoadYourAsyncScene());
            }
        }
    }

    void OnSceneLoaded(Scene scene)
    {
        SceneManager.SetActiveScene(scene);
        SceneManager.UnloadSceneAsync(SceneManager.GetSceneByName("MainMenuScene"));
    }

    IEnumerator LoadYourAsyncScene()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("MainScene");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
