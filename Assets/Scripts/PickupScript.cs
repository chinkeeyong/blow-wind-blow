using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupScript : MonoBehaviour
{	

	public int northCount;
	public int southCount;
	public int eastCount;
	public int westCount;
	
    // Start is called before the first frame update
    void Start()
    {
		northCount = 0;
		southCount = 0;
		eastCount = 0;
		westCount = 0;        
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
			northCount += 1;
			southCount = 0;
			eastCount = 0;
			westCount = 0;
		}
		else if(other.gameObject.CompareTag("PickupSouth")){
			other.gameObject.SetActive(false);
			northCount = 0;
			southCount += 1;
			eastCount = 0;
			westCount = 0;
		}
		else if(other.gameObject.CompareTag("PickupEast")){
			other.gameObject.SetActive(false);
			northCount = 0;
			southCount = 0;
			eastCount += 1;
			westCount = 0;
		}
		else if(other.gameObject.CompareTag("PickupWest")){
			other.gameObject.SetActive(false);
			northCount = 0;
			southCount = 0;
			eastCount = 0;
			westCount += 1;
		}
    }
}
