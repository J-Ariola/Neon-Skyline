using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustMass : MonoBehaviour {

    public float massChangeVal;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag.Equals("MassChange"))
        {
            this.GetComponent<Rigidbody>().mass *= massChangeVal;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("MassChange"))
        {
            this.GetComponent<Rigidbody>().mass /= massChangeVal;
        }
    }
}
