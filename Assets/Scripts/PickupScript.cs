using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupScript : MonoBehaviour
{	
	public int scoreCount; //1 = North, 2 = South, 3 = East, 4 = West
	
    // Start is called before the first frame update
    void Start()
    {
		scoreCount = 0;    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	// EC edited this: Pickup everything that enters the trigger, and set such that last touched object is only one which matters
	// iswear there is an easier way but okay i shouldn't overcomplicate things
    void OnTriggerEnter(Collider other)
    {
		if(other.gameObject.CompareTag("PickupNorth")){
			other.gameObject.SetActive(false);
			scoreCount = 1;
		}
		else if(other.gameObject.CompareTag("PickupSouth")){
			other.gameObject.SetActive(false);
			scoreCount = 2;			
		}
		else if(other.gameObject.CompareTag("PickupEast")){
			other.gameObject.SetActive(false);
			scoreCount = 3;			
		}
		else if(other.gameObject.CompareTag("PickupWest")){
			other.gameObject.SetActive(false);
			scoreCount = 4;			
		}
		
		
		if(other.gameObject.CompareTag("Flag")){
			if(scoreCount == 1){
				other.gameObject.GetComponent<Renderer>().material.color = Color.cyan;
			} else if (scoreCount == 2) {
				other.gameObject.GetComponent<Renderer>().material.color = Color.red;
			} else if (scoreCount == 3) {
				other.gameObject.GetComponent<Renderer>().material.color = Color.magenta;				
			} else if (scoreCount == 4) {
				other.gameObject.GetComponent<Renderer>().material.color = Color.green;				
			}
		}
    }
}
