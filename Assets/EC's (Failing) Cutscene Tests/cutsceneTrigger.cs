using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//ATTACH SCRIPT TO FLAG WITH SPHERE COLLIDER, AND TAG THE SHIP WITH PLAYER

public class cutsceneTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	void OnTriggerEnter(Collider other)
    {
		if(other.gameObject.CompareTag("Player")){			
			if(other.gameObject.GetComponent<PickupScript>().scoreCount == 1){
				SceneManager.LoadScene("CutsceneTest", LoadSceneMode.Additive);
			} else if (other.gameObject.GetComponent<PickupScript>().scoreCount == 2){
				SceneManager.LoadScene("CutsceneTest2", LoadSceneMode.Additive);
			} else if (other.gameObject.GetComponent<PickupScript>().scoreCount == 3){
				//SceneManager.LoadScene("CutsceneTest3", LoadSceneMode.Additive);
			} else if (other.gameObject.GetComponent<PickupScript>().scoreCount == 4){
				//SceneManager.LoadScene("CutsceneTest4", LoadSceneMode.Additive);
			}
			
			Time.timeScale = 0; //pauses main scene
		}
	}
}
