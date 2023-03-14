using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ToggleObject : MonoBehaviour
{
    public InputActionReference toggleReference;

    public List<GameObject> objects;
    // Start is called before the first frame update
    void Start()
    {
        toggleReference.action.started += Toggle;
    }

    private void OnDestroy()
    {
        toggleReference.action.started -= Toggle;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Toggle(InputAction.CallbackContext context)
    {
        foreach (var obj in objects)
        {
            bool isActive = !obj.activeSelf;
            obj.SetActive(isActive);
        }
        //bool isActive = !gameObject.activeSelf;
        //gameObject.SetActive(isActive);
    }
}
