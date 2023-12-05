using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Healthbar : MonoBehaviour
{
    [SerializeField]
    float maxDistanceToCamera;

    Canvas canvas;
    Camera cam;

    void Start()
    {
        cam = Camera.main;
        canvas = GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cam.transform);

        float distanceToCamera = (cam.transform.position - transform.position).magnitude;

        if(distanceToCamera > maxDistanceToCamera)
        {
            canvas.enabled = false;
        }

        if(distanceToCamera <= maxDistanceToCamera && !canvas.enabled)
        {
            canvas.enabled = true;
        }
    }
}
