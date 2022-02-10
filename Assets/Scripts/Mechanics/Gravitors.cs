//Jarrod Ariola
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravitors : MonoBehaviour {

    /// <summary>
    /// 1.Detect to see if any "Affectors" within the trigger range
    /// 2.Apply and attraction onto the "Affectors" and pull them towards this object
    /// 3.Once they leave the trigger range, do not apply forces onto object
    /// </summary>


    const float G = 6.674f;

    public float mass = 1000f;

    public List<GameObject> AffectorsList;

    void FixedUpdate()
    {
        if (AffectorsList != null)
        {
            foreach (GameObject go in AffectorsList)
            {
                ApplyAttraction(go);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log(name + " is now pullng on " + other.name);
        if(other.GetComponent<Affectors>() != null)
        {
            AffectorsList.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        Debug.Log(other.name + " has left the influence area of " + name);
        AffectorsList.Remove(other.gameObject);
    }

    //Applies Gravitation pull to Affector
    void ApplyAttraction(GameObject objToAffect)
    {
        Rigidbody rbObjToAffect = objToAffect.GetComponent<Rigidbody>();

        Vector3 direction = gameObject.transform.position - rbObjToAffect.position;
        float distance = direction.magnitude;

        float forceMagnitude = G * (rbObjToAffect.mass * mass) / Mathf.Pow(distance, 2);
        Vector3 force = direction.normalized * forceMagnitude;

        rbObjToAffect.AddForce(force);
    }
}
