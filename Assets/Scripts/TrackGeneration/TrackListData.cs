using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Track
{
    public GameObject track;

    public string Name
    {
        get { return track.transform.name; }
    }
}

[CreateAssetMenu(fileName = "Data", menuName = "Track List Data", order = 2)]
public class TrackListData : ScriptableObject {

    public List<Track> trackList;
    //public Transform contentPanel;
    //public TrackObjectPool buttonObjectPool;
    /*
	// Use this for initialization
	void Start () {
        RefreshDisplay();
	}
	
    void RefreshDisplay()
    {
        RemoveButtons();
        AddButtons();
    }

    private void RemoveButtons()
    {
        while (contentPanel.childCount > 0)
        {
            GameObject toRemove = transform.GetChild(0).gameObject;
            
        }
    }

    private void AddButtons()
    {
        for (int i = 0; i < trackList.Count; i++)
        {
            Track track = trackList[i];
            GameObject newButton = buttonObjectPool.GetObject();
            newButton.transform.SetParent(contentPanel);

            TrackPieceButton trackPieceButton = newButton.GetComponent<TrackPieceButton>();
            trackPieceButton.SetUp(track, this);
        }
    }
	
    public void TrySpawnTrack(Track track)
    {
        Debug.Log("Attempted");
        RefreshDisplay();
    }*/
}
