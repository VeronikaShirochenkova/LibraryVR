using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.UI;

namespace MenuScripts
{
    public class AngleController : MonoBehaviour
    {
        [SerializeField] private Slider slider;
        [SerializeField] private GameObject player;

        void Start()
        {
            player.transform.rotation = Quaternion.Euler(slider.value * -90f, 0, 0);
            slider.onValueChanged.AddListener((v) =>
            {
                player.transform.rotation = Quaternion.Euler(v * -90f, 0, 0);
            });
        }
    }
}
