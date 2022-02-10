using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentTrackListManager : MonoBehaviour {

    public TrackListData trackList;
    public GameObject currentTrackGameObject;
    Transform contentPanel;
    public TrackObjectPool buttonObjectPool;
    public TrackGenerationManager trackGenerationManager;

    // Use this for initialization
    void Start () {
        contentPanel = transform;
	}

    void Update()
    {
        
    }

    public void RefreshDisplay()
    {
        RemoveButtons();
        AddButtons();
    }

    private void RemoveButtons()
    {
        while (contentPanel.childCount > 0)
        {
            GameObject toRemove = transform.GetChild(0).gameObject;
            buttonObjectPool.ReturnObject(toRemove);
        }
    }

    private void AddButtons()
    {
        for (int i = 0; i < trackList.trackList.Count; i++)
        {
            Track track = trackList.trackList[i];
            GameObject newButton = buttonObjectPool.GetObject();
            newButton.transform.SetParent(contentPanel);

            TrackPieceButton trackPieceButton = newButton.GetComponent<TrackPieceButton>();
            trackPieceButton.SetUp(track, this);
        }
    }

    public void TrySpawnTrack(GameObject track)
    {
        Debug.Log("Attempted to spawn " + track.name);
        trackGenerationManager.SpawnTrack(track, currentTrackGameObject);
        RefreshDisplay();
    }
}
