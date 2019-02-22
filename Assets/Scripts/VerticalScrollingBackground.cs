using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerticalScrollingBackground : MonoBehaviour
{
    private AuxManager aux;
    private Transform cam;

    private void Start()
    {
        aux = AuxManager.instance;
        cam = aux.mainCamera.transform;

    }

    private void Update()
    {
        
    }
}
