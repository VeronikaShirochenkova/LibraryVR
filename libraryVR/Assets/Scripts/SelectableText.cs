using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;


public class SelectableText : MonoBehaviour
{
    private TMP_InputField _inputField;
    private RectTransform rectTransform;
    private int _cursorIndex;
    void Start()
    {
        _inputField = transform.GetComponent<TMP_InputField>();
        rectTransform = _inputField.GetComponent<RectTransform>();
        var rect = rectTransform.rect;
        float width = rect.width;
        float height = rect.height;
        
        //Debug.Log(width + " " + height);

    }



    void Update()
    {
        //Debug.Log("start: " + _inputField.selectionAnchorPosition);
        //Debug.Log("end: " + _inputField.selectionFocusPosition);

    }
}
