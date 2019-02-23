using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiscellaniousFunctions : MonoBehaviour
{
    private AuxManager aux;
    private bool alreadyStarted = false;
    [Header("Color")]
    [Space(3)]
    public bool changeColor = false;
    public bool isTrail = false;
    public ColorType currentColorType = ColorType.neutral;
    private Color currentColor;
    private SpriteRenderer sRenderer;
    private TrailRenderer trailRenderer;
    [Header("Destroy")]
    [Space(3)]
    public bool destroyWall = false;
    // Start is called before the first frame update
    void Start()
    {
        aux = AuxManager.instance;
        sRenderer = GetComponent<SpriteRenderer>();
        trailRenderer = GetComponent<TrailRenderer>();
        alreadyStarted = true;
        ChangeColor();
        ChangeTrailColor();
    }

    // Update is called once per frame
    /*void Update()
    {
        if (destroyWall && transform.position.x <= aux.wall.transform.position.x)
        {
            OnDeath();
        }
    }*/

    private void OnEnable()
    {
        if (alreadyStarted && changeColor)
        {
            if (!aux.IsColor(currentColor, currentColorType))
            {
                ChangeColor();
            }

            ChangeTrailColor();

        }
    }

    private void ChangeColor()
    {
        if (sRenderer != null && changeColor && !isTrail)
        {
            currentColor = aux.GetColor(currentColorType);
            sRenderer.color = currentColor;
        }
    }
    private void ChangeTrailColor()
    {
        if (trailRenderer != null && changeColor && isTrail)
        {
            currentColor = aux.GetColor(currentColorType);
            trailRenderer.startColor = currentColor;
            trailRenderer.endColor = currentColor;
        }
    }

    private void OnDeath()
    {
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Wall") && destroyWall)
        {
            OnDeath();
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Wall") && destroyWall)
        {
            OnDeath();
        }
    }
}
