using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundScript : MonoBehaviour {
    private AuxManager aux;
    private Transform player;
    // Use this for initialization
    void Start () {
        aux = AuxManager.instance;
        player = aux.GetPlayer().transform;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = new Vector3(player.transform.position.x, transform.position.y, transform.position.z);
	}
}
