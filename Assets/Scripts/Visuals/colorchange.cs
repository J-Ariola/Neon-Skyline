using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class colorchange : MonoBehaviour {

    public Material material;
    public List<Color> colors;
    public int colorcounter = 0;
    public float bpm = 120.0f;
    float timer = 0.0f;
    public float speed;
    // Use this for initialization
    void Start () {
        Renderer rend = GetComponent<Renderer>();
        material = rend.material;
        colors.Add(Color.red);
        colors.Add(Color.blue);
        colors.Add(Color.yellow);
        colors.Add(Color.magenta);
        colors.Add(Color.green);
        colors.Add(Color.cyan);


    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        speed = Time.deltaTime * (bpm / 60);
        if (colorcounter != colors.Count - 1)
        {
            if (material.GetColor("_MKGlowTexColor") == colors[colorcounter + 1])
            {
                colorcounter++;
                timer = 0;
            }
        }
        else
            if (material.GetColor("_MKGlowTexColor") == colors[0])
        {
            colorcounter = 0;
            timer = 0;
        }
        float lerp = Mathf.PingPong(timer * bpm / 60, 1.0f) / 1.0f;

        if (colorcounter != colors.Count - 1)
        {
            material.SetColor("_MKGlowTexColor", Color.Lerp(colors[colorcounter], colors[colorcounter + 1], timer * (bpm / 60)));
            material.SetColor("_MKGlowColor", Color.Lerp(colors[colorcounter], colors[colorcounter + 1], timer * (bpm / 60)));
        }
        else
        {
            material.SetColor("_MKGlowTexColor", Color.Lerp(colors[colorcounter], colors[0], timer * (bpm / 60)));
            material.SetColor("_MKGlowColor", Color.Lerp(colors[colorcounter], colors[0], timer * (bpm / 60)));
        }
    }
}
