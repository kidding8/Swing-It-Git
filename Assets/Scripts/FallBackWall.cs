using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallBackWall : MonoBehaviour {
    private AuxManager aux;
    private GameObject player;
    public float offset = 45f;

	// Use this for initialization
	void Start () {
        aux = AuxManager.instance;
        player = aux.GetPlayer();
    }
	
	// Update is called once per frame
	void Update () {
        float pos = player.transform.position.x - offset;
	    if(pos > transform.position.x)
            transform.position = new Vector3(pos, transform.position.y, transform.position.z);
        
	}
}
