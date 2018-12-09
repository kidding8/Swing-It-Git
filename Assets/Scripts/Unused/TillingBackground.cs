using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TillingBackground : MonoBehaviour {

    private AuxManager aux;
    public int offsetX = 2;

    public bool hasRightBuddy = false;
    public bool hasLeftBuddy = false;

    public bool reverseScale = false;
    private Camera cam;
    private float spriteWidth = 0f;

	// Use this for initialization
	void Start () {
        aux = AuxManager.instance;
        cam = aux.GetCamera();
        SpriteRenderer sRenderer = GetComponent<SpriteRenderer>();
        spriteWidth = sRenderer.sprite.bounds.size.x * transform.localScale.x;
	}
	
	// Update is called once per frame
	void Update () {

		if(hasLeftBuddy == false || hasRightBuddy == false)
        {
            float camHorizontalExtend = cam.orthographicSize * Screen.width / Screen.height;

            float edgeVisiblePositionRight = (transform.position.x + spriteWidth / 2) - camHorizontalExtend;
            float edgeVisiblePositionLeft = (transform.position.x - spriteWidth / 2) + camHorizontalExtend;

            if(cam.transform.position.x >= edgeVisiblePositionRight - offsetX && !hasRightBuddy)
            {
                MakeNewBuddy(1);
                hasRightBuddy = true;
            }else if(cam.transform.position.x <= edgeVisiblePositionLeft + offsetX && !hasLeftBuddy)
            {
                MakeNewBuddy(-1);
                hasLeftBuddy = true;
            }

        }
	}

    void MakeNewBuddy(int rightOrLeft)
    {
        Vector3 newPos = new Vector3(transform.position.x + spriteWidth * rightOrLeft, transform.position.y, transform.position.z);

        Transform newBuddy = (Transform)Instantiate(transform, newPos, transform.rotation);

        if(reverseScale == true)
        {
            newBuddy.localScale = new Vector3(newBuddy.localScale.x * -1, newBuddy.localScale.y, newBuddy.localScale.z);

        }

        newBuddy.parent = transform.parent;
        if(rightOrLeft > 0)
        {
            newBuddy.GetComponent<TillingBackground>().hasLeftBuddy = true;
        }
        else
        {
            newBuddy.GetComponent<TillingBackground>().hasRightBuddy = true;
        }
    }
}
