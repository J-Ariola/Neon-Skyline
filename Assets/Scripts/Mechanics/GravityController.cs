//Jarrod Ariola
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityController : MonoBehaviour {

    //const float G = 6.674f;
    [Range (5,25)]
    public float G = 10.0f;

    Rigidbody carRigidBody;

    [Header("Ray Distances")]
    //Distance of the ray once it has launched from the ramp, it must be shorter than hoverHeight
    [Range(1, 25)]
    public float shortGravityRayDetectionDistance = 0.5f;
    [Range (1,25)]
    public float gravityRayDetectionDistance = 1;
    [Range(1, 10)]
    public float inAirRayDetectionDistance = 1;
    [Range(0.1f, 5)]
    public float rotationRate;
    [Header("Hover Forces")]
    [Range(1, 25000)]
    public float hoverForce = 65f;
    [Range(0.1f, 250)]
    public float hoverHeight = 1.5f;
    [Range (1, 2)]
    public float rampGravBoost = 1.25f;
    Vector3 inverseVec;
    Vector3 lastAppliedForce;
    Quaternion lastAppliedRot;
    [Header("Booleans")]
    public bool applyGravity;
    public bool showLines;
    public bool showGravity;
    public bool checkForTrackTag;
    public bool detectRamp;
    bool inAir;

    //Either in floating state for jumping off ramps or magnet to stick to the track
    public enum gravStateEnum {floating, falling, magnet};
    public gravStateEnum gravState;

    [Tooltip("The last track piece the car was in contact")]
    public GameObject lastContactTrack;

    [Tooltip("The track piece behind lastContactTrack")]
    public GameObject previousContactTrack;

    // Use this for initialization
    void Start () {
        //ray = new Ray(transform.position,transform.forward);
        if (shortGravityRayDetectionDistance > hoverHeight)
        {
            //Debug.LogError("shortGravityRayDetectionDistance is not less than hoverHeight!");
        }
        carRigidBody = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        
        RaycastHit hit;
        Ray gravityRay = new Ray(transform.position, -transform.up);
        Ray inAirRay = new Ray(transform.position, -transform.up);
        Ray cancelFloatingRay = new Ray(transform.position, -transform.up);

        if (showLines)
        {
            Debug.DrawRay(gravityRay.origin, gravityRay.direction * gravityRayDetectionDistance, Color.white);
            Debug.DrawRay(inAirRay.origin, inAirRay.direction * inAirRayDetectionDistance, Color.blue);
        }

        //When the car is in the air
        if (Physics.Raycast(inAirRay, out hit, inAirRayDetectionDistance))
        {
            if (checkForTrackTag)
            {
                if (hit.collider.tag.ToUpper().Contains("TRACK"))
                {
                    inAir = false;
                    gravState = gravStateEnum.magnet;
                }
                else
                {
                    inAir = true;
                    //ApplyAttraction(lastAppliedForce);
                    gravState = gravStateEnum.floating;
                }
            }
            else
            {
                inAir = false;
                gravState = gravStateEnum.magnet;
            }

        }
        else
        {
            inAir = true;
            //ApplyAttraction(lastAppliedForce);
            gravState = gravStateEnum.floating;
        }

        if (inAir)
        {
            if (Physics.Raycast(cancelFloatingRay, out hit, shortGravityRayDetectionDistance))
            {
                if (checkForTrackTag)
                {
                    if (hit.collider.tag.ToUpper().Contains("TRACK"))
                    {
                        gravState = gravStateEnum.falling;
                    }
                }
                else
                {
                    gravState = gravStateEnum.falling;
                }
            }
            else
            {
                gravState = gravStateEnum.floating;
            }
        }

        //If the ray hits something find the normal of the object that the ray hit.
        if (Physics.Raycast(gravityRay, out hit, gravityRayDetectionDistance))
        {
            if (hit.collider.tag.ToUpper().Contains("RAMP"))
            {
                //Debug.Log("Ray detected ramp");
                detectRamp = true;
            }
            else
            {
                detectRamp = false;
            }
            if (checkForTrackTag) 
            {
                if (hit.collider.tag.ToUpper().Contains("TRACK"))
                {
                    GameObject recentTrack = hit.transform.parent.gameObject;

                    if (lastContactTrack == null)
                        lastContactTrack = recentTrack;

                    if (lastContactTrack != recentTrack)
                    {
                        previousContactTrack = lastContactTrack;
                        lastContactTrack = recentTrack;
                    }

                    Vector3 incomingVec = hit.point - transform.position;
                    Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);

                    if (showLines)
                    {
                        Debug.DrawLine(transform.position, hit.point, Color.green);
                        Debug.DrawRay(hit.point, reflectVec, Color.red);
                    }

                    //Apply gravity based on normal
                    if (applyGravity)
                    {
                        ApplyAttraction(hit);
                    }

                    //Keeps the car in the air based on hoverHeight
                    if (Physics.Raycast(gravityRay, out hit, hoverHeight))
                    {
                        float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
                        Vector3 appliedHoverForce = transform.up * proportionalHeight * hoverForce;
                        carRigidBody.AddForce(appliedHoverForce, ForceMode.Acceleration);
                    }
                }

            }
            else
            {
                GameObject recentTrack = hit.transform.parent.gameObject;

                if (lastContactTrack == null)
                    lastContactTrack = recentTrack;

                if (lastContactTrack != recentTrack)
                {
                    previousContactTrack = lastContactTrack;
                    lastContactTrack = recentTrack;
                }

                Vector3 incomingVec = hit.point - transform.position;
                Vector3 reflectVec = Vector3.Reflect(incomingVec, hit.normal);

                if (showLines)
                {
                    Debug.DrawLine(transform.position, hit.point, Color.green);
                    Debug.DrawRay(hit.point, reflectVec, Color.red);
                }

                //Apply gravity based on normal
                if (applyGravity)
                {
                    ApplyAttraction(hit);
                }

                //Keeps the car in the air based on hoverHeight
                if (Physics.Raycast(gravityRay, out hit, hoverHeight))
                {
                    float proportionalHeight = (hoverHeight - hit.distance) / hoverHeight;
                    Vector3 appliedHoverForce = transform.up * proportionalHeight * hoverForce;
                    carRigidBody.AddForce(appliedHoverForce, ForceMode.Acceleration);
                }
            }
        }

        

        }

    void ApplyAttraction(RaycastHit hit)
    {
        //Move towards the object's face
        Vector3 direction = hit.point - transform.position;
        float distance = direction.magnitude;
        //float distance = hit.distance;

        float forceMagnitude = 0.0f;
        switch (gravState)
        {
            case gravStateEnum.magnet:
                forceMagnitude = G * hit.distance * 1.5f;
                break;
            case gravStateEnum.falling:
                forceMagnitude = G * rampGravBoost * 10;
                break;
            case gravStateEnum.floating:
                forceMagnitude = rampGravBoost *  G;
                break;
        }

        Vector3 force = direction.normalized * forceMagnitude;

        if (showGravity)
            Debug.Log("Gravitational for in Vec3: " + force);

        //lastAppliedForce = force;
        carRigidBody.AddForce(force, ForceMode.Acceleration);

        //End result of the rotation, now you need to lerp it
        //Will always cause the wheels of the car to rotate towards the track

        Quaternion targetRot = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, Time.deltaTime * rotationRate);
        lastAppliedRot = targetRot;
    }

    void ApplyAttraction(Vector3 force)
    {
        carRigidBody.AddForce(lastAppliedForce * rampGravBoost, ForceMode.Acceleration);
        transform.rotation = Quaternion.Lerp(transform.rotation, lastAppliedRot, Time.deltaTime * rotationRate);
    }

    float GravityForce(float massObj1, float massObj2, float distance)
    {
        return (G * (massObj1 * massObj2)) / Mathf.Pow(distance, 2);
    }

    //Properties
    public bool InAir
    {
        get { return inAir; }
    }
}
