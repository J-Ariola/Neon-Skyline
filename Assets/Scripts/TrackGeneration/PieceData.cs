using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceData : MonoBehaviour {
    //This variable represents the approximate radius of the track piece, used to detect collisions with other track pieces
    public float Dist;

    //Holds the start and end connector's of the piece, for ease of access
    public GameObject startConnector;
    public GameObject endConnector;

    //List that contains index of pieces that cannot be used immediately after the current piece
    public List<int> bannedPieces;
}
