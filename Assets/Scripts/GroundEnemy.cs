using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundEnemy : MonoBehaviour {
    private Rigidbody2D rb;
    public float velocity = 2f;
    public float boost;
    private EffectsManager EM;
    private bool isLeft = false;
	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody2D>();
        EM = EffectsManager.instance;
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        rb.velocity = new Vector2(isLeft ? -velocity : velocity, rb.velocity.y);
	}

    bool GetRandomBool()
    {
        int i = Random.Range(0, 2);
        return i == 0;
    }
    private void OnEnable()
    {
        isLeft = GetRandomBool();
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            Vector2 velocity = rb.velocity;
            rb.velocity = new Vector2(velocity.x, boost);
            EM.SetCoinPickUpParticles(transform.position);
            EM.CreateDisappearingCircle(transform.position);
            //isLeft = GetRandomBool();
            gameObject.SetActive(false);
        }
    }

    /*private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            Vector2 velocity = rb.velocity;
            rb.velocity = new Vector2(velocity.x, boost);
            EM.SetCoinPickUpParticles(transform.position);
            EM.CreateDisappearingCircle(transform.position);
            gameObject.SetActive(false);
        }
    }*/
}
