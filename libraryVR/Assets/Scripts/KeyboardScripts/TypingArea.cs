using System;
using UnityEngine;

namespace KeyboardScripts
{
    public class TypingArea : MonoBehaviour
    {
        [Header("Typing spheres")]
        public GameObject leftTypingSphere;       // ref to typing object on left hand
        public GameObject rightTypingSphere;      // ref to typing object on right hand

        [Header("Basic hand models")]
        public GameObject baseHandLeft;
        public GameObject baseHandRight;

        [Header("Typing hand models")]
        public GameObject typingHandLeft;
        public GameObject typingHandRight;

        private bool _resetLeftHand;
        private bool _resetRightHand;

        private void Start()
        {
            _resetLeftHand = false;
            _resetRightHand = false;
        }

        private void Update()
        {
            if (!transform.gameObject.activeSelf)
            {
                if (!_resetLeftHand)
                {
                    ResetHandVisibility("LeftHand" , false);
                    _resetLeftHand = true;
                }

                if (!_resetRightHand)
                {
                    ResetHandVisibility("RightHand" , false);
                    _resetRightHand = true;
                }
            }
        }

        private void ResetHandVisibility(string tag, bool value)
        {
            if (tag == "LeftHand")
            {
                leftTypingSphere.SetActive(value);
                typingHandLeft.SetActive(value);
                
                baseHandLeft.SetActive(!value);
            }
            else
            {
                rightTypingSphere.SetActive(value);
                typingHandRight.SetActive(value);
                
                baseHandRight.SetActive(!value);
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Hands")) return;       // if the collision isn't with hands
        
            // activating typing objects
            if (other.CompareTag("LeftHand"))
            {
                ResetHandVisibility("LeftHand" , true);
                _resetLeftHand = false;
                //leftTypingSphere.SetActive(true);

                //baseHandLeft.SetActive(false);
                //typingHandLeft.SetActive(true);
            }
            else if (other.CompareTag("RightHand"))
            {
                ResetHandVisibility("RightHand" , true);
                _resetRightHand = false;
                //rightTypingSphere.SetActive(true);

                //baseHandRight.SetActive(false);
                //typingHandRight.SetActive(true);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer != LayerMask.NameToLayer("Hands")) return;       // if the collision isn't with hands
        
            // deactivating typing objects
            if (other.CompareTag("LeftHand"))
            {
                ResetHandVisibility("LeftHand" , false);
                _resetLeftHand = true;
                //leftTypingSphere.SetActive(false);

                //baseHandLeft.SetActive(true);
                //typingHandLeft.SetActive(false);
            }
            else if (other.CompareTag("RightHand"))
            {
                ResetHandVisibility("RightHand" , false);
                _resetRightHand = true;
                //rightTypingSphere.SetActive(false);

                //baseHandRight.SetActive(true);
                //typingHandRight.SetActive(false);
            }
        }
    }
}
