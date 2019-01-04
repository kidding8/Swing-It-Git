using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeScript : MonoBehaviour {

    private GameManager GM;
    private PlayerManager PM;
    private EffectsManager EM;
	// Use this for initialization
	void Start () {
        PM = PlayerManager.instance;
        GM = GameManager.instance;
        EM = EffectsManager.instance;
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && PM.CanCollect())
        {
            GM.AddLife();
            EM.SetCoinPickUpParticles(transform.position);
            EM.CreateDisappearingCircle(transform.position);
            gameObject.SetActive(false);
        }
        else if(other.CompareTag("Wall"))
        {
            onDeath();
        }
    }

    private void onDeath()
    {
        EM.SetCoinPickUpParticles(transform.position);
        EM.CreateDisappearingCircle(transform.position);
        gameObject.SetActive(false);
    }

}
