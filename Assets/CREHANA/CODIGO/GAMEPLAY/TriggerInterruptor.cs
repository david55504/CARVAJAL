using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Activa y desactiva objetos cuando el jugador entra en el trigger
/// </summary>
[RequireComponent(typeof(Collider))]
public class TriggerInterruptor : MonoBehaviour
{
    [Header("Objetos a Activar")]
    [Tooltip("Lista de objetos que se activarán al entrar")]
    public List<GameObject> objetosActivar = new List<GameObject>();

    [Header("Objetos a Desactivar")]
    [Tooltip("Lista de objetos que se desactivarán al entrar")]
    public List<GameObject> objetosDesactivar = new List<GameObject>();

    [Header("Scripts")]
    [Tooltip("Lista de scripts que se activarán")]
    public List<MonoBehaviour> scriptsActivar = new List<MonoBehaviour>();
    
    [Tooltip("Lista de scripts que se desactivarán")]
    public List<MonoBehaviour> scriptsDesactivar = new List<MonoBehaviour>();

    [Header("Configuración")]
    [Tooltip("Tag del jugador")]
    public string tagJugador = "Player";
    
    [Tooltip("¿Se autodestruye después de activarse?")]
    public bool autodestruible = false;

    [Header("Timer (Opcional)")]
    [Tooltip("Tiempo de espera antes de activar/desactivar objetos (0 = inmediato)")]
    public float tiempoTimer = 0f;
    
    [Tooltip("Objeto del timer (opcional, se activa durante la cuenta regresiva)")]
    public GameObject objetoTimer;

    void Start()
    {
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(tagJugador))
        {
            if (tiempoTimer > 0f)
            {
                // Activar timer y esperar
                if (objetoTimer != null)
                {
                    objetoTimer.SetActive(true);
                }
                StartCoroutine(EsperarTimer());
            }
            else
            {
                // Ejecutar inmediatamente
                EjecutarAccion();
            }
        }
    }

    System.Collections.IEnumerator EsperarTimer()
    {
        yield return new WaitForSeconds(tiempoTimer);

        // Desactivar objeto del timer
        if (objetoTimer != null)
        {
            objetoTimer.SetActive(false);
        }

        // Ejecutar acción
        EjecutarAccion();
    }

    void EjecutarAccion()
    {
        // Activar objetos
        foreach (GameObject obj in objetosActivar)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }

        // Desactivar objetos
        foreach (GameObject obj in objetosDesactivar)
        {
            if (obj != null)
            {
                obj.SetActive(false);
            }
        }

        // Activar scripts
        foreach (MonoBehaviour script in scriptsActivar)
        {
            if (script != null)
            {
                script.enabled = true;
            }
        }

        // Desactivar scripts
        foreach (MonoBehaviour script in scriptsDesactivar)
        {
            if (script != null)
            {
                script.enabled = false;
            }
        }

        // Autodestruirse si está activado
        if (autodestruible)
        {
            Destroy(gameObject);
        }
    }
}