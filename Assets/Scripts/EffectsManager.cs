using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectsManager : MonoBehaviour {


    private AuxManager aux;
    public ObjectPooler coinPickupParticles;
    public ObjectPooler CloseCallTxtPooler;
    public ObjectPooler DisappearingCirclePool;
    public ObjectPooler HookGrabParticles;
    private Camera cam;
    private Canvas canvas;
    private float shakeDuration = 0f;
    // Amplitude of the shake. A larger value shakes the camera harder.
    public float shakeAmount = 0.7f;
    public float decreaseFactor = 1.0f;
    public bool speeding = false;
    Vector3 originalPos;


    public static EffectsManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        
    }
    void Start () {
        aux = AuxManager.instance;
        cam = aux.GetCamera();
        canvas = aux.GetCanvas();
	}
	
	public void CreateCameraShake(float time)
    {
        shakeDuration = time;
    }

	void Update () {

        if (shakeDuration > 0)
        {
            cam.transform.localPosition = cam.transform.localPosition + Random.insideUnitSphere * shakeAmount;
            shakeDuration -= Time.deltaTime * decreaseFactor;
        }
        else
        {
            shakeDuration = 0f;
            //camTransform.localPosition = originalPos;
        }

        if (speeding)
        {
            cam.transform.localPosition = cam.transform.localPosition + Random.insideUnitSphere * 0.05f;
        }
    }



    public void CameraShake(float time, float amount)
    {
        StartCoroutine(ShakeCamera(time, amount));
    }

    IEnumerator ShakeCamera(float time, float amount)
    {
        cam.transform.localPosition = cam.transform.localPosition + Random.insideUnitSphere * amount;
        yield return new WaitForSeconds(time);
    }

    public void SetCoinPickUpParticles(Vector3 pos)
    {
        GameObject particle = coinPickupParticles.GetPooledObject();
        particle.transform.position = pos;
        particle.SetActive(true);
        /*ParticleSystem newParticle = particle.GetComponent<ParticleSystem>();
        var emission = newParticle.emission;
        emission.enabled = true;*/
        //var main = newParticle.main;
        //main.startColor = color;
    }

    public void GenerateText(string text, Transform otherTransform)
    {
        GameObject newTransform = CloseCallTxtPooler.GetPooledObject();
        PopupText popup = newTransform.GetComponentInChildren<PopupText>();
        popup.SetText(text);
        /*switch (num)
        {
            case 1:
                popup.SetColor(new Color(250, 197, 28));
                break;
            case 2:
                popup.SetColor(new Color(251, 160, 38));
                break;
            case 3:
                popup.SetColor(new Color(243, 121, 52));
                break;
        }*/
        newTransform.transform.SetParent(canvas.transform, false);
        Vector3 newPos = cam.WorldToScreenPoint(otherTransform.position);
        newTransform.transform.position = newPos;
        newTransform.transform.rotation = Quaternion.identity;
        newTransform.gameObject.SetActive(true);
    }

    public void CreateDisappearingCircle(Vector3 pos)
    {
        GameObject circle = DisappearingCirclePool.GetPooledObject();
        circle.SetActive(true);
        circle.transform.position = pos;
    }

    public void CreateHookGrabParticle(Vector3 pos)
    {
        GameObject particle = HookGrabParticles.GetPooledObject();
        particle.SetActive(true);
        particle.transform.position = pos;
    }
}
