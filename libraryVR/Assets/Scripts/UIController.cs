using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class UIController : MonoBehaviour
{
    [SerializeField] private XRRayInteractor _xrRayInteractor;
    [SerializeField] private ActionBasedController _actionBasedController;
    
    [SerializeField] private XRRayInteractor _xrRayInteractor2;
    [SerializeField] private ActionBasedController _actionBasedController2;
    
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
        if (_xrRayInteractor2)
        {
            _xrRayInteractor2.enabled = true;
        }
        
        
        _actionBasedController.enableInputActions = true;

        if (_actionBasedController2)
        {
          _actionBasedController2.enableInputActions = true;  
        }
        
    }

    private void _UIModeCancel(InputAction.CallbackContext obj) => Invoke("DisableUI", 0.05f);

    private void DisableUI()
    {
        _xrDirectInteractor.enabled = true;

        //_xrRayInteractor.enabled = false;
        _actionBasedController.enableInputActions = false;
        if (_actionBasedController2)
        {
            _actionBasedController2.enableInputActions = false;  
        }
    }

    private void OnDisable()
    {
        _UIActionRef.action.performed -= _UIModeActivate;
        _UIActionRef.action.canceled -= _UIModeCancel;
    }
}
