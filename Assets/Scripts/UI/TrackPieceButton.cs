using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackPieceButton : MonoBehaviour {

    public Button buttonComponent;
    public Text nameLabel;

    private Track track;
    private ContentTrackListManager contentTrackListManager;

	public void SetUp(Track currentTrack, ContentTrackListManager currentContentTrackListManager)
    {
        track = currentTrack;
        nameLabel.text = track.Name;
        contentTrackListManager = currentContentTrackListManager;

    }
	
    public void HandleClick()
    {
        contentTrackListManager.TrySpawnTrack(track.track);
        //Debug.Log("Attempting to spawn " + track);
    }
}
