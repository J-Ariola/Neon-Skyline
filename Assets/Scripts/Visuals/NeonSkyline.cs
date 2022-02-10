using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeonSkyline : MonoBehaviour {

    public Material materialExt,materialInt;
    public List<Color> colors;
    public float bpm = 120.0f;
    float timer = 0.0f;
    int colorext;
    int colorint;
    public Color targetColorExt,targetColorInt;
    // Use this for initialization
    void Start () {
        colors.Add(Color.red);
        colors.Add(Color.blue);
        colors.Add(Color.yellow);
        colors.Add(Color.magenta);
        colors.Add(Color.green);
        colors.Add(Color.cyan);
        colors.Add(Color.white);
        colors.Add(Color.black);
        //materialExt = transform.Find("Exterior").GetComponent<MeshRenderer>().materials[0];
        //materialInt = transform.Find("Interior").GetComponent<MeshRenderer>().materials[0];
        colorext = Mathf.RoundToInt(Random.Range(0, 7));
        colorint = Mathf.RoundToInt(Random.Range(0, 7));

        //materialExt.SetColor("_MKGlowTexColor",colors[colorext]);
        //materialInt.SetColor("_MKGlowColor", colors[colorint]);
        targetColorExt = colors[colorext + 1];
        targetColorInt = colors[colorint + 1];
        foreach (Transform g in transform)
        {
            foreach (Material m in g.Find("Exterior").GetComponent<MeshRenderer>().materials)
                if (m.name.Contains("Building"))
                {
                    m.SetColor("_MKGlowTexColor", colors[colorext]);
                }
            foreach (Material m in g.Find("Interior").GetComponent<MeshRenderer>().materials)
                if (m.name.Contains("Building"))
                { m.SetColor("_MKGlowColor", colors[colorint]); }
            
            
        }
        materialExt = transform.Find("Building1").Find("Exterior").GetComponent<MeshRenderer>().material;
        materialInt = transform.Find("Building1").Find("Interior").GetComponent<MeshRenderer>().material;
    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;

        
        if (materialExt.GetColor("_MKGlowTexColor") == targetColorExt)
        {
            if (colorext != colors.Count - 1)
            {
                targetColorExt = colors[colorext + 1];
                colorext++;
                if (colorint != colors.Count - 1)
                {
                    targetColorInt = colors[colorint + 1];
                    colorint++;
                }
                else
                {
                    targetColorInt = colors[0];
                    colorint = 0;
                }
                timer = 0;
            }
            else
            {
                colorext = 0;
                targetColorExt = colors[colorext];
                if (colorint != colors.Count - 1)
                {
                    targetColorInt = colors[colorint + 1];
                    colorint++;
                }
                else
                {
                    targetColorInt = colors[0];
                    colorint = 0;
                }
                timer = 0;
            }
        }
        //}
        //else
        //    if (materialExt.GetColor("_MKGlowTexColor") == targetColorExt)
        //{
        //    targetColorExt = colors[0];
        //    colorext = 0;
        //    if (colorint != colors.Count - 1)
        //    {
        //        targetColorInt = colors[colorint + 1];
        //        colorint++;
        //    }
        //    else
        //    {
        //        targetColorInt = colors[0];
        //        colorint = 0;
        //    }
        //    timer = 0;
        //}

        //    materialExt.SetColor("_MKGlowTexColor", Color.Lerp(colors[colorext], targetColorExt, timer * (bpm / 60)));
        //    materialExt.SetColor("_MKGlowColor", Color.Lerp(colors[colorint], targetColorInt, timer * (bpm / 60)));
        foreach (Transform g in transform)
        {
            foreach (Material m in g.Find("Exterior").GetComponent<MeshRenderer>().materials)
                if (m.name.Contains("Building"))
                {
                    m.SetColor("_MKGlowTexColor", Color.Lerp(m.GetColor("_MKGlowTexColor"), targetColorExt, timer * (bpm / 60)));
                    m.SetColor("_MKGlowColor", Color.Lerp(m.GetColor("_MKGlowColor"), targetColorExt, timer * (bpm / 60)));
                    m.SetTextureOffset("_MKGlowTex", new Vector2(0, m.GetTextureOffset("_MKGlowTex").y + .01f));
                }
            foreach (Material m in g.Find("Interior").GetComponent<MeshRenderer>().materials)
                if (m.name.Contains("Building"))
                {
                    m.SetColor("_MKGlowTexColor", Color.Lerp(m.GetColor("_MKGlowTexColor"), targetColorInt, timer * (bpm / 60)));
                    m.SetColor("_MKGlowColor", Color.Lerp(m.GetColor("_MKGlowColor"), targetColorInt, timer * (bpm / 60)));
                    m.SetTextureOffset("_MKGlowTex", new Vector2(m.GetTextureOffset("_MKGlowTex").x + .01f, 0));
                }

        }
    }
}
