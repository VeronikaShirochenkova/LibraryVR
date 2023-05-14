using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiralRotator : MonoBehaviour
{
    public float rotationSpeed = 50.0f; // скорость вращения
    
    void Update()
    {
        // вращение объекта вокруг его оси по оси Y
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime, Space.Self);
    }
}
