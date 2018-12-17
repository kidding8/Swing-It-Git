using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRightScript: MonoBehaviour {

    // Use this for initialization
    public float speed = 3f;
    private EffectsManager EM;
	void Start () {
        EM = EffectsManager.instance;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Translate(-transform.right * speed * Time.smoothDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall")){
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
