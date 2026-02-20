using UnityEngine;

/// <summary>
/// Trigger de un solo uso - se desactiva después de que el jugador lo atraviesa
/// </summary>
[RequireComponent(typeof(Collider))]
public class TriggerUnaVez : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Tag del jugador")]
    public string tagJugador = "Player";

    private Collider col;
    private bool yaUsado = false;

    void Start()
    {
        col = GetComponent<Collider>();
        if (col != null && !col.isTrigger)
        {
            col.isTrigger = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!yaUsado && other.CompareTag(tagJugador))
        {
            yaUsado = true;
            
            if (col != null)
            {
                col.enabled = false;
            }
        }
    }
}