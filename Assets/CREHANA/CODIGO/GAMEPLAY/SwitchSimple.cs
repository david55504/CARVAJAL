using System.Collections.Generic;
using UnityEngine;

public class SwitchSimple : MonoBehaviour
{
    [Header("Listas de Objetos")]
    [Tooltip("Objetos que se PRENDEN cuando el Player entra al trigger.")]
    public List<GameObject> objetosAPrender = new List<GameObject>();

    [Tooltip("Objetos que se APAGAN cuando el Player entra al trigger.")]
    public List<GameObject> objetosAApagar = new List<GameObject>();

    [Header("Configuración")]
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

        foreach (GameObject obj in objetosAPrender)
            if (obj != null) obj.SetActive(true);

        foreach (GameObject obj in objetosAApagar)
            if (obj != null) obj.SetActive(false);

        if (col != null) col.enabled = false;
    }
}