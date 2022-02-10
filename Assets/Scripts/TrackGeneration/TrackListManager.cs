using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrackListManager : MonoBehaviour {

    public TrackListData trackList;
    Transform contentPanel;

    void Start()
    {
        //TrackGenerationManager trackGenerationManager = transform.parent.GetComponent<TrackGenerationManager>();
        if (contentPanel == null)
        {
            try 
            {
                //GameObject.Find("Content").transform;
                contentPanel = GameObject.Find("Content").transform;
                ContentTrackListManager contentTrackListManager = contentPanel.GetComponent<ContentTrackListManager>();
                contentTrackListManager.trackList = trackList;
                contentTrackListManager.RefreshDisplay();
                contentTrackListManager.currentTrackGameObject = this.gameObject;
            }
            catch (System.Exception e)
            {
                Debug.LogError("Content has not been set on canvas. If this is not a procedurely generated track, please ignore this error.");
                return;
            }
        }
        
        /*if (trackGenerationManager != null)
        {
            contentPanel = transform.parent.GetComponent<TrackGenerationManager>().contentPanel;
        }*/
        if (contentPanel != null)
        {
            
            
        }
    }
}
