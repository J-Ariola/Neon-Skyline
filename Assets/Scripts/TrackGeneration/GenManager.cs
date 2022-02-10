using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenManager : MonoBehaviour
{
    public int pieceCount;

    public bool useRandSeed = false;
    public string seed = "test";
    System.Random seededRandom;
    public GameObject racetrack;
    SeedGenerator seedGen;
    
    //index 0 should be start tunnel + lap line
    //index 1 should be end tunnel
    public List<GameObject> trackPieces;
    List<int> bannedPieces;

    List<GameObject> currentTrack;
    GameObject previousPiece;
    int currentIndex;
    int timesFailed;
    int failedOffset = 0;

    bool allAdded = false;

    // Use this for initialization
    void Start ()
    {
        seed = PlayerPrefs.GetString("SeedInput");

        //if the player doesn't enter a seed, use a random one
        if(seed == "")
        {
            useRandSeed = true;
        }

        pieceCount = PlayerPrefs.GetInt("TrackPieces");

        //if the player enters less than 2 or nothing, set the default size to 25
        if(pieceCount < 2)
        {
            pieceCount = 25;
        }
        //if the player inputs more than 50, set the max to be 50
        else if(pieceCount > 50)
        {
            pieceCount = 50;
        }
        allAdded = false;
        currentIndex = 0;
        timesFailed = 0;

        seedGen = GetComponent<SeedGenerator>();
        if (useRandSeed)
        {
            seed = seedGen.seed;
        }
        seededRandom = new System.Random(seed.GetHashCode());

        currentTrack = new List<GameObject>();
        bannedPieces = new List<int>();

        //place starting piece
        GameObject s = Instantiate(trackPieces[0]);
        currentTrack.Add(s);
        previousPiece = currentTrack[currentIndex];
        currentIndex++;

        //Place generation code here if possible
        while (currentIndex < pieceCount)
        {
            
            #region proceduralcode

            if (bannedPieces.Count >= trackPieces.Count - 2)
            {
                //inefficiency that can be fixed later: track may just select the same piece next time
                currentIndex--;
                GameObject forRemoval = currentTrack[currentIndex];
                currentTrack.RemoveAt(currentIndex);
                GameObject.Destroy(forRemoval);
                previousPiece = currentTrack[currentIndex - 1];

                //reconstructing banned piece list for the previous piece
                bannedPieces.Clear();
                Debug.Log("Banned Pieces Cleared");

                if (previousPiece.GetComponent<PieceData>().bannedPieces.Count > 0)
                {
                    for (int i = 0; i < previousPiece.GetComponent<PieceData>().bannedPieces.Count; i++)
                    {
                        bannedPieces.Add(previousPiece.GetComponent<PieceData>().bannedPieces[i]);
                    }
                    //failedOffset = bannedPieces.Count;
                }
            }

            if (currentIndex == pieceCount - 1)               //If we're placing the last track piece
            {
                //place end piece

                GameObject currentPiece = Instantiate(trackPieces[1]);

                //Snap the spawned piece's start point to the previous one's end point (this is a six step process)
                GameObject start = currentPiece.GetComponent<PieceData>().startConnector;
                GameObject end = previousPiece.GetComponent<PieceData>().endConnector;

                start.transform.SetParent(end.transform);                                                   //Step 2: Set current piece's start connector to be the child of the previous piece's end connector

                currentPiece.transform.SetParent(start.transform);                                          //Step 3: Set current piece to be its connector's child

                start.transform.localPosition = new Vector3(0, 0, 0);                                       //Step 4: Reset start connector's position/rotation to snap it to end connector
                start.transform.localRotation = Quaternion.identity;

                currentPiece.transform.parent = null;                                                       //Step 5: Unparent current piece from start, now that it's snapped

                start.transform.SetParent(currentPiece.transform);                                          //Step 6: Reset child start under the current piece again

                //Check for collisions with previous pieces using distance metric
                //if there is a collision, the current piece will be deleted, its index will be added to the banned pieces list, and the current index will not advance
                //if there is no collision, the current piece will be added to the piece list and become the previous piece, the banned pieces list will clear, and the current index will advance
                float myDist = currentPiece.GetComponent<PieceData>().Dist;
                bool failed = false;

                //checking against every piece but the exact previous one

                for (int i = currentTrack.Count - 2; i >= 0; i--)
                {
                    float otherDist = currentTrack[i].GetComponent<PieceData>().Dist;
                    float comparedDist = Vector3.Distance(currentPiece.transform.position, currentTrack[i].transform.position);
                    if (otherDist + myDist > comparedDist)
                    {
                        failed = true;
                    }
                }

                if (failed)
                {
                    GameObject.Destroy(currentPiece);
                    GameObject forRemoval = currentTrack[currentIndex];
                    currentTrack.RemoveAt(currentIndex);
                    GameObject.Destroy(forRemoval);
                    currentIndex -= 1;
                    previousPiece = currentTrack[currentIndex];
                }
                else
                {
                    currentTrack.Add(currentPiece);
                    previousPiece = currentPiece;
                    bannedPieces.Clear();
                    timesFailed = 0;
                    currentIndex++;
                }
            }
            else                                            //For everything that isn't the last track piece
            {
                //Select random piece that isn't start or end piece
                int selectedPiece = seededRandom.Next(2, trackPieces.Count);


                //Guarantees the selected piece hasn't already been ruled out as a viable piece due to distance formula
                while (bannedPieces.Contains(selectedPiece))
                {
                    selectedPiece = seededRandom.Next(2, trackPieces.Count);
                }

                //Spawn that randomly selected piece
                GameObject currentPiece = Instantiate(trackPieces[selectedPiece]);

                //Snap the spawned piece's start point to the previous one's end point (this is a six step process)
                GameObject start = currentPiece.GetComponent<PieceData>().startConnector;
                GameObject end = previousPiece.GetComponent<PieceData>().endConnector;

                start.transform.SetParent(end.transform);                                                   //Step 2: Set current piece's start connector to be the child of the previous piece's end connector

                currentPiece.transform.SetParent(start.transform);                                          //Step 3: Set current piece to be its connector's child

                start.transform.localPosition = Vector3.zero;                                               //Step 4: Reset start connector's position/rotation to snap it to end connector
                start.transform.localRotation = Quaternion.identity;

                currentPiece.transform.parent = null;                                                       //Step 5: Unparent current piece from start, now that it's snapped

                start.transform.SetParent(currentPiece.transform);                                          //Step 6: Reset child start under the current piece again

                //Check for collisions with previous pieces using distance metric
                //if there is a collision, the current piece will be deleted, its index will be added to the banned pieces list, and the current index will not advance
                //if there is no collision, the current piece will be added to the piece list and become the previous piece, the banned pieces list will clear, and the current index will advance
                float myDist = currentPiece.GetComponent<PieceData>().Dist;
                bool failed = false;


                if (currentTrack.Count > 3)
                {
                    //checking against every piece but the exact previous one
                    for (int i = currentTrack.Count - 2; i >= 0; i--)
                    {
                        float otherDist = currentTrack[i].GetComponent<PieceData>().Dist;
                        float comparedDist = Vector3.Distance(currentPiece.transform.position, currentTrack[i].transform.position);
                        if (otherDist + myDist > comparedDist)
                        {
                            failed = true;
                        }
                    }
                }

                if (failed)
                {
                    GameObject.Destroy(currentPiece);
                    bannedPieces.Add(selectedPiece);
                    timesFailed++;
                    //Debug.Log("failed");
                }
                else
                {
                    currentTrack.Add(currentPiece);
                    previousPiece = currentPiece;
                    bannedPieces.Clear();
                    if (currentPiece.GetComponent<PieceData>().bannedPieces.Count > 0)
                    {
                        for (int i = 0; i < currentPiece.GetComponent<PieceData>().bannedPieces.Count; i++)
                        {
                            bannedPieces.Add(currentPiece.GetComponent<PieceData>().bannedPieces[i]);
                        }
                        //failedOffset = bannedPieces.Count;
                    }
                    timesFailed = 0;
                    currentIndex++;
                }
            }

            #endregion

        }

        if (currentIndex >= pieceCount && !allAdded)
        {
            foreach (GameObject g in currentTrack)
            {
                g.transform.parent = racetrack.transform;
            }
            racetrack.transform.parent = null;
            allAdded = true;
        }
    }

    private void Update()
    {
        if (currentIndex < pieceCount)
        {

        }

        
    }
}
