using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Activa objetos cuando el jugador entra en el trigger
/// Puede ser evento único (se autodestruye) o reutilizable
/// </summary>
[RequireComponent(typeof(Collider))]
public class TriggerActivadorObjetos : MonoBehaviour
{
    [Header("Objetos a Activar")]
    [Tooltip("Lista de objetos que se activarán al entrar en el trigger")]
    public List<GameObject> objetosAActivar = new List<GameObject>();

    [Header("Configuración")]
    [Tooltip("¿Se autodestruye después de la primera activación?")]
    public bool eventoUnico = true;

    [Tooltip("Tag del jugador (por defecto: Player)")]
    public string tagJugador = "Player";

    private bool yaActivado = false;

    void Start()
    {
        // Verificar que el collider es un trigger
        Collider col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            Debug.LogWarning($"⚠️ El Collider en '{gameObject.name}' no está marcado como Trigger. Se activará automáticamente.");
            col.isTrigger = true;
        }

        // Verificar que hay objetos en la lista
        if (objetosAActivar.Count == 0)
        {
            Debug.LogWarning($"⚠️ TriggerActivadorObjetos en '{gameObject.name}': No hay objetos asignados para activar.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        // Verificar que es el jugador
        if (other.CompareTag(tagJugador))
        {
            // Si es evento único y ya se activó, no hacer nada
            if (eventoUnico && yaActivado)
                return;

            // Activar todos los objetos de la lista
            ActivarObjetos();

            // Marcar como activado
            yaActivado = true;

            // Si es evento único, autodestruirse
            if (eventoUnico)
            {
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// Activa todos los objetos de la lista
    /// </summary>
    void ActivarObjetos()
    {
        foreach (GameObject obj in objetosAActivar)
        {
            if (obj != null)
            {
                obj.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Método público para resetear el trigger (útil si no es evento único)
    /// </summary>
    public void ResetearTrigger()
    {
        yaActivado = false;
    }
}