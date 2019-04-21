using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO.Ports;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    // Input and Arduino integration

    public bool usingArduinoInput;
    private SerialPort sp;

    public Image background;

    private bool loading;

    void Start()
    {

        if (usingArduinoInput)
        {
            sp = new SerialPort("COM3", 9600);
            sp.Open();
            sp.ReadTimeout = 1;
        }

        loading = false;
    }

    private void Update()
    {
        if (loading)
        {
            background.color = Color.black;
        }
        else
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
                    if (direction == -2)
                    {
                        loading = true;

                        // Use a coroutine to load the Scene in the background
                        StartCoroutine(LoadYourAsyncScene());
                    }
                }
            }
            else
            {
                if (Input.anyKey)
                {
                    loading = true;

                    // Use a coroutine to load the Scene in the background
                    StartCoroutine(LoadYourAsyncScene());
                }
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
