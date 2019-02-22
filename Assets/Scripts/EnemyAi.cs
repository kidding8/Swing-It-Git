using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    private AuxManager aux;
    private EffectsManager EM;
    private GameManager GM;
    public float timeToChangeDirection = 3;
    public float wanderingSpeed = 3f;
    public float radiusCircle = 10f;
    private float timerToDirection;
    private Rigidbody2D rb;
    private Vector2 newPos;
    private Vector2 currentPos;
    public bool isWandering = true;
    public bool rotate = true;
    private Transform playerPos;
    private SpriteRenderer sRenderer;
    private float previousPosX;
    // Start is called before the first frame update
    void Start()
    {
        aux = AuxManager.instance;
        EM = EffectsManager.instance;
        GM = GameManager.instance;
        rb = GetComponent<Rigidbody2D>();
        sRenderer = GetComponent<SpriteRenderer>();
        // ChangeDirection();
        newPos = GetPosInsideCircle();
        timerToDirection = timeToChangeDirection;
        currentPos = transform.position;
        previousPosX = transform.position.x;
        playerPos = aux.player.transform;
    }

    // Update is called once per frame
    void Update()
    {



        //timerToDirection -= Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, isWandering? newPos : (Vector2)playerPos.position, (isWandering ? wanderingSpeed : wanderingSpeed * 2) * Time.deltaTime);
        if ((Vector2)transform.position == newPos && isWandering)
        {
            newPos = GetPosInsideCircle();
        }

        if (Vector3.Distance(transform.position, playerPos.position) <= 40)
        {
           // newPos = playerPos.position;
            isWandering = false;
        }
        else if (!isWandering)
        {
            isWandering = true;
            newPos = GetPosInsideCircle();
        }

        if(rotate)
            transform.localScale = new Vector3(transform.position.x > previousPosX ? 1 : -1, 1, 1);
       /* if (transform.position.x > previousPosX)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            transform.localScale = Vector3.right;
        }*/
        previousPosX = transform.position.x;
        /*if (timerToDirection <= 0)
        {
            //ChangeDirection();
            newPos = GetPosInsideCircle();

            timerToDirection = timeToChangeDirection;
        }*/


        /* if (timerToDirection <= 0)
         {
             ChangeDirection();
             timerToDirection = timeToChangeDirection;
         }*/

        //rigidbody.velocity = transform.up * 2;
    }

    private void OnEnable()
    {
        currentPos = transform.position;
    }

    private void ChangeDirection()
    {
        float angle = Random.Range(0f, 360f);
        Quaternion quat = Quaternion.AngleAxis(angle, Vector3.forward);
        Vector3 newUp = quat * Vector3.up;
        newUp.z = 0;
        newUp.Normalize();
        transform.up = newUp;
        timeToChangeDirection = 1.5f;
    }

    private Vector2 GetPosInsideCircle()
    {
        return currentPos + Random.insideUnitCircle * radiusCircle;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Destroyer"))
        {
            OnDeath();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player")){
            float height = other.contacts[0].point.y - sRenderer.bounds.center.y;

            if(height > 0)
            {
                OnDeath();
                PlayerManager.instance.Jump(PlayerManager.instance.jumpForce);
                GM.AddCombo(50);
                PlayerManager.instance.ResetAirJump();
            }
            else
            {
                EM.CreateEnemyEffects(transform.position);
                OnDeath();
                GM.RemoveLife();
                /*if(PlayerManager.instance.playerState == States.STATE_ON_FIRE)
                {
                    OnDeath();
                }*/
            }

        }/*else if (other.gameObject.CompareTag("Enemy"))
        {
            EM.CreateEnemyEffects(transform.position);
        }*/
    }

    private void OnDeath()
    {
        EM.CreateEnemyEffects(transform.position);
        gameObject.SetActive(false);
    }
}
