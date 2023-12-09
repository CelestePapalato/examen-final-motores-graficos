using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRotateAround : MonoBehaviour
{
    [SerializeField]
    Transform centerPoint;
    [SerializeField]
    float velocity;

    void Update()
    {
        transform.RotateAround(centerPoint.position, Vector3.up, velocity * Time.deltaTime);
    }
}
