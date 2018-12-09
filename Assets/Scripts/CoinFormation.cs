using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinFormation : MonoBehaviour {

    private Transform[] coinList;
    private AuxManager aux;
	// Use this for initialization
	void Start () {
        aux = AuxManager.instance;
        int count = transform.childCount;
        coinList = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            coinList[i] = transform.GetChild(i).transform;
        }
        CreateCoins();
	}
	
	
    public void CreateCoins()
    {
        foreach(Transform t in coinList)
        {
            GameObject coin = aux.GetSingleCoinPool().GetPooledObject();
            coin.SetActive(true);
            coin.transform.position = t.position;
        }
    }
}
