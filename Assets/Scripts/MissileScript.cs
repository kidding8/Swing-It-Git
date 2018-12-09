using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileScript : MonoBehaviour {

    private AuxManager aux;
    private Transform player;
    private Rigidbody2D rb;
    public float speed = 5f;
    public float rotateSpeed = 200f;
	void Start () {
        aux = AuxManager.instance;
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
}
