using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelFadeHandler : MonoBehaviour {
    [Range(0, 2)]
    public float fadeSpeed = 1.0f;
    public Texture2D fadeOutTexture;

    private int fadeDir = -1;   // -1 for fade in or 1 for fade out
    private float alpha = 1.0f;
    private int drawDepth = -1000;

    void OnGUI()
    {
        FadeHandling();
    }

    void FadeHandling()
    {
        alpha += fadeDir * fadeSpeed * Time.deltaTime;
        alpha = Mathf.Clamp01(alpha);

        //fadeScreenImage.color = new Color(0, 0, 0, alpha);
        GUI.color = new Color(0, 0, 0, alpha);
        GUI.depth = drawDepth;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);
        /*
        if (alpha == 0)
        {
            fadeScreen.SetActive(false);
        }
        else
            fadeScreen.SetActive(true);*/
    }

    public float StartFade(int direction)
    {
        fadeDir = direction;
        return (fadeSpeed);
    }
}
