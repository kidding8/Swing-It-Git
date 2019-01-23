using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdScript : MonoBehaviour
{
    public float horizontalSpeed = 3f;
    public float verticalSpeed = 2f;
    public float amplitude = 1f;

    private Vector2 tempPos;
    void Start()
    {
        tempPos = transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        tempPos.x += horizontalSpeed;
        tempPos.y = Mathf.Sin(Time.realtimeSinceStartup * verticalSpeed) * amplitude;
        transform.position = tempPos;
    }
}
