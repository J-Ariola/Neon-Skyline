//Jarrod Ariola
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Affectors : MonoBehaviour {

    public Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError(name + ": You must attach a Rigidbody to this game object!");
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
