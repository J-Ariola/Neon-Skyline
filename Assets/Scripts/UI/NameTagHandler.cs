using UnityEngine;
using System.Collections;

public class NameTagHandler : MonoBehaviour
{

    [Range(0, 5)]
    public float rotationSpeed = 0.25f;
    public GameObject portrait, nameText;
    public Texture2D image;
    public Material material;

    //player's ID number
    public int playerID;
    
    void Awake()
    {
        //From Selected Character Info script
        string name = PlayerPrefs.GetString("CharacterSelectedPlayer" + playerID);
        string imageName = PlayerPrefs.GetString("Player" + playerID + "Profile");

        if(playerID == 0)
        {
            name = "Racer";
            imageName = "GenericRacer";
        }
        
        nameText.GetComponent<TextMesh>().text = name;
        //Material quadMaterial = (Material)Resources.Load("CharacterPortraits/PortraitPlaceholder");
        //portrait.GetComponent<Renderer>().material = material;
        //Texture2D texture = Resources.Load("/Sprites/CharacterPortraits/" + name + ".png") as Texture2D;
        image = Resources.Load("Sprites/CharacterPortraits/" + imageName) as Texture2D;
        portrait.GetComponent<Renderer>().material.mainTexture = image;
    }

    void OnEnable()
    {
        CameraPreRender.onPreCull += MyPreCull;
    }

    void OnDisable()
    {
        CameraPreRender.onPreCull -= MyPreCull;
    }

    void MyPreCull()
    {
        //The tranform will face the camera
        Vector3 difference = Camera.current.transform.position - transform.position;
        transform.LookAt(transform.position - difference, Camera.current.transform.up);

        /*
        Vector3 targetDir = Camera.current.transform.position - transform.position;
        float step = rotationSpeed * Time.deltaTime;
        Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, step, 0.0f);
        Quaternion lookRot = Quaternion.LookRotation(newDir);
        lookRot.x = 0.0f;
        //lookRot.z = 0.0f;
        transform.rotation = lookRot;*/
    }
}
