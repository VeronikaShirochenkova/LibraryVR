using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

namespace MenuScripts
{
    public class PlayerHeightController : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private XROrigin xrOrigin;

        void Start()
        {
            xrOrigin.CameraYOffset = slider.value;
            slider.onValueChanged.AddListener((v) => { xrOrigin.CameraYOffset = v;});
        }
        
    }
}
