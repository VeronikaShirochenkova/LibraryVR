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
