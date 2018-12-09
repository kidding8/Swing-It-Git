using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour {

    private AuxManager aux;
    //private EffectsManager EM;
    private GameObject player;
    private void Start()
    {
        aux = AuxManager.instance;
        player = aux.GetPlayer();
        //EM = EffectsManager.instance;
    }
    void Update () {
        
            transform.position = player.transform.position;
	}
}
