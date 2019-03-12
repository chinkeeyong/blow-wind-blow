using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class moveTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
		if(transform.position.x > 1200){
			transform.Translate(Vector3.left * Time.deltaTime * 100, Camera.main.transform);
		} else {
			SceneManager.UnloadSceneAsync("CutsceneTest");//unload scene, which is obviously not working
		}
    }
}
