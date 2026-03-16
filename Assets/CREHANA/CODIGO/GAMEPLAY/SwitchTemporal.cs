using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTemporal : MonoBehaviour
{
    [Header("Listas de Objetos")]
    [Tooltip("Objetos que se APAGAN al entrar y se PRENDEN al terminar el tiempo.")]
    public List<GameObject> objetosAApagar = new List<GameObject>();

    [Tooltip("Objetos que se PRENDEN al entrar y se APAGAN al terminar el tiempo.")]
    public List<GameObject> objetosAPrender = new List<GameObject>();

    [Header("Configuración")]
    [Tooltip("Duración en segundos antes de revertir el switch.")]
    public float duracion = 5f;

    [Tooltip("Tag del jugador.")]
    public string tagJugador = "Player";

    private Collider col;

    void Start()
    {
        col = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagJugador)) return;

        // Apagar y prender listas
        foreach (GameObject obj in objetosAApagar)
            if (obj != null) obj.SetActive(false);

        foreach (GameObject obj in objetosAPrender)
            if (obj != null) obj.SetActive(true);

        // Desactivar collider mientras el timer corre para evitar retrigers
        if (col != null) col.enabled = false;

        StartCoroutine(Revertir());
    }

    IEnumerator Revertir()
    {
        yield return new WaitForSeconds(duracion);

        // Revertir al estado original
        foreach (GameObject obj in objetosAApagar)
            if (obj != null) obj.SetActive(true);

        foreach (GameObject obj in objetosAPrender)
            if (obj != null) obj.SetActive(false);

    }
}