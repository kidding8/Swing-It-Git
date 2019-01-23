using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallPlayerRadius : MonoBehaviour {

    private AuxManager aux;
    private EffectsManager EM;
    private Transform player;
    private void Start()
    {
        aux = AuxManager.instance;
        EM = EffectsManager.instance;
        player = aux.GetPlayer().transform;
    }

    private void Update()
    {
        transform.position = player.position;
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            //other.
            EM.GenerateText("Close Call 1000", other.transform.position);
        }
    }
}
