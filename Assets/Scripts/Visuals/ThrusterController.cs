using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrusterController : MonoBehaviour {

    public HealthController healthController;
    public HoverCarController hovercontroller;
    public List<GameObject> thrusters;
	// Use this for initialization
	void Start () {
        
        if(healthController==null)
        healthController = GetComponent<HealthController>();
        if(hovercontroller==null)
        hovercontroller = GetComponent<HoverCarController>();
        if (thrusters == null)
        {
            thrusters = new List<GameObject>();
            foreach (Transform g in transform.Find("CarFrame"))
            {
                if (g.name.Contains("Thruster"))
                    thrusters.Add(g.gameObject);
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void FixedUpdate()
    {
        HandleThrusters();
    }
    public void HandleThrusters()
    {
        for(int i =0;i<thrusters.Count;i++)
        {
            Material m = thrusters[i].GetComponent<MeshRenderer>().material;

            m.SetFloat("_MKGlowPower", Mathf.Lerp(healthController.currentHealth + 3, 0, healthController.timer * (healthController.bpm / 60)));
            m.SetFloat("_MKGlowTexStrength", Mathf.Lerp(healthController.currentHealth+3, 0, healthController.timer * (healthController.bpm / 60)));
            //float x = (hovercontroller.currentSpeed / Time.deltaTime)/100;
            //float y = m.GetTextureOffset("_MKGlowTex").y;
            //m.SetTextureOffset("_MKGlowTex", new Vector2((m.GetTextureOffset("_MKGlowTex").x+5.0f), 0));
            //Debug.Log(m.GetTextureOffset("_MKGlowTex"));
        }
        //Lerp the glow power based on timer
    }
    private void LateUpdate()
    {
        for (int i = 0; i < thrusters.Count; i++)
        {
            Material m = thrusters[i].GetComponent<MeshRenderer>().material;

            //m.SetFloat("_MKGlowPower", Mathf.Lerp(healthController.currentHealth + 2, 0, healthController.timer * (healthController.bpm / 60)));
            //float x = (hovercontroller.currentSpeed / Time.deltaTime)/100;
            //float y = m.GetTextureOffset("_MKGlowTex").y;

            m.SetTextureOffset("_MKGlowTex", new Vector2((m.GetTextureOffset("_MKGlowTex").x + .030f+(hovercontroller.currentSpeed/100)), 0));
            //Debug.Log(m.GetTextureOffset("_MKGlowTex"));
        }
    }
}
