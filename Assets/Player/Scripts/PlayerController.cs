using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    
    [Header("Movimiento")]
    [SerializeField]
    float aceleracionMovimiento;
    [SerializeField]
    float velocidadMaxima;
    [SerializeField]
    float suavizadoDesaceleracion;
    [SerializeField]
    bool ajustarACamara = true;

    [SerializeField]
    [Range(0f, 0.3f)] float suavizadoDeRotacion;

    Vector2 input_vector = Vector2.zero;

    float smoothDampAngle_currentVelocity;

    GameObject cam;

    //Componentes

    Rigidbody rb;
    Animator animator;

    void Start()
    {
        cam = Camera.main.gameObject;
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
    }

    void Update()
    {
        input_movimiento();

        updateBlendParameters();
    }

    private void FixedUpdate()
    {
        suavizadoRotacionJugador();
        physics_movimiento();
    }


    // #---------------- MOVIMIENTO ----------------#

    void input_movimiento()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        input_vector.x = x;
        input_vector.y = y;

        input_vector = Vector2.ClampMagnitude(input_vector, 1f);
    }

    void physics_movimiento()
    {
        Vector3 direction = transform.forward * input_vector.magnitude * aceleracionMovimiento * Time.fixedDeltaTime;
        rb.AddForce(direction, ForceMode.Acceleration);

        if (rb.velocity.magnitude > velocidadMaxima)
        {
            Vector3 currentVelocity = rb.velocity;
            currentVelocity.y = 0;
            Vector3 vel_objetivo = currentVelocity.normalized * velocidadMaxima;
            Vector3 vel_excedida = currentVelocity - vel_objetivo;
            vel_excedida *= (1f / suavizadoDesaceleracion) * Time.fixedDeltaTime;
            rb.AddForce(vel_excedida * -1, ForceMode.Acceleration);
        }

    }

    void suavizadoRotacionJugador()
    {                
        if (input_vector != Vector2.zero)
        {

            float rotacionActual = transform.eulerAngles.y;
            float rotacionObjetivo = Mathf.Atan2(input_vector.x, input_vector.y) * Mathf.Rad2Deg;
            if (ajustarACamara)
            {
                rotacionObjetivo += cam.transform.eulerAngles.y;
            }

            float rotacion = Mathf.SmoothDampAngle(rotacionActual, rotacionObjetivo, ref smoothDampAngle_currentVelocity, suavizadoDeRotacion);

            rb.MoveRotation(Quaternion.Euler(0f, rotacion, 0f));
        }
    }

    // #---------------- ANIMACIÓN ----------------#

    void updateBlendParameters()
    {
        float velocityBlend = rb.velocity.magnitude / velocidadMaxima;
        velocityBlend = Mathf.Clamp(velocityBlend, 0f, 1f);
        animator.SetFloat("Velocity", velocityBlend);
    }

}
