using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : MonoBehaviour {

    private EffectsManager EM;
    private PlayerManager PM;
    private Rigidbody2D rb;
    public float velocity = 2f;
    public float bouncerBoost = 16f;
    public float loseBoost = 0.7f;
    public float bombBoost = 100f;
    public bool isBouncer = true;
    public bool isBomber = false;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        rb.velocity = new Vector2(velocity, rb.velocity.y);
	}


    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            if(PM.IsState(States.STATE_NORMAL))
            if (isBouncer)
            {
                PM.JumpUpwards(bouncerBoost);
            }
            else if(isBomber)
            {
                PM.AddImpulsiveForce(new Vector3(0.5f, 0.8f, 0f), 300f);
            }
            else
            {
                PM.JumpUpwards(5f);
                PM.LoseMomentum(loseBoost);
            }
            onDeath();
            //isLeft = GetRandomBool();

        }
        else if (other.gameObject.tag == "Wall")
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
