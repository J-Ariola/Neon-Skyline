using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedometerHandler : MonoBehaviour {

    public Image fillBar;

    [ColorHtmlProperty]
    public Color beginFillColor;
    [ColorHtmlProperty]
    public Color endFillColor;

    [Range(0, 100)]
    public float maxSpeed;

    public HoverCarController hoverController;

    void Start()
    {
        hoverController = transform.root.GetComponent<HoverCarController>();
    }

    void FixedUpdate () {
        deltaSpeed(hoverController.currentSpeed);
	}

    /// <summary>
    /// As the speed changes, the fill bar will increase as well as the color
    /// </summary>
    /// <param name="speed"></param>
    void deltaSpeed(float speed)
    {
        float amount = (speed / maxSpeed);
        amount = Mathf.Clamp(amount, 0, 1f);
        //Debug.Log(amount);
        //Keeps it within the half circle
        fillBar.fillAmount = amount * 0.5f;
        
        fillBar.color = Color.Lerp(beginFillColor, endFillColor, amount);
    }
}
