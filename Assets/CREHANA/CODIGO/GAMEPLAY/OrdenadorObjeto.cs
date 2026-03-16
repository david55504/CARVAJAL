using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Cuando el Player entra al trigger, el objeto "desordenado" se anima
/// hacia la posición y rotación del objeto "ordenado" (que está apagado como referencia).
/// Se ejecuta una sola vez y desactiva el collider al terminar.
/// </summary>
public class OrdenadorObjeto : MonoBehaviour
{
    [Header("Objetos a Ordenar")]
    [Tooltip("El objeto visible y desordenado que se animará hacia la posición/rotación correcta.")]
    public Transform objetoDesordenado;

    [Tooltip("El objeto apagado que sirve de referencia de posición y rotación correcta.")]
    public Transform objetoOrdenado;

    [Header("Animación")]
    [Tooltip("Velocidad en grados/unidades por segundo a la que el objeto se mueve y rota hacia su lugar.")]
    public float velocidadAnimacion = 5f;

    [Header("Elementos a Activar")]
    [Tooltip("Lista de GameObjects que se activarán cuando el jugador entre al trigger.")]
    public List<GameObject> elementosAActivar = new List<GameObject>();

    [Header("Configuración")]
    [Tooltip("Tag del jugador para detectar la entrada al trigger.")]
    public string tagJugador = "Player";

    private Collider col;
    private bool accionRealizada = false;

    void Start()
    {
        col = GetComponent<Collider>();

        if (col == null)
            Debug.LogError("⚠️ ORDENADOROBJETO: Este GameObject necesita un Collider con 'Is Trigger' activado.");

        if (objetoDesordenado == null || objetoOrdenado == null)
            Debug.LogError("⚠️ ORDENADOROBJETO: Debes asignar ambos objetos (desordenado y ordenado).");
    }

    void OnTriggerEnter(Collider other)
    {
        if (accionRealizada) return;
        if (!other.CompareTag(tagJugador)) return;

        accionRealizada = true;

        // Activar elementos de la lista
        foreach (GameObject elemento in elementosAActivar)
        {
            if (elemento != null)
                elemento.SetActive(true);
        }

        // Iniciar animación y desactivar collider al terminar
        StartCoroutine(AnimarHaciaOrden());
    }

    IEnumerator AnimarHaciaOrden()
    {
        if (objetoDesordenado == null || objetoOrdenado == null) yield break;

        Vector3 posicionObjetivo = objetoOrdenado.position;
        Quaternion rotacionObjetivo = objetoOrdenado.rotation;

        // Animar hasta que posición y rotación estén lo suficientemente cerca
        while (Vector3.Distance(objetoDesordenado.position, posicionObjetivo) > 0.001f ||
               Quaternion.Angle(objetoDesordenado.rotation, rotacionObjetivo) > 0.1f)
        {
            objetoDesordenado.position = Vector3.MoveTowards(
                objetoDesordenado.position,
                posicionObjetivo,
                velocidadAnimacion * Time.deltaTime
            );

            objetoDesordenado.rotation = Quaternion.RotateTowards(
                objetoDesordenado.rotation,
                rotacionObjetivo,
                velocidadAnimacion * 100f * Time.deltaTime
            );

            yield return null;
        }

        // Fijar valores exactos al terminar
        objetoDesordenado.position = posicionObjetivo;
        objetoDesordenado.rotation = rotacionObjetivo;

        // Desactivar el collider: la acción ya se cumplió
        if (col != null)
            col.enabled = false;

        Debug.Log("✅ ORDENADOROBJETO: Objeto ordenado. Collider desactivado.");
    }
}