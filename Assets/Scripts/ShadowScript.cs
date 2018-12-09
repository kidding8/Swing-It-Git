using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowScript : MonoBehaviour {

    private AuxManager aux;
    public Vector2 offset = new Vector2(-1, -1);
    public bool updateEveryFrame = false;
    private Material shadowMaterial;
    private Color shadowColor;

    private SpriteRenderer sRendererCaster;
    private SpriteRenderer sRendererShadow;
    // Use this for initialization
    private Transform transformCaster;
    private Transform transformShadow;
    
    void Start () {
        aux = AuxManager.instance;

        shadowMaterial = aux.GetShadowMaterial();
        shadowColor = aux.GetShadowColor();

        transformCaster = transform;
        transformShadow = new GameObject().transform;
        transformShadow.parent = transformCaster;
        transformShadow.gameObject.name = "Shadow";
        transformShadow.localRotation = Quaternion.identity;
        sRendererCaster = GetComponent<SpriteRenderer>();
        sRendererShadow = transformShadow.gameObject.AddComponent<SpriteRenderer>();
        sRendererShadow.sprite = sRendererCaster.sprite;

        sRendererShadow.material = shadowMaterial;
        sRendererShadow.color = shadowColor;
        sRendererShadow.sortingLayerName = sRendererCaster.sortingLayerName;
        sRendererShadow.sortingOrder = sRendererCaster.sortingOrder -2;

    }

    private void LateUpdate()
    {
        transformShadow.position = new Vector2(transformCaster.position.x + offset.x, transformCaster.position.y + offset.y);
        if (updateEveryFrame)
        {
            sRendererShadow.sprite = sRendererCaster.sprite;
        }
        

    }
}
