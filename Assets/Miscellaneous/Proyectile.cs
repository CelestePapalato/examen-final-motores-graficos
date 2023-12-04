using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Proyectile : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    float maxDistance;
    [SerializeField]
    [Range(0f, 0.3f)]float suavizadoCambioDireccion;

    GameObject objective;
    Vector3 startPosition;
    float distancia;

    Vector3 velocity;
    Vector3 currentVelocity;

    void Start()
    {
        startPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!objective)
        {
            Destroy(gameObject);
        }
        distanciaRecorrida();
        move();
    }

    void move()
    {
        Vector3 newDirection = getDirectionToObjetive();
        velocity = Vector3.SmoothDamp(velocity, newDirection, ref currentVelocity, suavizadoCambioDireccion);
        transform.Translate(velocity * speed * Time.deltaTime);
    }

    void distanciaRecorrida()
    {
        distancia = (transform.position - startPosition).magnitude;
        if(distancia > maxDistance)
        {
            Destroy(gameObject);
        }
    }

    Vector3 getDirectionToObjetive()
    {
        Vector3 newDirection = objective.transform.position - transform.position;
        return newDirection.normalized;
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    public void setVelocity(Vector3 value)
    {
        velocity = value.normalized;
    }

    public void setObjective(GameObject obj)
    {
        objective = obj;
        velocity = getDirectionToObjetive();
    }
}
