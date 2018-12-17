using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinFormation : MonoBehaviour {


    private Transform[] coinList;
    private AuxManager aux;
    private bool canSpawnIt = false;
    private List<GameObject> coins  = new List<GameObject>();
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
            CreateCoins();
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
        coinList = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            coinList[i] = transform.GetChild(i).transform;
        }
    }


    public void CreateCoins()
    {
        foreach(Transform t in coinList)
        {
            GameObject coin = aux.GetSingleCoinPool().GetPooledObject();
            coin.SetActive(true);
            coins.Add(coin);
            coin.transform.position = t.position;
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
