using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DriftmeterHandler : MonoBehaviour {

    public Image fillBarLeft, fillBarRight;


    [ColorHtmlProperty]
    public Color beginFillColor;
    [ColorHtmlProperty]
    public Color endFillColor;

    [Range(0, 100)]
    public float maxDriftPower;

    public HoverCarController hoverController;

    private void Start()
    {
        hoverController = transform.root.GetComponent<HoverCarController>();
    }

    // Update is called once per frame
    void Update () {
        deltaDrift(hoverController.DriftTimer, hoverController.driftTimeMax);
	}

    void deltaDrift(float driftTimer, float maxDriftTimer)
    {
        float amount = driftTimer / maxDriftTimer;
        
        //Creates a scale from (1 and maxDriftPower) and sets it between (0 and 1)
        //float scaledAmt = scale(1, maxDriftPower, 0, 1, amount);
        //Debug.Log(scaledAmt);
        fillBarLeft.fillAmount = fillBarRight.fillAmount = amount;
        fillBarLeft.color = fillBarRight.color = Color.Lerp(beginFillColor, endFillColor, amount);
    }

    float scale (float oldMin, float oldMax, float newMin, float newMax, float value)
    {
        float oldRange = oldMax - oldMin;
        float newRange = newMax - newMin;
        float newVal = (((value - oldMin) * newRange) / oldRange) + newMin;

        return newVal;
    }
}
