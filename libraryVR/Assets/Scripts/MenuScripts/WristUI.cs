using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MenuScripts
{
    public class WristUI : MonoBehaviour
    {
        public InputActionAsset inputActions;
        public GameObject wristMenu;
        

        private InputAction _menu;
        

        void Start()
        {
            wristMenu.SetActive(false);
            _menu = inputActions.FindActionMap("XRI LeftHand").FindAction("Menu");
            _menu.Enable();
            _menu.performed += ToggleMenu;
        }

        private void OnDestroy()
        {
            _menu.performed -= ToggleMenu;
        }

        public void ToggleMenu(InputAction.CallbackContext context)
        {
            wristMenu.SetActive(!wristMenu.activeSelf);
        }
    }
}
