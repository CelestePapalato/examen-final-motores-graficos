using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    // Constantes

    enum Type { FISICO, DISTANCIA}
    [SerializeField]
    [Tooltip("Indica si el enemigo ataca cuerpo a cuerpo o a distancia")]
    Type tipoDeEnemigo;

    [Header("Combate")]
    [SerializeField]
    [Tooltip("Ángulo en el que debe encontrarse el enemigo para atacar y moverse a velocidad normal")]
    float anguloEnCamara;
    [SerializeField]
    [Tooltip("Modificador de velocidad para cuando el enemigo está fuera de ángulo")]
    float cambioVelocidadFueraDeAngulo;

    [Header("Enemigo de ataques físicos")]
    [SerializeField]
    float tiempoActualizacionPath;
    [SerializeField]
    float distanciaParaAtacar;

    [Header("Enemigo de ataques a distancia")]
    [SerializeField]
    [Tooltip("A partir de qué distancia con el jugador el enemigo debe cambiar de posición")]
    float distanciaMinConElJugador;
    [SerializeField]
    float frecuenciaAtaque;

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
    enum State { WANDER, CHASE, ATTACK, PARALYSED, ESCAPE, DEAD }
    State estado;

    GameObject enemy;

    Camera cam;

    bool canAttack = true;

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
            case State.ESCAPE:
                escapeState();
                break;
        }
    }

    void chaseState()
    {        
        float distancia = Vector3.Distance(enemy.transform.position, transform.position);
        if (tipoDeEnemigo == Type.FISICO)
        {
            if (!navMeshAgent.pathPending && distancia <= distanciaParaAtacar && meshRenderer.isVisible)
            {
                updateAttackAnimation();
            }
            
            if (dentroDeAngulo())
            {
                navMeshAgent.speed = velocidad * cambioVelocidadFueraDeAngulo;
                return;
            }
            navMeshAgent.speed = velocidad;
        }
        if(tipoDeEnemigo == Type.DISTANCIA)
        {
            if (distancia <= distanciaMinConElJugador)
            {
                estado = State.ESCAPE;
                navMeshAgent.speed = velocidad;
                CancelInvoke("ataqueDistancia");
                canAttack = true;
                return;
            }
            if (navMeshAgent.remainingDistance < 0.5f)
            {
                lookAt();
                if (canAttack)
                {
                    canAttack = false;
                    ataqueDistancia();
                }            
            }
        }
    }

    void escapeState()
    {
        if (navMeshAgent.pathPending)
        {
            return;
        }

        float distancia = Vector3.Distance(enemy.transform.position, transform.position);

        if (distancia > distanciaMinConElJugador)
        {
            changeToChase();
        }
        else if (navMeshAgent.remainingDistance < 0.5f)
        {
            wanderController.nextPoint();
        }
        
    }

    bool dentroDeAngulo()
    {
        Vector3 camForward = cam.transform.forward;
        camForward.y = 0;
        Vector3 forward = transform.forward;
        forward.y = 0;
        float angle = Mathf.Abs(Vector3.SignedAngle(camForward, forward, Vector3.up));
        return angle < 180f - anguloEnCamara;
    }

    void ataqueDistancia()
    {
        updateAttackAnimation();
        Invoke("ataqueDistancia", frecuenciaAtaque);
    }

    void lookAt()
    {
        Quaternion rotacionActual = transform.rotation;
        Vector3 pos = enemy.transform.position;
        pos.y = transform.position.y;
        transform.LookAt(pos);
        Quaternion rotacionObjetivo = transform.rotation;
        transform.rotation = Quaternion.Slerp(rotacionActual, rotacionObjetivo, 1.4f * Time.deltaTime);
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
        CancelInvoke("ataqueDistancia");
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
        if(tipoDeEnemigo == Type.DISTANCIA)
        {
            GameObject obj = SpawnManager.instance.spawnObject("magic", transform.position + new Vector3(0f, 0.7f, 0f));
            Projectile proyectile = obj.GetComponent<Projectile>();
            proyectile.setObjective(enemy);
        }
    }

    void changeToChase()
    {
        if (enemy)
        {
            estado = State.CHASE;
            if (tipoDeEnemigo == Type.FISICO)
            {
                StartCoroutine(actualizarPath());
            }
            wanderController.detenerWander();
            faceShape.angryFace(25);
        }
    }

    public void die()
    {
        StopAllCoroutines();
        CancelInvoke("ataqueDistancia");
        estado = State.DEAD;
        navMeshAgent.isStopped = true;
        wanderController.gameObject.SetActive(false);
        col.enabled = false;
        GameManager.instance.enemigoDerrotado();
        Destroy(gameObject, 5);
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
