using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class TypingArea : MonoBehaviour
{
    public GameObject leftTypingHand;       // ref to typing object on left hand
    public GameObject rightTypingHand;      // ref to typing object on right hand

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Hands")) return;       // if the collision isn't with hands
        
        // activating typing objects
        if (other.CompareTag("LeftHand")) leftTypingHand.SetActive(true);
        else if (other.CompareTag("RightHand")) rightTypingHand.SetActive(true);
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != LayerMask.NameToLayer("Hands")) return;       // if the collision isn't with hands
        
        // deactivating typing objects
        if (other.CompareTag("LeftHand")) leftTypingHand.SetActive(false);
        else if (other.CompareTag("RightHand")) rightTypingHand.SetActive(false);
    }
}
