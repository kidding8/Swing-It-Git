using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupText : MonoBehaviour {

    private AuxManager aux;
    private bool alreadyStarted = false;
    public ColorType currentColorType = ColorType.neutral;
    private Color currentColor;
    /*private TextMeshProUGUI textPro;

    private void Start()
    {
        textPro = GetComponent<TextMeshProUGUI>();
    }*/

    private void Start()
    {
        aux = AuxManager.instance;
        alreadyStarted = true;
        ChangeColor();
    }

    private void OnEnable()
    {
        if (alreadyStarted )
        {
            if (!aux.IsColor(currentColor, currentColorType))
            {
                ChangeColor();
            }
        }
    }

    public void SetText(string txt)
    {
        GetComponent<TextMeshProUGUI>().text = txt;
    }

    public void SetColor(Color color)
    {
        GetComponent<TextMeshProUGUI>().color = color;
    }
    private void ChangeColor()
    {
        currentColor = aux.GetColor(currentColorType);
        GetComponent<TextMeshProUGUI>().color = currentColor;
    }

}
