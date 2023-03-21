using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonVR : MonoBehaviour
{
    public GameObject button;           // ref to button body
    private GameObject presser;         // ref to presser
    private AudioSource sound;          // ref to audio source
    private bool isPressed;
    
    public UnityEvent onPress; 
    public UnityEvent onRelease;

    void Start()
    {
        sound = GetComponent<AudioSource>();    // get sound for "click"
        isPressed = false;                      // start setup
        
        // initialize events
        onPress = new UnityEvent();         
        onRelease = new UnityEvent();
    }

    private void OnTriggerEnter(Collider other)
    {
        // if button isn't pressed yet
        if (!isPressed)
        {
            button.transform.localPosition = new Vector3(0, 0.003f, 0);         // change mesh position
            presser = other.gameObject;                                              // detect presser object
            onPress.Invoke();                                                        // invoke "onPress" event
            sound.Play();                                                            // play "click" sound
            isPressed = true;                                                        // change bool variable
        }
    }

    private void OnTriggerExit(Collider other)
    {   
        // if presser has release the button
        if (other.gameObject == presser)
        {
            button.transform.localPosition = new Vector3(0, 0.015f, 0);         // change mesh position
            onRelease.Invoke();                                                      // invoke "onRelease" event
            isPressed = false;                                                       // change bool variable
        }
    }
    
}
