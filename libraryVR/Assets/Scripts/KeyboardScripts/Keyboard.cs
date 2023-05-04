using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace KeyboardScripts
{
    public class Keyboard : MonoBehaviour
    {
        public TMP_InputField inputField;           // ref to search box
        public GameObject normalLetters;            
        public GameObject capsLetters;
        public GameObject normalSigns;
        public GameObject capsSigns;
        public GameObject numbers;
                        
    
        void Start()
        {
            capsLetters.SetActive(false);
            capsSigns.SetActive(false);
        }

    
        /**
        * insert character to the end of string in search box;
        */
        public void InsertChar(string s)
        {
            if (inputField.text.Length == 0)
            {
                inputField.Select();
            }
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
    

        public void CapsPressed()
        {
            normalLetters.SetActive(!normalLetters.activeSelf);
            capsLetters.SetActive(!capsLetters.activeSelf);
            
            normalSigns.SetActive(!normalSigns.activeSelf);
            capsSigns.SetActive(!capsSigns.activeSelf);
            
            numbers.SetActive(!numbers.activeSelf);
        }
    }
}
