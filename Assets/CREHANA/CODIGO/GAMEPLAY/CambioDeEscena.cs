using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Carga una nueva escena cuando el jugador entra al trigger.
/// Asigna este script al GameObject que tenga el Collider trigger.
/// </summary>
public class CambioDeEscena : MonoBehaviour
{
    [Tooltip("Nombre exacto de la escena a cargar (debe estar añadida en File > Build Settings)")]
    public string nombreEscena;

    [Tooltip("Tag que debe tener el objeto para activar el cambio de escena")]
    public string tagJugador = "Player";

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagJugador)) return;

        if (string.IsNullOrEmpty(nombreEscena))
        {
            Debug.LogError("⚠️ CAMBIODEESCENA: El nombre de la escena está vacío.");
            return;
        }

        SceneManager.LoadScene(nombreEscena);
    }
}