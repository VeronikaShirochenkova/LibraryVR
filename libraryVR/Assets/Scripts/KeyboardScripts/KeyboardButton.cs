using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class KeyboardButton : MonoBehaviour
{
    public Keyboard keyboard;              // ref to the keyboard
    
    private ButtonVR _button;                // ref to the keyboard button
    private TextMeshProUGUI _buttonText;     // ref to the text which is displayed on the keyboard button
    
    void Start()
    {
        // initialize components
        _buttonText = GetComponentInChildren<TextMeshProUGUI>();
        _button = GetComponentInChildren<ButtonVR>();
        
        // adding listeners to the buttons
        if (_buttonText.text.Length == 1)
        {
            NameToButtonText(); 
            _button.onRelease.AddListener(SendTextToInputField);
        }
        else if (_buttonText.text == "Space") _button.onRelease.AddListener(SendSpaceToInputField);
        else if (_buttonText.text == "Delete") _button.onRelease.AddListener(DeleteCharInInputField);

    }
    
    /**
     * get button name and write it to text in button's text component
     */
    private void NameToButtonText()
    {
        _buttonText.text = gameObject.name;
    }
    
    /**
     * action for listener;
     * call InsertChar function
     */
    private void SendTextToInputField()
    {
        keyboard.InsertChar(_buttonText.text);
    }
    
    /**
     * action for listener;
     * call InsertSpace function
     */
    private void SendSpaceToInputField()
    {
        keyboard.InsertSpace();
    }

    /**
     * action for listener;
     * call DeleteChar function
     */
    private void DeleteCharInInputField()
    {
        keyboard.DeleteChar();
    }
}
