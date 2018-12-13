using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorOffscreen : MonoBehaviour {
    private AuxManager aux;
    private Camera mainCamera;

    private RectTransform m_icon;
    private Image m_iconImage;
    [Space]
    [Range(0, 100)]
    public float m_edgeBuffer;
    [Space]
    public bool PointTarget = true;
    //Indicates if the object is out of the screen
    private bool m_outOfScreen;
    void Start()
    {
        aux = AuxManager.instance;
        mainCamera = aux.GetCamera();
        m_icon = aux.GetIndicatorPool().GetPooledObject().GetComponent<RectTransform>();
    }
    void LateUpdate()
    {
        
        UpdateTargetIconPosition();
    }

    private void UpdateTargetIconPosition()
    {
        Vector3 newPos = transform.position;
        newPos = mainCamera.WorldToViewportPoint(newPos);

        if (newPos.x > 1 || newPos.y > 1 || newPos.x < 0 || newPos.y < 0)
        {
            m_outOfScreen = true;
        }else
            m_outOfScreen = false;
        if (newPos.z < 0)
        {
            newPos.x = 1f - newPos.x;
            newPos.y = 1f - newPos.y;
            newPos.z = 0;
            newPos = Vector3Maxamize(newPos);
        }
        newPos = mainCamera.ViewportToScreenPoint(newPos);
        newPos.x = Mathf.Clamp(newPos.x, m_edgeBuffer, Screen.width - m_edgeBuffer);
        newPos.y = Mathf.Clamp(newPos.y, m_edgeBuffer, Screen.height - m_edgeBuffer);
        
        //Operations if the object is out of the screen
        if (m_outOfScreen)
        {
            m_icon = GetNewIcon();
            m_icon.transform.position = newPos;
            m_icon.gameObject.SetActive(true);
            //Show the target off screen icon
            //m_iconImage.sprite = m_targetIconOffScreen;
            if (PointTarget)
            {
                //Rotate the sprite towards the target object
                var targetPosLocal = mainCamera.transform.InverseTransformPoint(transform.position);
                var targetAngle = -Mathf.Atan2(targetPosLocal.x, targetPosLocal.y) * Mathf.Rad2Deg - 90;
                //Apply rotation
                m_icon.transform.eulerAngles = new Vector3(0, 0, targetAngle + 180);
            }

        }
        else
        {
            m_icon.gameObject.SetActive(false);
            //Reset rotation to zero and swap the sprite to the "on screen" one
            /*m_icon.transform.eulerAngles = new Vector3(0, 0, 0);
            m_iconImage.sprite = m_targetIconOnScreen;*/
        }

    }

    public Vector3 Vector3Maxamize(Vector3 vector)
    {
        Vector3 returnVector = vector;
        float max = 0;
        max = vector.x > max ? vector.x : max;
        max = vector.y > max ? vector.y : max;
        max = vector.z > max ? vector.z : max;
        returnVector /= max;
        return returnVector;
    }

    private RectTransform GetNewIcon()
    {
        if (m_icon.gameObject.activeInHierarchy)
        {
            return m_icon;
        }
        else
        {
            return aux.GetIndicatorPool().GetPooledObject().GetComponent<RectTransform>();
        }
    } 
}
