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
    float impulsoEsquive;
    [SerializeField]
    float tiempoInhabilitacionEsquive;
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
    State estado;

    // Banderas
    bool canAttack = true;
    bool canDodge = true;

    void Start()
    {
        estado = State.IDLE;
        cam = Camera.main.gameObject;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        health = GetComponentInChildren<HealthComponent>();
    }

    void Update()
    {
        input_movimiento();
        switch (estado)
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
        switch (estado)
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
        input_esquive();
        attack_input();
        avanzar();
    }

    void input_movimiento()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        input_vector.x = x;
        input_vector.y = y;

        input_vector = Vector2.ClampMagnitude(input_vector, 1f);      
    }

    void avanzar()
    {
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
        if (Input.GetButtonDown("Attack") && canAttack)
        {
            estado = State.ATTACK;
            updateAttackAnimation();
        }
    }
    void attack_state()
    {
        input_esquive();
        attack_input();
    }

    // #---------------- ESQUIVE ----------------#

    void input_esquive()
    {
        if (Input.GetButtonDown("Dodge") && canDodge)
        {
            Vector2 _input_rodar = input_vector.normalized;
            if (_input_rodar == Vector2.zero)
            {
                _input_rodar = Vector2.up;
            }
            float x = _input_rodar.x;
            float y = _input_rodar.y;
            Vector3 input_rodar = new Vector3(x, 0, y);
            input_rodar = Quaternion.Euler(0f, cam.transform.eulerAngles.y, 0f) * input_rodar;
            rb.AddForce(impulsoEsquive * input_rodar, ForceMode.Impulse);
            updateDodgeAnimation(x, y);
            StartCoroutine(inhabilitarEsquive());
        }
    }

    IEnumerator inhabilitarEsquive()
    {
        canDodge = false;
        yield return new WaitForSeconds(tiempoInhabilitacionEsquive);
        if(estado != State.ATTACK)
        {
            canDodge = true;
        }
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

    void disableDodge()
    {
        canDodge = false;
    }

    void enableDodge()
    {
        canDodge = true;
    }

    public void changeToAttack()
    {
        estado = State.ATTACK;
        canAttack = false;
        canDodge = false;
    }

    public void changeToParalysed()
    {
        estado = State.PARALYSED;
        canAttack = true;
        canDodge = true;
    }

    public void changeToIdle()
    {
        estado = State.IDLE;
        canAttack = true;
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

    void updateDodgeAnimation(float x, float y)
    {
        animator.SetFloat("Input X", x);
        animator.SetFloat("Input Y", y);
        animator.SetTrigger("Dodge");
    }

}
