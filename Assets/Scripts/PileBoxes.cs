using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PileBoxes : MonoBehaviour
{
    private Transform[] boxesTrans;
    private AuxManager aux;
    private bool canSpawnIt = false;
    // private List<GameObject> coins  = new List<GameObject>();
    private void Start()
    {
        aux = AuxManager.instance;
        //coins = new List<GameObject>();
        StartChilds();
        canSpawnIt = true;
    }

    private void Update()
    {
        if (canSpawnIt)
        {
            CreateBoxes();
            canSpawnIt = false;
        }


    }

    private void OnDisable()
    {
        canSpawnIt = true;
    }

    private void StartChilds()
    {
        int count = transform.childCount;
        boxesTrans = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            boxesTrans[i] = transform.GetChild(i).transform;
        }
    }


    public void CreateBoxes()
    {
        foreach (Transform t in boxesTrans)
        {
            GameObject box = aux.GetBoxPool().GetPooledObject();
            box.SetActive(true);
            //coins.Add(coin);
            box.transform.position = t.position;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Wall"))
        {
            gameObject.SetActive(false);
        }
    }
}
