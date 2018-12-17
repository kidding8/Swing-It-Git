using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinScript : MonoBehaviour {

    private GameManager GM;
    private EffectsManager EM;
    private void Start()
    {
        GM = GameManager.instance;
        EM = EffectsManager.instance;
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GM.IncrementCoins(1);
            EM.SetCoinPickUpParticles(transform.position);
            EM.CreateDisappearingCircle(transform.position);
            gameObject.SetActive(false);
        }else if (other.CompareTag("Wall"))
        {
            gameObject.SetActive(false);
        }
    }
}
