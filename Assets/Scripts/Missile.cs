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
    private CameraFollow camFollow;
    private float maxDistanceDetectPlayer = 20f;
    private bool isAlreadyATarget = false;
	void Start () {
        aux = AuxManager.instance;
        EM = EffectsManager.instance;
        player = aux.GetPlayer().transform;
        rb = GetComponent<Rigidbody2D>();
        camFollow = aux.GetCamera().GetComponent<CameraFollow>();
    }
	

	void FixedUpdate () {

        SlowRotateTowardsPlayer();
        rb.velocity = -transform.right * speed;
    }

    private void Update()
    {
        if(Vector3.Distance(transform.position, player.position) < maxDistanceDetectPlayer)
        {
            if (!isAlreadyATarget)
            {
                camFollow.AddTarget(transform);
                isAlreadyATarget = true;
            }
        }
        else if(isAlreadyATarget)
        {
            RemoveCamTarget();
        }    
    }



    private void SlowRotateTowardsPlayer()
    {
        Vector2 direction = (Vector2)player.position - rb.position;
        direction.Normalize();
        float rotateAmount = Vector3.Cross(direction, -transform.right).z;

        rb.angularVelocity = -rotateAmount * rotateSpeed;
    }

    private void FastRotateTowardsPlayer()
    {
       
        Vector2 distance = (Vector2)player.position - rb.position;
        var angle = Mathf.Atan2(distance.y, distance.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Slerp(transform.rotation, (Quaternion.AngleAxis(angle, Vector3.forward)), rotateSpeed * Time.deltaTime);
    }

    private void RemoveCamTarget()
    {
        camFollow.RemoveTarget(transform);
        isAlreadyATarget = false;
    }

    private void OnDeath()
    {
        RemoveCamTarget();
        /*
        EM.SetCoinPickUpParticles(transform.position);
        EM.CreateDisappearingCircle(transform.position);
        EM.GenerateText("Missile 100", transform.position);*/
        GameManager.instance.AddCombo(50);
        EM.CreateEnemyEffects(transform.position);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if(other.CompareTag("Player") || other.CompareTag("Enemy") || other.CompareTag("Grabber") || other.CompareTag("Rocks") || other.CompareTag("Skull"))
        {

            OnDeath();
        }
    }
}
