using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LifeScript : MonoBehaviour {
    private GameManager GM;
	// Use this for initialization
	void Start () {
        GM = GameManager.instance;
	}

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GM.AddLife();
        }
    }
}
