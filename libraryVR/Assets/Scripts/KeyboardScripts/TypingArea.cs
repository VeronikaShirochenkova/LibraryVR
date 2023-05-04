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

        [Header("Other")] 
        public GameObject marker;

        private bool _resetLeftHand;
        private bool _resetRightHand;

        private void Start()
        {
            _resetLeftHand = true;
            _resetRightHand = true;
        }

        
        public void CheckVisibility()
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
            }
            else if (other.CompareTag("RightHand") && !marker.activeSelf)
            {
                ResetHandVisibility("RightHand" , true);
                _resetRightHand = false;
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
            }
            else if (other.CompareTag("RightHand") && !marker.activeSelf)
            {
                ResetHandVisibility("RightHand" , false);
                _resetRightHand = true;
            }
        }
    }
}
