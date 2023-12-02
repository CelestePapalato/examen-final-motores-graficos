using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Controles")]
    [SerializeField]
    KeyCode ataqueTecladoMouse;
    [SerializeField]
    KeyCode ataqueGamepad;

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

    // Movimiento
    Vector3 direction = Vector3.zero;

    // Componentes

    Rigidbody rb;
    Animator animator;
    HealthComponent health;

    // Estados
    enum State { IDLE, ATTACK, PARALYSED}
    State estadoJugador;

    // Banderas
    bool canAttack = true;

    void Start()
    {
        estadoJugador = State.IDLE;
        cam = Camera.main.gameObject;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        health = GetComponentInChildren<HealthComponent>();
    }

    void Update()
    {
        switch (estadoJugador)
        {
            case State.IDLE:
                idle_state();
                break;
            case State.ATTACK:
                attack_state();
                break;                        
        }

        updateMovementAnimationBlend();


    }

    private void FixedUpdate()
    {
        switch (estadoJugador)
        {
            case State.IDLE:
                suavizadoRotacionJugador();
                break;
            case State.ATTACK:
                dontMove();
                break;
            case State.PARALYSED:
                dontMove();
                break;
        }

        move();
    }


    // #---------------- MOVIMIENTO ----------------#

    void idle_state()
    {
        attack_input();
        input_movimiento();
    }

    void input_movimiento()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        input_vector.x = x;
        input_vector.y = y;

        input_vector = Vector2.ClampMagnitude(input_vector, 1f);

        direction = transform.forward * input_vector.magnitude * aceleracionMovimiento;
    }

    void move()
    {
        direction *= Time.fixedDeltaTime;
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

    void dontMove()
    {
        direction = Vector3.zero;
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

    // #---------------- ATAQUE ----------------#

    void attack_input()
    {
        bool input_attack = Input.GetKeyDown(ataqueGamepad) || Input.GetKeyDown(ataqueTecladoMouse);

        if (input_attack && canAttack)
        {
            estadoJugador = State.ATTACK;
            updateAttackAnimation();
        }
    }
    void attack_state()
    {
        attack_input();
    }


    // #---------------- ANIMACIÓN ----------------#

    // #-- Eventos para animaciones --#

    void enableInvulnerability()
    {
        health.changeInvulnerability(true, gameObject);
    }

    void disableInvulnerability()
    {
        health.changeInvulnerability(false, gameObject);
    }

    void disableAttackInput()
    {
        canAttack = false;
    }

    void enableAttackInput()
    {
        canAttack = true;
    }

    public void changeToAttack()
    {
        estadoJugador = State.ATTACK;
    }

    public void changeToParalysed()
    {
        estadoJugador = State.PARALYSED;
    }

    public void changeToIdle()
    {
        estadoJugador = State.IDLE;
    }

    void addImpulseToCharacter(float value)
    {
        rb.AddForce(transform.forward * value, ForceMode.Impulse);
    }

    // # ---------------------------- #

    void updateMovementAnimationBlend()
    {
        float velocityBlend = rb.velocity.magnitude / velocidadMaxima;
        velocityBlend = Mathf.Clamp(velocityBlend, 0f, 1f);
        animator.SetFloat("Velocity", velocityBlend);
    }

    void updateAttackAnimation()
    {
        animator.SetTrigger("Attack");
    }

}
