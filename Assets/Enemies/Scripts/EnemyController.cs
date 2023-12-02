using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Constantes
    [Header("Combate")]
    [SerializeField]
    float distanciaParaAtacar;
    [SerializeField]
    float tiempoActualizacionPath;
    [SerializeField]
    float anguloEnCamara;
    [SerializeField]
    float cambioVelocidadFueraDeAngulo;

    float velocidad;
    float velocidadAngular;
    float aceleracion;

    // Componentes
    Rigidbody rb;
    Animator animator;
    HealthComponent health;
    NavMeshAgent navMeshAgent;
    WanderController wanderController;
    MeshRenderer meshRenderer;
    Collider col;

    FaceShape faceShape;

    // Estados
    enum State { WANDER, CHASE, ATTACK, PARALYSED, DEAD }
    State estado;

    GameObject enemy;

    Camera cam;

    void Start()
    {
        estado = State.WANDER;
        wanderController = GetComponentInChildren<WanderController>();
        wanderController.enabled = true;
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        animator = GetComponent<Animator>();
        health = GetComponentInChildren<HealthComponent>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        faceShape = GetComponentInChildren<FaceShape>();
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        cam = Camera.main;

        velocidad = navMeshAgent.speed;
        velocidadAngular = navMeshAgent.angularSpeed;
        aceleracion = navMeshAgent.acceleration;
    }

    // Update is called once per frame
    void Update()
    {
        if (estado == State.DEAD)
        {
            return;
        }
        updateMovementAnimationBlend();
        switch (estado)
        {
            case State.CHASE:
                chaseState();
                break;
            case State.ATTACK:
                break;
            case State.PARALYSED:
                break;
        }
    }

    void chaseState()
    {
        
        float distancia = Vector3.Distance(enemy.transform.position, transform.position);
        if (!navMeshAgent.pathPending && distancia <= distanciaParaAtacar && meshRenderer.isVisible)
        {
            updateAttackAnimation();
        }
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0;
        Vector3 forward = transform.forward;
        forward.y = 0;
        float angle = Mathf.Abs(Vector3.SignedAngle(camForward, forward, Vector3.up));
        if(angle < 180f - anguloEnCamara)
        {
            navMeshAgent.speed = velocidad * cambioVelocidadFueraDeAngulo;
            return;
        }
        navMeshAgent.speed = velocidad;        
    }

    IEnumerator actualizarPath()
    {
        while(estado == State.CHASE)
        {
            navMeshAgent.destination = enemy.transform.position;
            yield return new WaitForSeconds(tiempoActualizacionPath);
        }
    }
    public void enemyInRange(GameObject obj)
    {
        if(estado == State.DEAD) {
            return;
        }
        enemy = obj;
        estado = State.CHASE;
        changeToChase();
    }

    public void enemyLeft()
    {
        if (estado == State.DEAD)
        {
            return;
        }
        enemy = null;
        faceShape.happyFace(100);
        StopCoroutine(actualizarPath());
        estado = State.WANDER;
    }

    // ANIMACIONES

    public void changeToParalysed()
    {
        StopCoroutine(actualizarPath());
        estado = State.PARALYSED;
        navMeshAgent.speed = 0;
    }

    public void changeToAttack()
    {
        StopCoroutine(actualizarPath());
        faceShape.angryFace(100);
        estado = State.ATTACK;
        navMeshAgent.speed = 0;
    }

    void changeToChase()
    {
        if (enemy)
        {
            estado = State.CHASE;
            StartCoroutine(actualizarPath());
            wanderController.detenerWander();
            faceShape.angryFace(25);
        }
    }

    public void die()
    {
        StopAllCoroutines();
        estado = State.DEAD;
        navMeshAgent.isStopped = true;
        wanderController.gameObject.SetActive(false);
        col.enabled = false;
    }

    public void endAttack()
    {
        navMeshAgent.speed = velocidad;
        if (enemy)
        {
            changeToChase();
            return;
        }
        faceShape.happyFace(100);
        estado = State.WANDER;
    }

    void updateMovementAnimationBlend()
    {
        float velocityBlend = navMeshAgent.velocity.magnitude / velocidad;
        velocityBlend = Mathf.Clamp(velocityBlend, 0f, 1f);
        animator.SetFloat("Velocity", velocityBlend);
    }

    void updateAttackAnimation()
    {
        animator.SetTrigger("Attack");
    }
}
