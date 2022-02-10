using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetMovement : MonoBehaviour {

    public GameObject car;
	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    private void LateUpdate()
    {
        this.transform.position = car.transform.position - new Vector3(0, 10, 0);
    }
}
