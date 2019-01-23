using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudScript : MonoBehaviour {

    private AuxManager aux;
    private Camera cam;
    private SpriteRenderer sRenderer;
    public float minSpeed;
    public float maxSpeed;

    //Set these variables to the lowest and highest y values you want clouds to spawn at.
    //For Example, I have these set to 1 and 4
    public float minY;
    public float maxY;

    public float minSize;
    public float maxSize;

    public Sprite[] cloudSprites;
    //Set this variable to how far off screen you want the cloud to spawn, and how far off the screen you want the cloud to be for it to despawn. You probably want this value to be greater than or equal to half the width of your cloud.
    //For Example, I have this set to 4, which should be more than enough for any cloud.
    public float buffer = 4f;

    float speed;
    float camWidth;

    

    void Start()
    {
        aux = AuxManager.instance;
        cam = aux.GetCamera();
        sRenderer = GetComponent<SpriteRenderer>();
        //Set camWidth. Will be used later to check whether or not cloud is off screen.
        camWidth = cam.orthographicSize * cam.aspect;

        //Set Cloud Movement Speed, and Position to random values within range defined above
        
    }

    // Update is called once per frame
    void Update()
    {
        //Translates the cloud to the right at the speed that is selected
        transform.Translate(-speed * Time.deltaTime, 0, 0);
        //If cloud is off Screen, Destroy it.
        
       /* if (transform.position.x - buffer > camWidth)
        {
            gameObject.SetActive(false);
        }*/
        if(transform.position.x <= aux.wall.transform.position.x)
        {
            gameObject.SetActive(false);
        }
    }

    public void Reset(Vector3 pos)
    {
        speed = Random.Range(minSpeed, maxSpeed);
        transform.position = new Vector3(pos.x + buffer, Random.Range(minY, maxY), transform.position.z);
        float size = GetRandomSize();
        transform.localScale = new Vector3(size, size, size);
        //if(GM != null) 
        //GetComponent<SpriteRenderer>().sprite = GM.GetRandomSprite();
    }

   

    private float GetRandomSize()
    {
        return Random.Range(minSize, maxSize);
    }
}