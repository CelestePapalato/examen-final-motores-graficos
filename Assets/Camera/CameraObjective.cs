using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraObjective : MonoBehaviour
{
    [Header("Seguimiento al jugador")]
    [SerializeField]
    [Range(0f, 0.3f)]float suavizadoMovimiento;
    [Header("Cámara")]
    [SerializeField]
    float sensibilidadX;
    [SerializeField]
    float sensibilidadY;
    [SerializeField]
    [Range(0f, 0.3f)] float rotationSmoothing;
    [SerializeField]
    float minAnguloVertical;
    [SerializeField]
    float maxAnguloVertical;

    float rotacionVerticalActual = 0f;
    float rotacionHorizontalActual = 0f;

    Vector2 input_vector = Vector2.zero;
    Vector2 smoothing_currentVelocty;
    Vector2 startPos;
    Vector2 endPos;

    GameObject player;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        rotarCamara();
    }

    private void FixedUpdate()
    {
        transform.position = Vector3.Lerp(transform.position, player.transform.position, (1f / suavizadoMovimiento) * Time.deltaTime);
    }

    void rotarCamara()
    {
        float horizontalInput = Input.GetAxis("Mouse X");
        float verticalInput = Input.GetAxis("Mouse Y");

        input_vector = new Vector2(horizontalInput, verticalInput);
        input_vector = Vector2.ClampMagnitude(input_vector, 1f);

        Vector2 offset = input_vector * Time.deltaTime;
        offset.x *= sensibilidadX;
        offset.y *= sensibilidadY;


        startPos = new Vector2(rotacionHorizontalActual, rotacionVerticalActual);
        endPos = endPos + offset;
        endPos.y = Mathf.Clamp(endPos.y, minAnguloVertical, maxAnguloVertical);

        startPos = Vector2.SmoothDamp(startPos, endPos, ref smoothing_currentVelocty, rotationSmoothing);

        rotacionHorizontalActual = startPos.x;
        rotacionVerticalActual = startPos.y;

        transform.localRotation = Quaternion.Euler(rotacionVerticalActual, rotacionHorizontalActual, 0f);
    }
}
