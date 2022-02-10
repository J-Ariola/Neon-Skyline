using Kino;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Assets.MultiAudioListener;

public class HealthController : MonoBehaviour
{

    //holds the car body object
    public GameObject carBody;
    //holds the correct shader for the car body
    public Shader carBodyShader;

    public Material[] carMaterials;
    public Material glowmat;

    //Pulse Variables
    public float bpm = 120.0f;
    float speed;
    public float timer = 0.0f;

    //ignore for now
    //Color carBodyColor;
    //float carBodyAlpha;

    //holds player's current health and max health(max health will be 1)
    [Header("Health")]
    public float currentHealth;
    float maxHealth = 1;

    [Header("Collision")]
    public float collisionTimerMax=2;
    private float collisionTimer;
    public bool canCollide;
    //multiplicative value that will make the lines on the shader more vibrant with max health
    public float glowTexValue;

    public bool showCollisionLog;
    HoverCarController hoverCarController;

    MultiAudioSource staticSfx;
    MultiAudioSource collisionSfx;

    LocalAudioManager audioManager;

    // Use this for initialization
    void Start()
    {

        //copy array of materials to temp array so we can access the correct materials
        carMaterials = carBody.GetComponent<Renderer>().materials;

        //assign the carbody shader
        for (int i = 0; i < carMaterials.Length; i++)
        {
            if (carMaterials[i].name.ToLower().Contains("glow"))
            {
                carBodyShader = carMaterials[i].shader;
            }
        }

        //carBodyColor = carBody.GetComponent<Renderer>().material.color;

        currentHealth = maxHealth;

        //only for objects with glow shaders
        //loop through car materials array and find the glow shader
        for (int i = 0; i < carMaterials.Length; i++)
        {
            if (carMaterials[i].name.ToLower().Contains("glow"))
            {
                if (carBodyShader.name.ToLower().Contains("glow"))
                {
                    carMaterials[i].SetColor("_MKGlowColor", carMaterials[i].GetColor("_MKGlowTexColor"));
                    //set the glow texture to the current health(the lines on the car will fade as health goes down)
                    carMaterials[i].SetFloat("_MKGlowTexStrength", currentHealth * glowTexValue);
                    //set the glow power to the current health(glow will fade as health goes down)
                    carMaterials[i].SetFloat("_MKGlowPower", currentHealth);
                    //Created a material specifically for glow material, makes it easier to reference. - Erik
                    glowmat = carMaterials[i];
                }
            }
        }

        carBody.GetComponent<Renderer>().materials = carMaterials;
        hoverCarController = GetComponent<HoverCarController>();

        audioManager = this.GetComponent<LocalAudioManager>();

        for (int i = 0; i < audioManager.audioSources.Count; i++)
        {
            if (audioManager.audioSources[i].AudioClip.name.ToLower().Contains("static"))
            {
                staticSfx = audioManager.audioSources[i];
                staticSfx.VolumeRolloff = AudioRolloffMode.Linear;
                staticSfx.MaxDistance = 50;
                staticSfx.Volume = .5f;
            }

            if (audioManager.audioSources[i].AudioClip.name.ToLower().Equals("collision"))
            {
                collisionSfx = audioManager.audioSources[i];
                collisionSfx.VolumeRolloff = AudioRolloffMode.Linear;
                collisionSfx.MaxDistance = 50;
                collisionSfx.Volume = .3f;
            }
        }
        }

    void FixedUpdate()
    {

        //test: decrements health over time
        //if (Input.GetKey(KeyCode.Space))
        //{
        //    if (currentHealth > 0)
        //    {
        //        currentHealth -= .005f;
        //        //only for objects with glow shaders
        //        //loop through car materials array and find the glow shader
        //        //Moved this into when Space is pressed, doesn't always need to be run. -Erik
        //        SetCarGlow();
        //    }

        //}
        ColorPulse();

        //carBody.GetComponent<Renderer>().material.color = new Color(carBodyAlpha, carBodyAlpha, carBodyAlpha, carBodyAlpha);
        if (currentHealth <= 0f)
        {
            currentHealth = 0;
            hoverCarController.isAlive = false;
            glowmat.SetFloat("_MKGlowPower", Mathf.Lerp(currentHealth + 1, 0, hoverCarController.respawnTimerMax - hoverCarController.respawnTimer));
            glowmat.SetFloat("_MKGlowTexStrength", Mathf.Lerp(currentHealth + 1, 0, hoverCarController.respawnTimerMax - hoverCarController.respawnTimer));
        }



        if (!canCollide)
        {
            float s, h, i;
            if (collisionTimer < collisionTimerMax && hoverCarController.driver == HoverCarController.driverEnum.Player)
            {
                collisionTimer += Time.deltaTime;
                hoverCarController.myCamera.GetComponent<AnalogGlitch>().scanLineJitter = Mathf.Lerp(0, ((1 - currentHealth) / 5), collisionTimerMax-collisionTimer);
                s = hoverCarController.myCamera.GetComponent<AnalogGlitch>().scanLineJitter;
                hoverCarController.myCamera.GetComponent<AnalogGlitch>().horizontalShake = Mathf.Lerp( 0, ((1 - currentHealth) / 10), collisionTimerMax - collisionTimer);
                h = hoverCarController.myCamera.GetComponent<AnalogGlitch>().horizontalShake;
                hoverCarController.myCamera.GetComponent<DigitalGlitch>().intensity = Mathf.Lerp( 0, ((1 - currentHealth) / 15), collisionTimerMax - collisionTimer);
                i = hoverCarController.myCamera.GetComponent<DigitalGlitch>().intensity;
            }
            else
            {
                canCollide = true;
                collisionTimer = 0;
            }
        }
    }

    //decreases health by a specific value
    public void DecrementHealth(float value)
    {
        if (currentHealth > 0)
        {
            currentHealth -= value;
            if (!hoverCarController.powerupAnimator.GetBool("isShielding"))
            {
                collisionSfx.Pitch = Random.Range(1, 3);
                collisionSfx.Play();
                setCameraGlitch();
            }
            SetCarGlow();
            
        }
        if (showCollisionLog)
        {
            Debug.Log(name + "takes " + value + "damage");
        }
    }

    public void ResetHealth()
    {
        this.currentHealth = 0;
    }

    public void setCameraGlitch()
    {
        if (hoverCarController.driver == HoverCarController.driverEnum.Player)
        {
            hoverCarController.myCamera.GetComponent<AnalogGlitch>().scanLineJitter = (1 - currentHealth) / 5;
            hoverCarController.myCamera.GetComponent<AnalogGlitch>().horizontalShake = (1 - currentHealth) / 10;
            hoverCarController.myCamera.GetComponent<DigitalGlitch>().intensity = (1 - currentHealth) / 15;
            if (!hoverCarController.powerupAnimator.GetBool("isShielding"))
            {
                staticSfx.Pitch = Random.Range(1, 3);
                staticSfx.Play();
            }
        }
    }

    //increases health by a specific value
    public void IncrementHealth(float value)
    {
        currentHealth += value;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        SetCarGlow();
    }

    void SetCarGlow()
    {
        for (int i = 0; i < carMaterials.Length; i++)
        {
            if (carMaterials[i].name.ToLower().Contains("glow"))
            {
                if (carBodyShader.name.ToLower().Contains("glow"))
                {
                    carMaterials[i].SetColor("_MKGlowColor", carMaterials[i].GetColor("_MKGlowTexColor"));
                    //set the glow texture to the current health(the lines on the car will fade as health goes down)
                    carMaterials[i].SetFloat("_MKGlowTexStrength", currentHealth * glowTexValue);
                    //set the glow power to the current health(glow will fade as health goes down)
                    carMaterials[i].SetFloat("_MKGlowPower", currentHealth);
                }
            }
        }
        carBody.GetComponent<Renderer>().materials = carMaterials;
    }

    void ColorPulse()
    {
        //***********************************************************
        //Color Pulse section
        //***********************************************************

        //increase a timer from 0 and Calculate speed (not used)
        timer += Time.deltaTime;
        speed = Time.deltaTime * (bpm / 60);
        //check if the GlowPower is at 0, and if it is, reset the timer to 0
        if (glowmat.GetFloat("_MKGlowPower") == 0.0f)
        {
            timer = 0;

        }
        //Don't believe this is used-Ping pong makes it glow more and less
        float lerp = Mathf.PingPong(timer * bpm / 60, 1.0f) / 1.0f;

        //Lerp the glow power based on timer
        glowmat.SetFloat("_MKGlowPower", Mathf.Lerp(currentHealth+1, 0, timer * (bpm / 60)));


        //works for standard shaders
        //if(carBodyAlpha > 0)
        //{
        //    carBodyAlpha -= .01f;
        //}
    }

    void OnCollisionEnter(Collision collision)
    {
        //Collider collider = collision.contacts[0].thisCollider;
        //Debug.Log(collider.name + " collided with " + collision.contacts[0].otherCollider.name);
        foreach (ContactPoint contact in collision.contacts)
        {
            float relativeVelocity = collision.relativeVelocity.magnitude;
            if (contact.otherCollider.tag.Contains("NoRay") && canCollide)
            {
                if (showCollisionLog)
                    Debug.Log(contact.thisCollider.transform.root.name + " collided with " + contact.otherCollider.name + " at a velocity of " + relativeVelocity);
                DecrementHealth(relativeVelocity / hoverCarController.damageDivisor);
                canCollide = false;
            }
            if (contact.otherCollider.transform.root.tag.Contains("Player") && canCollide)
            {
                HoverCarController otherHoverController = contact.otherCollider.transform.root.gameObject.GetComponent<HoverCarController>();
                if (showCollisionLog)
                    Debug.Log(contact.thisCollider.transform.root.name + " collided with " + contact.otherCollider.transform.root.name + " at a velocity of " + relativeVelocity);
                if (hoverCarController.UsingPowerUp)
                {
                    if (hoverCarController.storedPowerUp != HoverCarController.powerUpEnum.Attack)
                        DecrementHealth(relativeVelocity / hoverCarController.damageDivisor);
                }
                else if (otherHoverController.UsingPowerUp)
                {
                    if (otherHoverController.storedPowerUp == HoverCarController.powerUpEnum.Attack)
                    {
                        DecrementHealth(relativeVelocity / hoverCarController.amplifiedDamageDivisor);
                    }
                }
                else
                    DecrementHealth(relativeVelocity / hoverCarController.damageDivisor);
                canCollide = false;
            }
        }
    }

    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }
}
