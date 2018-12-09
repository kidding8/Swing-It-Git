using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupText : MonoBehaviour {

    /*private TextMeshProUGUI textPro;

    private void Start()
    {
        textPro = GetComponent<TextMeshProUGUI>();
    }*/

    public void SetText(string txt)
    {
        GetComponent<TextMeshProUGUI>().text = txt;
    }

    public void SetColor(Color color)
    {
        GetComponent<TextMeshProUGUI>().color = color;
    }

}
