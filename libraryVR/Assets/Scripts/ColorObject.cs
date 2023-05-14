using UnityEngine;
using UnityEngine.InputSystem;

public class ColorObject : MonoBehaviour
{

    public InputActionReference colorReference;
    private MeshRenderer _meshRender;
    
    // Start is called before the first frame update
    void Start()
    {
        _meshRender = GetComponent<MeshRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        float value = colorReference.action.ReadValue<float>();
        UpdateColor(value);
    }

    private void UpdateColor(float value)
    {
        _meshRender.material.color = new Color(value, value, value);
    }
}
