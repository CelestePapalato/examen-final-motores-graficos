using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangePlayerOpacity : MonoBehaviour
{
    [SerializeField]
    float minDistance = 3f;

    GameObject player;
    SkinnedMeshRenderer skinnedMeshRenderer;

    float currentLerp = 1f;

    private void Start()
    {
        player = GameObject.FindWithTag("Player");
        skinnedMeshRenderer = player.GetComponentInChildren<SkinnedMeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        reducePlayerOpacity();
    }

    float distanceToPlayer()
    {
        Vector3 distance = player.transform.position - transform.position;

        Debug.Log(distance.magnitude);
        return distance.magnitude;
    }

    void reducePlayerOpacity()
    {
        float t = distanceToPlayer() / minDistance;
        t = Mathf.Clamp(t, 0f, 1f);
        setPlayerOpacity(t);
    }

    void setPlayerOpacity(float t)
    {
        if(t == currentLerp)
        {
            return;
        }
        Color col = skinnedMeshRenderer.material.color;
        col.a = Mathf.Lerp(255f, 0f, t);
        skinnedMeshRenderer.material.color = col;
    }
}
