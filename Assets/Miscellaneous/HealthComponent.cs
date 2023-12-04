using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    [SerializeField]
    int maxHealth;
    [SerializeField]
    float invulnerabilityTime;

    int health;
    Collider col;

    Rigidbody rbParent;
    Animator animatorParent;
    ParticleSystem particle;

    bool isPlayer = false;

    void Start()
    {
        rbParent = GetComponentInParent<Rigidbody>();
        animatorParent = GetComponentInParent<Animator>();
        col = GetComponent<Collider>();
        particle = GetComponentInChildren<ParticleSystem>();
        health = maxHealth;

        isPlayer = rbParent.CompareTag("Player");

        //Debug.Log("Objeto: " + rbParent.name + " | Vida: " + health);
    }

    public void damage(int value)
    {
        health = Mathf.Clamp(health - value, 0, maxHealth);

        if(health == 0)
        {
            deadAnimation();
            setInvulnerability(true);
        }
        else
        {
            StartCoroutine(invulnerabilityManager());
        }

        Debug.Log("Objeto: " + rbParent.name + " | Vida: " + health);
    }

    void heal(int value)
    {
        health = Mathf.Clamp(health + value, 0, maxHealth);

        Debug.Log("Objeto: " + rbParent.name + " | Vida: " + health);

        if (particle)
        {
            particle.Emit(1);
        }
    }

    IEnumerator invulnerabilityManager()
    {
        setInvulnerability(true);
        yield return new WaitForSeconds(invulnerabilityTime);
        setInvulnerability(false);
    }

    void setInvulnerability(bool value)
    {
        col.enabled = !value;

        /*
        if (value)
        {
            Debug.Log(rbParent.name + " es invulnerable");
        }
        else
        {
            Debug.Log(rbParent.name + " ha vuelto a ser vulnerable");
        }
        */
    }

    public void changeInvulnerability(bool value, GameObject caller)
    {
        if(caller = rbParent.gameObject)
        {
            setInvulnerability(value);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Heal"))
        {
            if (isPlayer && health < maxHealth && health != 0)
            {
                heal(1);
                Destroy(other.gameObject);
            }
            return;
        }

        DamageComponent enemy = other.gameObject.GetComponent<DamageComponent>();
        if (!enemy)
        {
            return;
        }
        int dmg = enemy.getDamage();
        damage(dmg);
        float impulseMagnitude = enemy.getImpulse();
        damageImpulse(enemy.getPosition(), impulseMagnitude);
        damageAnimation(dmg);
    }

    void damageImpulse(Vector3 origin, float magnitude) {
        if (!rbParent)
        {
            return;
        }
        Vector3 currentPosition = rbParent.transform.position;
        Vector3 impulseVector = currentPosition - origin;
        impulseVector.y = 0;
        impulseVector = impulseVector.normalized;
        rbParent.AddForce(impulseVector * magnitude, ForceMode.Impulse);
    }
    void damageAnimation(int dmg) {
        if (!animatorParent)
        {
            return;
        }
        float dmgBlend = dmg / maxHealth;
        if (!CompareTag("Player")) {
            dmgBlend = Mathf.Clamp(dmgBlend + 0.15f, 0.15f, 1f);
            dmgBlend *= 0.35f;
        }
        animatorParent.SetFloat("DamageAmount", dmgBlend);
        animatorParent.SetTrigger("Damage");
    }

    void deadAnimation()
    {
        if (!animatorParent)
        {
            return;
        }
        animatorParent.SetTrigger("Death");
    }

}
