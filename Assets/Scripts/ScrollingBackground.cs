using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour {

    private AuxManager aux;

    public float parallaxSpeed;
    public bool useCamera = true;
    public float offsetBackground = 0.1f;
    public bool randomBackground = false;
    private Transform cam;
    private Transform[] layers;
    private float viewZone = 10;
    private int leftIndex;
    private int rightIndex;
    private float lastCameraX;
    private float backgroundSize;

	// Use this for initialization
	void Start () {
        aux = AuxManager.instance;

        cam = aux.GetCamera().transform;
        lastCameraX = cam.transform.position.x;
        layers = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            layers[i] = transform.GetChild(i);
            if (randomBackground)
                ChangeBackground(i);
        }
        leftIndex = 0;
        rightIndex = layers.Length - 1;

        backgroundSize = layers[0].GetComponent<SpriteRenderer>().sprite.bounds.size.x - offsetBackground;
    }
	
	// Update is called once per frame
	void Update () {

        
        if (useCamera)
        {
            float deltaX = cam.position.x - lastCameraX;
            transform.position += Vector3.right * (deltaX * parallaxSpeed);
            lastCameraX = cam.transform.position.x;
        }
        else
        {
            transform.position += Vector3.right * parallaxSpeed;
        }
        

        if (cam.position.x < (layers[leftIndex].transform.position.x + viewZone))
        {
            ScrollLeft();
        } else if (cam.position.x > (layers[rightIndex].transform.position.x - viewZone))
        {
            ScrollRight();
        }
    }

    private void ScrollLeft()
    {

        layers[rightIndex].position = new Vector3(layers[leftIndex].position.x - backgroundSize, layers[rightIndex].position.y , layers[rightIndex].position.z);
        leftIndex = rightIndex;
        rightIndex--;

        

        if (rightIndex < 0)
        {
            rightIndex = layers.Length - 1;
        }

        if (randomBackground)
            ChangeBackground(leftIndex);
    }
    private void ScrollRight()
    {
        
        layers[leftIndex].position = new Vector3(layers[rightIndex].position.x + backgroundSize, layers[leftIndex].position.y, layers[leftIndex].position.z);
        rightIndex = leftIndex;
        leftIndex++;

        

        if (leftIndex == layers.Length)
        {
            leftIndex = 0;
        }

        if (randomBackground)
            ChangeBackground(rightIndex);
    }

    private void ChangeBackground(int pos)
    {
        layers[pos].GetComponent<SpriteRenderer>().sprite = aux.GetBackgroundSprite();
    }
}
