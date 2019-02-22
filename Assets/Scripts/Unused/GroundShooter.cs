using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundShooter : MonoBehaviour
{
    private EffectsManager EM;
    private AuxManager aux;

    private ObjectPooler bulletPool;
    public float velocity = 3f;
    public float rotationSpeed;
    public float boostOnDeath = 10f;
    public float timeBetweenBullets = 3f;
    public bool shootBullets = false;

    private Rigidbody2D rb;
    private GameObject player;
    private Transform shooter;
    private Transform shootPoint;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        EM = EffectsManager.instance;
        aux = AuxManager.instance;
        bulletPool = aux.GetGroundBulletPool();
        player = aux.GetPlayer();
        shooter = transform.GetChild(0);
        shootPoint = shooter.GetChild(0);
    }
    void FixedUpdate()
    {
        rb.velocity = new Vector2(velocity, rb.velocity.y);
    }

    // Update is called once per frame
    void Update()
    {
        //RotateGunTowardsPlayer();
    }

    private void OnEnable()
    {
        if(shootBullets){
            StartCoroutine(ShootBulletGenerator());
        }
            
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void RotateGunTowardsPlayer()
    {
        Vector3 vectorToTarget = player.transform.position - shooter.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion qt = Quaternion.AngleAxis(angle, Vector3.forward);
        shooter.rotation = Quaternion.RotateTowards(shooter.rotation, qt, Time.deltaTime * rotationSpeed);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            Vector2 velocity = rb.velocity;
            rb.velocity = new Vector2(velocity.x, boostOnDeath);
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

    private void ShootBullet()
    {
        GameObject newMissile = bulletPool.GetPooledObject();
        newMissile.SetActive(true);
        //newMissile.transform.rotation = Quaternion.Euler(0, 0, -90);
        //Vector3 newPos;
       
        newMissile.transform.rotation = shooter.rotation;
        newMissile.transform.position = shootPoint.position;
    }

    IEnumerator ShootBulletGenerator()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenBullets);
            ShootBullet();
        }

    }
}
