using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingSpinner : MonoBehaviour
{
    private AuxManager aux;
    private GameObject spinner;
    private bool canSpawnIt = true;
    private float pointA = 5.35f;
    public float speed = 20f;
    private bool goToA = true;
    public bool isVertical = true;
    public bool isRotating = false;
    public float rotateSpeed = 5f;
    public float radiusToRotate = 3f;
    private float angle;

    void Start()
    {
        aux = AuxManager.instance;
        
    }


    // Update is called once per frame
    void Update()
    {

        if (canSpawnIt)
        {
            CreateSpinner();
        }
        else if(isVertical)
        {
            Vector3 newPos = Vector3.zero;

            if (goToA)
                newPos = new Vector3(transform.position.x, transform.position.y + pointA, transform.position.z);
            else
                newPos = new Vector3(transform.position.x, transform.position.y - pointA, transform.position.z);

            spinner.transform.position = Vector3.MoveTowards(spinner.transform.position, newPos, speed * Time.deltaTime);

            if (Vector3.Distance(spinner.transform.position, newPos) < 0.1f)
            {
                goToA = !goToA;
            }
        }
        else if(isRotating)
        {
            angle += rotateSpeed * Time.deltaTime;
            Vector3 offset = new Vector3(Mathf.Sin(angle), Mathf.Cos(angle),0) * radiusToRotate;
            spinner.transform.position = transform.position + offset;
        }
       

        if(transform.position.x <= aux.wall.transform.position.x)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDisable()
    {
        canSpawnIt = true;
    }

    private void CreateSpinner()
    {
        spinner = aux.spinnerPool.GetPooledObject();
        spinner.transform.position = transform.position;
        spinner.SetActive(true);
        canSpawnIt = false;
    }


}
