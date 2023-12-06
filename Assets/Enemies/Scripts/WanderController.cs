using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WanderController : MonoBehaviour
{
    [Header("Configuraci�n de estado vigilancia")]
    [SerializeField]
    [Tooltip("Los hijos de este objeto ser�n los puntos por los que vigilar� el personaje")]
    Transform puntosDeVigilancia;

    List<Transform> _puntosDeVigilancia = new List<Transform>();
    [SerializeField]
    float tiempoMinParaReposicionarse;
    [SerializeField]
    float tiempoMaxParaReposicionarse;
    [SerializeField]
    float distanciaMin;
    [SerializeField]
    float distanciaMax;
    [SerializeField]
    bool randomizarDestino = false;

    Vector3 destino = Vector3.zero;

    EnemyController father;
    NavMeshAgent navMeshAgent;

    int currentPoint = 0;
    float remainingDistanceExpected;

    enum State { WAIT, MOVE }
    State estado = State.WAIT;

    void Start()
    {
        father = GetComponentInParent<EnemyController>();
        navMeshAgent = GetComponentInParent<NavMeshAgent>();
        generarPuntosDeVigilancia();
        empezarWander();
    }

    // Update is called once per frame
    void Update()
    {
        if(estado == State.WAIT)
        {
            return;
        }
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= remainingDistanceExpected)
        {
            empezarWander();
        }        
    }

    IEnumerator waitState()
    {
        navMeshAgent.destination = transform.position;
        estado = State.WAIT;
        float time = Random.Range(tiempoMinParaReposicionarse, tiempoMaxParaReposicionarse);
        yield return new WaitForSeconds(time);
        if (randomizarDestino)
        {
            randomizePoint();
        }
        else
        {
            calcularDestino();
        }
        remainingDistanceExpected = Random.Range(distanciaMin, distanciaMax);
        estado = State.MOVE;
    }

    void randomizePoint()
    {
        currentPoint = Random.Range(0, _puntosDeVigilancia.Count);
        navMeshAgent.destination = _puntosDeVigilancia[currentPoint].position;
    }

    void calcularDestino()
    {
        nextPoint();
    }
    
    public void nextPoint()
    {
        currentPoint++;
        if (currentPoint >= _puntosDeVigilancia.Count)
        {
            currentPoint = 0;
        }
        navMeshAgent.destination = _puntosDeVigilancia[currentPoint].position;
    }

    public void detenerWander()
    {
        estado = State.WAIT;
        StopAllCoroutines();
    }

    void empezarWander()
    {
        StartCoroutine(waitState());
    }

    void generarPuntosDeVigilancia()
    {
        foreach(Transform child in puntosDeVigilancia)
        {
            if(child != puntosDeVigilancia)
            {
                _puntosDeVigilancia.Add(child);
            }
        }
    }

    public List<Transform> getPuntos()
    {
        return _puntosDeVigilancia;
    }

    private void OnTriggerEnter(Collider other)
    {
        detenerWander();
        father.enemyInRange(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        father.enemyLeft();
        if(Random.Range(0, 2) == 0)
        {
            empezarWander();
        }
        else
        {
            calcularDestino();
        }
    }

}
