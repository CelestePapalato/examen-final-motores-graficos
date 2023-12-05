using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField]
    float speed;
    [SerializeField]
    float lifeTime;
    [SerializeField]
    [Range(0f, 0.3f)]float suavizadoCambioDireccion;

    GameObject objective;

    Vector3 velocity;
    Vector3 currentVelocity;

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        if (!objective)
        {
            Destroy(gameObject);
        }
        move();
    }

    void move()
    {
        transform.LookAt(objective.transform);
        velocity = Vector3.SmoothDamp(velocity, transform.forward, ref currentVelocity, suavizadoCambioDireccion);
        velocity.y = 0;
        transform.Translate(velocity * speed * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    public void setObjective(GameObject obj)
    {
        if(obj == null)
        {
            Destroy(gameObject);
            return;
        }
        objective = obj;
        transform.LookAt(objective.transform);
        velocity = transform.forward;
    }
}
