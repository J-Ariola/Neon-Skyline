using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupColors : MonoBehaviour {

    public GameObject carBody;

    Material[] mats;

    public ParticleSystem boostTrail;

	// Use this for initialization
	void Start () {

        mats = carBody.GetComponent<MeshRenderer>().materials;

        for (int i = 0; i < mats.Length; i++)
        {
            if (mats[i].name.ToLower().Contains("glow"))
            {
                if (boostTrail != null)
                {
                    Debug.Log(boostTrail.GetComponent<Renderer>().material.name);
                    boostTrail.GetComponent<Renderer>().material.SetColor("_MKGlowTexColor", mats[i].GetColor("_MKGlowTexColor"));
                    boostTrail.GetComponent<Renderer>().material.SetColor("_MKGlowColor", mats[i].GetColor("_MKGlowColor"));
                    ColorPulse pulse = boostTrail.GetComponent<ColorPulse>();
                    pulse.colors.Clear();
                    pulse.colors.Add(mats[i].GetColor("_MKGlowColor"));
                }
                else
                {
                    Debug.Log("Car Color: " + mats[i].GetColor("_MKGlowColor"));
                    Debug.Log("Powerup Color: " + this.GetComponent<MeshRenderer>().material.GetColor("_MainColor"));
                    this.GetComponent<MeshRenderer>().material.SetColor("_MainColor", mats[i].GetColor("_MKGlowColor"));
                }
            }
        }

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
