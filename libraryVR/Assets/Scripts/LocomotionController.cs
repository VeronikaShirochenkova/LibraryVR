using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class LocomotionController : MonoBehaviour
{
    public XRController teleportRay;
    public InputHelpers.Button teleportBtn;
    
    public XRController uiRay;
    public InputHelpers.Button uiBtn;
    
    public float threshold = 0.1f;

    // Update is called once per frame
    void Update()
    {
        bool btnIsPressed = TeleportBtnIsPressed(teleportRay);
        teleportRay.gameObject.SetActive(btnIsPressed);
        
        btnIsPressed = UIbtnIsPressed(teleportRay);
        uiRay.gameObject.SetActive(btnIsPressed);
    }

    public bool TeleportBtnIsPressed(XRController controller)
    {
        InputHelpers.IsPressed(controller.inputDevice, teleportBtn, out bool isPressed, threshold);
        return isPressed;
    }
    
    public bool UIbtnIsPressed(XRController controller)
    {
        InputHelpers.IsPressed(controller.inputDevice, uiBtn, out bool isPressed, threshold);
        return isPressed;
    }
}
