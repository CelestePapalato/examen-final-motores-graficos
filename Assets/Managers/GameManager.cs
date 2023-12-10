using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField]
    TMP_Text textoContador;
    [SerializeField]
    TMP_Text textoEnemigosDerrotados;
    [SerializeField]
    Canvas perder;
    [SerializeField]
    Canvas ganar;

    int cantEnemigos;
    int cantEnemigosDerrotados = 0;


    void Start()
    {
        instance = this;
        cantEnemigos = GameObject.FindGameObjectsWithTag("Enemy").Length;
        updateContador();
        perder.enabled = false;
        ganar.enabled = false;
    }
        
    void updateContador()
    {
        textoContador.text = cantEnemigosDerrotados + " / " + cantEnemigos;
    }

    void cumplioCondicion()
    {
        if(cantEnemigosDerrotados >= cantEnemigos)
        {
            Time.timeScale = 0;
            textoContador.enabled = false;
            ganar.enabled = true;
        }
    }

    public void irAlMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void enemigoDerrotado()
    {
        cantEnemigosDerrotados++;
        updateContador();
        cumplioCondicion();
    }

    public void jugadorPerdio()
    {
        Time.timeScale = 0;
        textoContador.enabled = false;
        textoEnemigosDerrotados.text = "" + cantEnemigosDerrotados;
        perder.enabled = true;
    }
}
