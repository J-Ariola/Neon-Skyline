using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class CameraController : MonoBehaviour {

    public Transform target;
    public float distance;
    public float height;
    public float rotationDamping;
    public float heightDamping;
    public float defaultFOV;
    public float zoomRatio;
    //right stick controls for camera, allows the player to move the camera with a restricted limit
    float lookRotX;
    float lookRotY;

    //used to seperate camera controls between players
    public float playerID;

    //value that increases the camera's look limit
    public float lookXValue;
    public float lookYValue;
    
    //inverses the camera controls
    public bool inverseCamera;

    public bool followBehind;

    private Vector3 rotation;

    private void Start()
    {
        followBehind = true;
        

    }

    // Update is called once per frame
    void FixedUpdate () {
        if (GameObject.FindGameObjectWithTag("Player" + playerID) != null)
        {
            if (GameObject.FindGameObjectWithTag("Player" + playerID).name.Contains("Minivan"))
            {
                distance = 7f;
                height = 2.5f;
            }
        }

        if (GameObject.FindGameObjectWithTag("Player" + playerID) != null)
            target = GameObject.FindGameObjectWithTag("Player" + playerID).GetComponent<Transform>();

        if (target == null)
        {
            return;
        }

        //zooms camera out while the car picks up speed
        if (followBehind)
        {
            float acceleration = target.GetComponentInParent<Rigidbody>().velocity.magnitude;
            GetComponent<Camera>().fieldOfView = defaultFOV + acceleration * zoomRatio;
        }

        if (target.GetComponent<HoverCarController>().controlsType.ToLower().Contains("xbox"))
        {
            //left and right on right thumb stick
           // lookRotX = -CrossPlatformInputManager.GetAxis("Axis 4 Player " + playerID);
            //up and down on right thumb stick
           // lookRotY = CrossPlatformInputManager.GetAxis("Axis 5 Player " + playerID);
            followBehind = !CrossPlatformInputManager.GetButton("ColorLeftPlayer" + playerID);
        }
        else if (target.GetComponent<HoverCarController>().controlsType.ToLower().Contains("wireless"))
        {
            //left and right on right thumb stick
           // lookRotX = CrossPlatformInputManager.GetAxis("Axis 3 Player " + playerID);
            //up and down on right thumb stick
           // lookRotY = CrossPlatformInputManager.GetAxis("Axis 6 Player " + playerID);
            followBehind = !CrossPlatformInputManager.GetButton("ColorLeftPlayer" + playerID);
        }

        if(inverseCamera)
        {
            lookRotX = -lookRotX;
            lookRotY = -lookRotY;
        }

        Vector3 targetPos;
        Quaternion targetRot;



        //have camera either behind or in front of target(can switch for rear view camera)
        if(followBehind)
        {
            //gets world pos of car's position and determines camera's pos at height and distance values
            targetPos = target.TransformPoint(0, height, -distance);
        }
        else
        {
            targetPos = target.TransformPoint(0, height, distance * 2);
        }

        //lerps the camera's position to the car's position
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * heightDamping);

        Vector3 cameraLook = new Vector3(-lookRotX * lookXValue, -lookRotY * lookYValue, 0);
        //lerps the camera's rotation to the car's rotation
        targetRot = Quaternion.LookRotation((target.position - transform.position) + 
            cameraLook,  target.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * rotationDamping);
    }
}
