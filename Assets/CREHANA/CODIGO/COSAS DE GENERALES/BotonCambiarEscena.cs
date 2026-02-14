using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class BotonCambiarEscena : MonoBehaviour
{
    [Header("Configuración de Escena")]
    [Tooltip("Nombre exacto de la escena a cargar (debe estar en Build Settings)")]
    public string nombreEscena;

    [Header("Opciones Adicionales (Opcional)")]
    [Tooltip("Mostrar mensaje de debug al cambiar de escena")]
    public bool mostrarDebug = false;

    [Tooltip("Delay antes de cambiar de escena (en segundos)")]
    [Range(0f, 5f)]
    public float delayAntesDeCambiar = 0f;

    // Referencia al botón
    private Button boton;

    void Start()
    {
        // Obtener el componente Button
        boton = GetComponent<Button>();

        // Suscribirse al evento onClick del botón
        boton.onClick.AddListener(AlHacerClick);

        // Verificar que el nombre de escena no esté vacío
        if (string.IsNullOrEmpty(nombreEscena))
        {
            Debug.LogError($"⚠️ BOTÓN '{gameObject.name}': No has asignado un nombre de escena en el Inspector!");
        }

        if (mostrarDebug)
        {
            Debug.Log($"✅ Botón '{gameObject.name}' configurado para ir a escena: '{nombreEscena}'");
        }
    }

    // Método que se ejecuta al hacer click
    void AlHacerClick()
    {
        if (mostrarDebug)
        {
            Debug.Log($"🎮 Click en botón '{gameObject.name}' - Cargando escena '{nombreEscena}'...");
        }

        // Si hay delay, usar Invoke, si no, cargar directamente
        if (delayAntesDeCambiar > 0)
        {
            Invoke(nameof(CambiarEscena), delayAntesDeCambiar);
        }
        else
        {
            CambiarEscena();
        }
    }

    // Método que cambia la escena
    void CambiarEscena()
    {
        // Verificar que el nombre de escena no esté vacío
        if (string.IsNullOrEmpty(nombreEscena))
        {
            Debug.LogError($"⚠️ ERROR: No se puede cambiar de escena. Asigna un nombre de escena en el Inspector del botón '{gameObject.name}'");
            return;
        }

        // Cargar la escena
        try
        {
            SceneManager.LoadScene(nombreEscena);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"⚠️ ERROR al cargar la escena '{nombreEscena}': {e.Message}\n" +
                          $"Asegúrate de que la escena esté añadida en File → Build Settings → Scenes in Build");
        }
    }

    // Limpiar el evento cuando se destruya el objeto
    void OnDestroy()
    {
        if (boton != null)
        {
            boton.onClick.RemoveListener(AlHacerClick);
        }
    }
}
