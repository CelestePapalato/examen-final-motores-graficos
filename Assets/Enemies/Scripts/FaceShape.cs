using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FaceShape : MonoBehaviour
{
    [SerializeField]
    SkinnedMeshRenderer meshCabeza;

    [Header("Modificador blend shapes")]
    [SerializeField]
    float tiempoHappy;
    [SerializeField]
    float tiempoAngry;
    [SerializeField]
    float tiempoNone;

    float time = 0;

    bool active = false;

    float startAngry;
    float startHappy;

    float endAngry;
    float endHappy;

    enum Face { NONE, HAPPY, ANGRY}
    Face actual = Face.NONE;
    void Update()
    {
        if (!active)
        {
            return;
        }

        time += Time.deltaTime;
        switch (actual)
        {
            case Face.NONE:
                modifyBlendShapes(0f, 0f);
                break;
            case Face.ANGRY:
                modifyBlendShapes(endAngry, 0f);
                break;
            case Face.HAPPY:
                modifyBlendShapes(0f, endHappy);
                break;
        }
    }

    void modifyBlendShapes(float angry, float happy)
    {
        float t = Mathf.Clamp(time / tiempoNone, 0f, 1f);
        float current = meshCabeza.GetBlendShapeWeight(0);
        meshCabeza.SetBlendShapeWeight(0, Mathf.Lerp(startAngry, angry, t));
        current = meshCabeza.GetBlendShapeWeight(1);
        meshCabeza.SetBlendShapeWeight(1, Mathf.Lerp(startHappy, happy, t));
        if (t == 1f)
        {
            active = false;
        }
    }

    public void happyFace(float value)
    {
        endHappy = Mathf.Clamp(value, 0f, 100f);

        if(actual == Face.HAPPY)
        {
            return;
        }
        startAngry = meshCabeza.GetBlendShapeWeight(0);
        startHappy = meshCabeza.GetBlendShapeWeight(1);
        time = 0;
        actual = Face.HAPPY;
        active = true;
    }

    public void angryFace(float value)
    {
        endAngry = Mathf.Clamp(value, 0f, 100f);

        if (actual == Face.ANGRY)
        {
            return;
        }
        startAngry = meshCabeza.GetBlendShapeWeight(0);
        startHappy = meshCabeza.GetBlendShapeWeight(1);
        time = 0;
        actual = Face.ANGRY;
        active = true;
    }

    public void noneFace()
    {
        if (actual == Face.NONE)
        {
            return;
        }
        startAngry = meshCabeza.GetBlendShapeWeight(0);
        startHappy = meshCabeza.GetBlendShapeWeight(1);
        time = 0;
        actual = Face.NONE;
        active = true;
    }
}
