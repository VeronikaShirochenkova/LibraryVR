using UnityEngine;

namespace KeyboardScripts
{
    public class KeyboardButton : MonoBehaviour
    {
        public Keyboard keyboard;              // ref to the keyboard
    
        private ButtonVR _button;                // ref to the keyboard button

        void Start()
        {
            // initialize components
            _button = GetComponentInChildren<ButtonVR>();

            // adding listeners to the buttons
            // if (transform.name.Length == 1)
            // {
            //     //_button.onRelease.AddListener(SendTextToInputField);
            // }
            // else switch (transform.name)
            // {
            //     case "Space":
            //         _button.onRelease.AddListener(SendSpaceToInputField);
            //         break;
            //     case "Back":
            //         _button.onRelease.AddListener(DeleteCharInInputField);
            //         break;
            //     case "Tab":
            //         _button.onRelease.AddListener(SendTabToInputField);
            //         break;
            //     case "Enter":
            //         _button.onRelease.AddListener(SendEnterToInputField);
            //         break;
            //     case "Caps":
            //         _button.onRelease.AddListener(ChangeLetters);
            //         break;
            // }

        }
        
        /**
     * action for listener;
     * call InsertChar function
     */
        public void SendTextToInputField()
        {
            keyboard.InsertChar(transform.name);
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

        private void SendTabToInputField()
        {
            keyboard.InsertChar("\t");
        }
        
        private void SendEnterToInputField()
        {
            keyboard.InsertChar("\n");
        }


        private void ChangeLetters()
        {
            keyboard.CapsPressed();
        }
    }
}
