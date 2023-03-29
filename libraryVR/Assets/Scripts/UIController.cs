using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class UIController : MonoBehaviour
{
    [SerializeField] private XRRayInteractor _xrRayInteractor;
    [SerializeField] private ActionBasedController _actionBasedController;
    [SerializeField] private XRDirectInteractor _xrDirectInteractor;
    [SerializeField] private InputActionReference _UIActionRef;
    
    
    private void OnEnable()
    {
        _UIActionRef.action.performed += _UIModeActivate;
        _UIActionRef.action.canceled += _UIModeCancel;

    }

    private void _UIModeActivate(InputAction.CallbackContext obj)
    {
        _xrDirectInteractor.enabled = false;

        _xrRayInteractor.enabled = true;
        _actionBasedController.enableInputActions = true;
    }

    private void _UIModeCancel(InputAction.CallbackContext obj) => Invoke("DisableUI", 0.05f);

    private void DisableUI()
    {
        _xrDirectInteractor.enabled = true;

        //_xrRayInteractor.enabled = false;
        _actionBasedController.enableInputActions = false;
    }

    private void OnDisable()
    {
        _UIActionRef.action.performed -= _UIModeActivate;
        _UIActionRef.action.canceled -= _UIModeCancel;
    }
}
