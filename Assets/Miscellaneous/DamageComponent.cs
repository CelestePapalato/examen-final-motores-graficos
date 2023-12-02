using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageComponent : MonoBehaviour
{
    [TextArea]
    public string Note = "Este componente debe ir en el objeto que haga daño, no en el ente.";

    [SerializeField]
    GameObject parent;

    [SerializeField]
    int damage;
    [SerializeField]
    float impulse;

    public int getDamage()
    {
        return damage;
    }

    public Vector3 getPosition()
    {
        return (parent) ? parent.transform.position : transform.position;
    }

    public float getImpulse()
    {
        return impulse;
    }
}
