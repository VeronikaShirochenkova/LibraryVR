using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;


public class HandController : MonoBehaviour
{
    [SerializeField] private XRRayInteractor _xrRayInteractor;
    [SerializeField] private ActionBasedController _actionBasedController;
    [SerializeField] private XRDirectInteractor _xrDirectInteractor;
    [SerializeField] private InputActionReference _teleportActionRef;
    
    private void OnEnable()
    {
        _teleportActionRef.action.performed += teleportModeActivate;
        _teleportActionRef.action.canceled += teleportModeCancel;

    }
    
    private void OnDisable()
    {
        _teleportActionRef.action.performed -= teleportModeActivate;
        _teleportActionRef.action.canceled -= teleportModeCancel;
    }

    private void teleportModeActivate(InputAction.CallbackContext obj)
    {
        //_xrDirectInteractor.enabled = false;

        _xrRayInteractor.enabled = true;
        _actionBasedController.enableInputActions = true;
    }

    private void teleportModeCancel(InputAction.CallbackContext obj) => Invoke("DisableTeleport", 0.05f);

    private void DisableTeleport()
    {
        _xrDirectInteractor.enabled = true;

        _xrRayInteractor.enabled = false;
        _actionBasedController.enableInputActions = false;
    }


}
