using UnityEngine;

namespace KeyboardScripts
{
    public class TouchButton : MonoBehaviour
    {
        public TouchKeyboard keyboard;
        private string sign;
        private AudioSource sound;          // ref to audio source
        
        

        void Start()
        {
            sound = GetComponent<AudioSource>();    // get sound for "click"
            sign = transform.gameObject.name;

        }

        public void Click()
        {
            if (sign.Length == 1)
            {
                SendTextToInputField();
            }
            else switch (sign)
            {
                case "Space":
                    SendSpaceToInputField();
                    break;
                case "Back":
                    DeleteCharInInputField();
                    break;
                case "Tab":
                    SendTabToInputField();
                    break;
                case "Enter":
                    SendEnterToInputField();
                    break;
                case "Caps":
                    ChangeLetters();
                    break;
            }
            
            sound.Play();
        }
        
        
        private void SendTextToInputField()
        {
            keyboard.InsertChar(sign);
        }
        
        private void SendSpaceToInputField()
        {
            keyboard.InsertSpace();
        }
        
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
