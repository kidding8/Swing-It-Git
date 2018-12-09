using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinnerScript : MonoBehaviour {

    public float rotatingSpeed1 = 30f;
    public float rotatingSpeed2 = 40f;
    private Transform spike1;
    private Transform spike2;

	// Use this for initialization
	void Start () {
        spike1 = transform.GetChild(0).transform;
        spike2 = transform.GetChild(1).transform;

    }
	
	// Update is called once per frame
	void Update () {
        spike1.transform.Rotate(new Vector3(0, 0, rotatingSpeed1 * Time.deltaTime));
        spike2.transform.Rotate(new Vector3(0, 0, rotatingSpeed2 * Time.deltaTime));
    }
}
