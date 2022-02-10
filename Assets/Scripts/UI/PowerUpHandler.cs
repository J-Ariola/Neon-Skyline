using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpHandler : MonoBehaviour {

    //public enum powerUpEnum;
    //public powerUpEnum powerUp;

    public Image powerUpImage;

    public Texture2D healthRestoration, boost, shield, attack;

    [Range(0.5f, 25.0f)]
    [Tooltip("How fast the power up will complete animation in seconds")]
    public float fillRate;

    public HoverCarController hoverCarController;

    private Sprite healthRestorationSprite, boostSprite, shieldSprite, attackSprite;
    private float fillAmt;
    bool isStoringPowerUp;

	// Use this for initialization
	void Start () {
        hoverCarController = transform.root.GetComponent<HoverCarController>();
        powerUpImage = transform.Find("PowerUpImage").GetComponent<Image>();

        healthRestorationSprite = Sprite.Create(healthRestoration, new Rect(0, 0, healthRestoration.width, healthRestoration.height), new Vector2(0.5f, 0.5f));
        boostSprite = Sprite.Create(boost, new Rect(0, 0, boost.width, boost.height), new Vector2(0.5f, 0.5f));
        shieldSprite = Sprite.Create(shield, new Rect(0, 0, shield.width, shield.height), new Vector2(0.5f, 0.5f));
        attackSprite = Sprite.Create(attack, new Rect(0, 0, attack.width, attack.height), new Vector2(0.5f, 0.5f));
    }
	
	// Update is called once per frame
	void Update () {
        isStoringPowerUp = hoverCarController.hasPowerUp;
        SwitchPowerUp();
        ShowPowerUp();
	}

    void SwitchPowerUp()
    {
        switch (hoverCarController.storedPowerUp)
        {
            case HoverCarController.powerUpEnum.Restoration:
                powerUpImage.sprite = healthRestorationSprite;
                break;

            case HoverCarController.powerUpEnum.Boost:
                powerUpImage.sprite = boostSprite;
                break;

            case HoverCarController.powerUpEnum.Defense:
                powerUpImage.sprite = shieldSprite;
                break;

            case HoverCarController.powerUpEnum.Attack:
                powerUpImage.sprite = attackSprite;
                break;
        }
    }

    void ShowPowerUp()
    {
        if (isStoringPowerUp)
        {
            fillAmt += Time.deltaTime / fillRate;
        }
        else
        {
            fillAmt -= Time.deltaTime;
        }
        fillAmt = Mathf.Clamp(fillAmt, 0, 1);
        powerUpImage.fillAmount = fillAmt;
    }

    public void ResetPowerUpAnim()
    {
        fillAmt = 0;
    }
}
