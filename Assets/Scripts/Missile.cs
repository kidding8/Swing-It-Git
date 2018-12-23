using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour {

    private AuxManager aux;
    private EffectsManager EM;
    private Transform player;
    private Rigidbody2D rb;
    public float speed = 5f;
    public float rotateSpeed = 200f;
	void Start () {
        aux = AuxManager.instance;
        EM = EffectsManager.instance;
        player = aux.GetPlayer().transform;
        rb = GetComponent<Rigidbody2D>();
    }
	

	void FixedUpdate () {
        Vector2 direction = (Vector2)player.position - rb.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, transform.up).z;

        rb.angularVelocity = -rotateAmount * rotateSpeed;

        rb.velocity = transform.up * speed;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Player") || other.CompareTag("Enemy") || other.CompareTag("Hook") || other.CompareTag("Ground"))
        {
            EM.SetCoinPickUpParticles(transform.position);
            EM.CreateDisappearingCircle(transform.position);
            gameObject.SetActive(false);
        }
    }
}
