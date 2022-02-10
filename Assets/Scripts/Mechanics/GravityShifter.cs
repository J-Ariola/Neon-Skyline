using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityShifter : MonoBehaviour {

    /// <summary>
    /// Will be used to apply sudden shifts in gravity using triggers
    /// </summary>

    [Range(1, 25)]
    public float magnitude = 1.0f;

    public bool showForce;
    public enum directionEnum {Up, Down, Left, Right };
    public directionEnum Direction;

    Transform gravitationalPoint;
    Vector3 forceVec3;
    float gravitationalConstant;


	// Use this for initialization
	void Start () {
        //gravitationalPoint = transform.Find("GravitationalPoint");
        //float forceMagnitude = 1 / (distance);
        //forceVec3 = magnitude * gravitationalConstant * direction.normalized * 0;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        //Debug.DrawLine(transform.position, gravitationalPoint.position, Color.cyan);
	}

    void OnTrigger(Collider other)
    {
        //Debug.Log("Trigger detected from" + transform.name + " to " + other.transform.root.name);
        
        if (other.transform.root.GetComponent<Rigidbody>() != null)
        {
            forceVec3 = AcquireGravitationalVec();

            if (showForce)
                Debug.Log(forceVec3);
            other.transform.root.GetComponent<Rigidbody>().AddForce(forceVec3, ForceMode.Acceleration);
        }
    }

    Vector3 determineDirection()
    {
        Vector3 dir = new Vector3();
        switch (Direction)
        {
            case directionEnum.Up :
                dir = transform.parent.up;
                break;
            case directionEnum.Down:
                dir =  -transform.parent.up;
                break;
            case directionEnum.Left:
                dir = -transform.parent.right;
                break;
            case directionEnum.Right:
                dir = transform.parent.right;
                break;

        }
        return dir;
    }

    Vector3 AcquireGravitationalVec()
    {
        Vector3 direction = determineDirection();
        float distance = direction.magnitude;
        gravitationalConstant = FindObjectOfType<GravityController>().G;

        return magnitude * gravitationalConstant * direction.normalized;
    }
}
