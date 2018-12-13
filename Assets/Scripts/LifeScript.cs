using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeScript : MonoBehaviour {
    private GameManager GM;
    private EffectsManager EM;
    private bool isTriggered = false;
	// Use this for initialization
	void Start () {
        GM = GameManager.instance;
        EM = EffectsManager.instance;
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isTriggered)
        {
            GM.AddLife();
            isTriggered = true;
            EM.SetCoinPickUpParticles(transform.position);
            EM.CreateDisappearingCircle(transform.position);
            gameObject.SetActive(false);
        }
    }

    private void OnEnable()
    {
        isTriggered = false;
    }
}
