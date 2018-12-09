using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRightScript: MonoBehaviour {

    // Use this for initialization
    public float speed = 3f;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(-transform.right * speed * Time.smoothDeltaTime);
    }
}
