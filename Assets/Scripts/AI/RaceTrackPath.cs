using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceTrackPath : MonoBehaviour {

    [ColorHtmlProperty]
    public Color lineColor;

    //The EndConnector of every track piece needs to be added to this list
    public List<Transform> nodes = new List<Transform>();

    //public List<Transform> tracks = new List<Transform>();

    //public int maxLaps;
    
    void OnDrawGizmos()
    {
        Gizmos.color = lineColor;

        Transform[] trackPiecesTransform = GetComponentsInChildren<Transform>();
        nodes = new List<Transform>();
        //tracks = new List<Transform>();
        //findTrackTag(GameObject.FindGameObjectsWithTag("Track"));

        for (int i = 0; i < trackPiecesTransform.Length; i++)
        {
            //If not our own transform
            if (trackPiecesTransform[i] != transform.gameObject)
            {
                if (trackPiecesTransform[i].name.Equals("EndConnector") || trackPiecesTransform[i].name.Equals("Center"))
                {
                    nodes.Add(trackPiecesTransform[i]);
                }
                //nodes.Add(trackPiecesTransform[i]);
            }
        }

        //Draws the path
        for (int i = 0; i < nodes.Count; i++)
        {
            Vector3 currentNode = nodes[i].position;
            Vector3 nextNode = Vector3.zero;
            Vector3 previousNode = Vector3.zero;
            
            if (i > 0)
            {
                if (i == nodes.Count - 1 && nodes.Count > 1)
                {
                    previousNode = nodes[i - 1].position;
                    nextNode = nodes[0].position;
                }
                else
                {
                    previousNode = nodes[i - 1].position;
                    nextNode = nodes[i + 1].position;
                }
            }
            //Completes a circuit and grabs the last node
            else if (i == 0 && nodes.Count > 1)
            {
                previousNode = nodes[nodes.Count - 1].position;
                nextNode = nodes[i + 1].position;
            }

            Gizmos.DrawLine(previousNode, currentNode);
            Gizmos.DrawWireSphere(currentNode, 0.3f);
            Gizmos.DrawWireCube(FindMidPoint(currentNode, nextNode), new Vector3(0.3f, 0.3f, 0.3f));
        }
    }

    /*
    void findTrackTag (GameObject[] trackGOs)
    {
        foreach (GameObject go in trackGOs)
        {
            tracks.Add(go.transform);
        }
    }
    */
    Transform findEndConnector(Transform trackPiece)
    {
        return trackPiece.transform;
    }

    Vector3 FindMidPoint(Vector3 v1, Vector3 v2)
    {
        return ((v1 + v2) / 2);
    }
}
