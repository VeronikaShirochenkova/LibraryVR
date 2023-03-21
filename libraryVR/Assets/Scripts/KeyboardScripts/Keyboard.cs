using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Keyboard : MonoBehaviour
{
    public TMP_InputField inputField;           // ref to search box
    //public GameObject normalButtons;            
    //public GameObject capsButtons;              
    //private bool caps;                          
    
    void Start()
    {
        //caps = false;
    }

    
    /**
     * insert character to the end of string in search box;
     */
    public void InsertChar(string s)
    {
        inputField.text += s;
        ResetCaretPos();
    }
    

    /**
     * delete last character to the end of string in search box;
     */
    public void DeleteChar()
    {
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
            ResetCaretPos();
        }
    }
    
    
    /**
     * insert " " character to the end of string in search box;
     */
    public void InsertSpace()
    {
        inputField.text += " ";
        ResetCaretPos();
    }
    
    
    /**
     * update position of caret in search box;
     */
    private void ResetCaretPos()
    {
        inputField.caretPosition = inputField.text.Length;
    }
    

    // public void CapsPressed()
    // {
    //     if (!caps)
    //     {
    //         normalButtons.SetActive(false); 
    //         capsButtons.SetActive(true);
    //         caps = true;
    //     }
    //     else
    //     {
    //         normalButtons.SetActive(true);
    //         capsButtons.SetActive(false);
    //         caps = false;
    //     }
    // }
}
