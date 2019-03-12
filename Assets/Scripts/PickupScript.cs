using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickupScript : MonoBehaviour
{	
	public int prevailingWind; //1 = North, 2 = South, 3 = East, 4 = West
    public Text WindText;
	
    // Start is called before the first frame update
    void Start()
    {
		prevailingWind = 0;    
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	// EC edited this: Pickup everything that enters the trigger, and set such that last touched object is only one which matters
	// iswear there is an easier way but okay i shouldn't overcomplicate things
    void OnTriggerEnter(Collider other)
    {
		
		
		if(other.gameObject.CompareTag("Flag")){
            switch (prevailingWind)
            {
                case 1:
                    other.gameObject.GetComponent<Renderer>().material.color = Color.cyan;
                    break;

                case 2:
                    other.gameObject.GetComponent<Renderer>().material.color = Color.red;
                    break;

                case 3:
                    other.gameObject.GetComponent<Renderer>().material.color = Color.magenta;
                    break;

                case 4:
                    other.gameObject.GetComponent<Renderer>().material.color = Color.green;
                    break;

                default:
                    other.gameObject.GetComponent<Renderer>().material.color = Color.white;
                    break;
            }
		}
    }
}
