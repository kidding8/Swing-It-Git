using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleShooter : MonoBehaviour
{
    private EffectsManager EM;
    private PlayerManager PM;
    private AuxManager aux;

    public bool shootBullets = true;
    public float minDistanceToPlayer = 30f;
    private ObjectPooler bulletPool;
    public float timebetweenBullets = 4f;
    public float rotationSpeed = 2f;

    private float distanceToPlayer;
    private GameObject player;
    void Start()
    {
        EM = EffectsManager.instance;
        PM = PlayerManager.instance;
        aux = AuxManager.instance;
        player = aux.GetPlayer();
        bulletPool = aux.GetGroundBulletPool();
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(player.transform.position, transform.position);
        Vector3 vectorToTarget = player.transform.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, q, rotationSpeed* Time.deltaTime);
        Debug.DrawRay(transform.position, vectorToTarget.normalized * minDistanceToPlayer);
    }

    private void OnEnable()
    {
        if (shootBullets)
        {
            StartCoroutine(ShootBulletGenerator());
        }
    }
    private void onDeath()
    {
        EM.SetCoinPickUpParticles(transform.position);
        EM.CreateDisappearingCircle(transform.position);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            onDeath();
            //isLeft = GetRandomBool();

        }
        else if (other.CompareTag("Wall"))
        {
            onDeath();
        }
    }

    private void ShootBullet()
    {
        GameObject newMissile = bulletPool.GetPooledObject();
        newMissile.SetActive(true);
        //newMissile.transform.rotation = Quaternion.Euler(0, 0, -90);
        //Vector3 newPos;

        newMissile.transform.rotation = transform.rotation;
        newMissile.transform.position = transform.position;
    }

    IEnumerator ShootBulletGenerator()
    {
        while (true)
        {
            yield return new WaitForSeconds(timebetweenBullets);
            if (distanceToPlayer > minDistanceToPlayer)
                ShootBullet();
            else
                Debug.Log("Too Close to shoot");
        }

    }
}
