using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackGenerationManager : MonoBehaviour {

    GameObject lastTrack;
    TrackListManager trackListManager;
    public Transform contentPanel;
    public GameObject startTrack;


    public float rotationMajorOffset = 90.0f;
    public float positionMajorOffset = 80.0f;

    void Start () {
        if (transform.childCount == 0)
        {
            if (startTrack != null)
            {
                StartTrack();
            }
            else
                Debug.LogError("You do not have a starting piece attached to " + this);
        }
        
	}

    void SendTrackListData()
    {
        ContentTrackListManager contentTrackListManager = contentPanel.GetComponent<ContentTrackListManager>();
        contentTrackListManager.trackList = trackListManager.trackList;
        Debug.Log("Sending TrackData to contentPanel"); 

    }

    public void StartTrack()
    {
        GameObject spawnedTrack = Instantiate(startTrack);
        spawnedTrack.transform.parent = transform;
        //spawnedTrack.GetComponent<TrackListManager>().contentPanel = contentPanel;
    }

    public void SpawnTrack(GameObject track, GameObject currentTrack)
    {
        //First parse the currentTrackGameObject name to determine where you are going to spawn it
        //Do math calculations to spawn object relative to currentTrackGameObject
        Vector3 posOffset = Vec3PosOffset(currentTrack);
        Quaternion quatRotOffset = QuaternionRotOffset(currentTrack);

        Vector3 spawnPos = currentTrack.transform.position + posOffset;
        GameObject spawnedTrack = Instantiate((GameObject)track, spawnPos, quatRotOffset);
        spawnedTrack.transform.parent = transform;
        //spawnedTrack.GetComponent<TrackListManager>().contentPanel = contentPanel;
    }

    public Vector3 Vec3PosOffset(GameObject currentTrack)
    {
        string trackName = currentTrack.name;
        Vector3 posOffset = currentTrack.transform.position;

        //90 degree tracks
        if (trackName.Contains("Straight90Left"))
        {
            posOffset = -currentTrack.transform.right * positionMajorOffset;
        }
        else if (trackName.Contains("Straight90Right"))
        {
            posOffset = currentTrack.transform.right * positionMajorOffset;
        }
        else if (trackName.Contains("TwistLeft"))
        {
            posOffset = currentTrack.transform.forward * positionMajorOffset;
        }
        else if (trackName.Contains("TwistRight"))
        {
            posOffset = currentTrack.transform.forward * positionMajorOffset;
        }

        //45 degree track
        else if (trackName.Contains("45Straight"))
        {
            posOffset = (currentTrack.transform.forward * positionMajorOffset) + (-currentTrack.transform.right * positionMajorOffset);
        }
        else if (trackName.Contains("45Left"))
        {
            posOffset = (currentTrack.transform.forward * positionMajorOffset) + (-currentTrack.transform.right * positionMajorOffset);
        }

        //Else, it is a straight piece
        else
            posOffset = currentTrack.transform.forward * positionMajorOffset;

        return posOffset;
    }

    public Vector3 EulerRotOffset(GameObject currentTrack)
    {
        string trackName = currentTrack.name;
        Vector3 rotOffset = currentTrack.transform.localEulerAngles;

        if (trackName.Contains("Straight90Left"))
        {
            rotOffset += new Vector3(0,-rotationMajorOffset,0);
        }
        else if (trackName.Contains("Straight90Right"))
        {
            rotOffset += new Vector3(0, rotationMajorOffset, 0);
        }
        else if (trackName.Contains("TwistLeft"))
        {
            rotOffset += new Vector3(0,0, rotationMajorOffset);
        }
        else if (trackName.Contains("TwistRight"))
        {
            rotOffset += new Vector3(0, 0, -rotationMajorOffset);
        }
        //Else, it is a straight piece
        else
            rotOffset += new Vector3(0,0,0);

        return rotOffset;
    }

    public Quaternion QuaternionRotOffset(GameObject currentTrack)
    {
        string trackName = currentTrack.name;
        Quaternion rotOffset = currentTrack.transform.localRotation;

        if (trackName.Contains("Straight90Left"))
        {
            rotOffset *= Quaternion.AngleAxis(-rotationMajorOffset, transform.up);
        }
        else if (trackName.Contains("Straight90Right"))
        {
            rotOffset *= Quaternion.AngleAxis(rotationMajorOffset, transform.up);
        }
        else if (trackName.Contains("TwistLeft"))
        {
            rotOffset *= Quaternion.AngleAxis(rotationMajorOffset, transform.forward);
        }
        else if (trackName.Contains("TwistRight"))
        {
            rotOffset *= Quaternion.AngleAxis(-rotationMajorOffset, transform.forward);
        }
        //Else, it is a straight piece
        else
            rotOffset *= Quaternion.identity;

        return rotOffset;
    }
}
