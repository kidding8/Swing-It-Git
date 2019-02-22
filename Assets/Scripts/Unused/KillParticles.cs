using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillParticles : MonoBehaviour {

    private ParticleSystem pSystem;
	void Start () {
        pSystem = GetComponent<ParticleSystem>();
	}
	void Update () {
        if (pSystem)
        {
            if (!pSystem.IsAlive())
            {
                gameObject.SetActive(false);
            }
        }
            
	}
}
