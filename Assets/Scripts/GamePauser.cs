using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePauser : MonoBehaviour
{

    public static bool paused;
    public static float originalTimeScale;

    void Start()
    {
        paused = false;
        Time.timeScale = 1F;
        originalTimeScale = Time.timeScale;
    }

    private void Update()
    {
        if (Input.GetKeyUp("escape"))
        {
            if (!paused)
            {
                Pause();
            } else
            {
                Resume();
            }
        }
    }

    public static void Pause()
    {
        originalTimeScale = Time.timeScale;
        Time.timeScale = 0F;
        paused = true;
        Object[] objects = FindObjectsOfType(typeof(GameObject));
        foreach (GameObject go in objects)
        {
            go.SendMessage("OnPauseGame", SendMessageOptions.DontRequireReceiver);
        }
    }

    public static void Resume()
    {
        Time.timeScale = originalTimeScale;
        paused = false;
        Object[] objects = FindObjectsOfType(typeof(GameObject));
        foreach (GameObject go in objects)
        {
            go.SendMessage("OnResumeGame", SendMessageOptions.DontRequireReceiver);
        }
    }
}
