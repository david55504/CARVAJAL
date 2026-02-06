using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// ============================================================================
// BOTÓN REINICIAR NIVEL ACTUAL
// ============================================================================
[RequireComponent(typeof(Button))]
public class BotonReiniciar : MonoBehaviour
{
    [Header("Opciones")]
    [Tooltip("Mostrar mensaje de debug")]
    public bool mostrarDebug = false;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Reiniciar);
        
        if (mostrarDebug)
        {
            Debug.Log($"✅ Botón Reiniciar '{gameObject.name}' configurado");
        }
    }

    void Reiniciar()
    {
        string escenaActual = SceneManager.GetActiveScene().name;
        
        if (mostrarDebug)
        {
            Debug.Log($"🔄 Reiniciando escena: {escenaActual}");
        }
        
        SceneManager.LoadScene(escenaActual);
    }
}


// ============================================================================
// BOTÓN SALIR DEL JUEGO
// ============================================================================
[RequireComponent(typeof(Button))]
public class BotonSalir : MonoBehaviour
{
    [Header("Opciones")]
    [Tooltip("Mostrar mensaje de debug")]
    public bool mostrarDebug = false;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Salir);
        
        if (mostrarDebug)
        {
            Debug.Log($"✅ Botón Salir '{gameObject.name}' configurado");
        }
    }

    void Salir()
    {
        if (mostrarDebug)
        {
            Debug.Log("👋 Saliendo del juego...");
        }
        
        #if UNITY_EDITOR
            // En el editor, detener el Play mode
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            // En build, cerrar la aplicación
            Application.Quit();
        #endif
    }
}


// ============================================================================
// BOTÓN SIGUIENTE NIVEL (por índice en Build Settings)
// ============================================================================
[RequireComponent(typeof(Button))]
public class BotonSiguienteNivel : MonoBehaviour
{
    [Header("Opciones")]
    [Tooltip("Mostrar mensaje de debug")]
    public bool mostrarDebug = false;
    
    [Tooltip("Escena a cargar si no hay siguiente nivel (opcional)")]
    public string escenaSiNoHaySiguiente = "";

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(SiguienteNivel);
        
        if (mostrarDebug)
        {
            Debug.Log($"✅ Botón Siguiente Nivel '{gameObject.name}' configurado");
        }
    }

    void SiguienteNivel()
    {
        int nivelActual = SceneManager.GetActiveScene().buildIndex;
        int totalEscenas = SceneManager.sceneCountInBuildSettings;
        
        if (mostrarDebug)
        {
            Debug.Log($"📊 Nivel actual: {nivelActual} | Total escenas: {totalEscenas}");
        }
        
        // Verificar si hay siguiente nivel
        if (nivelActual + 1 < totalEscenas)
        {
            if (mostrarDebug)
            {
                Debug.Log($"➡️ Cargando siguiente nivel (índice {nivelActual + 1})");
            }
            SceneManager.LoadScene(nivelActual + 1);
        }
        else
        {
            // No hay siguiente nivel
            if (!string.IsNullOrEmpty(escenaSiNoHaySiguiente))
            {
                if (mostrarDebug)
                {
                    Debug.Log($"🏁 Último nivel alcanzado. Cargando: {escenaSiNoHaySiguiente}");
                }
                SceneManager.LoadScene(escenaSiNoHaySiguiente);
            }
            else
            {
                if (mostrarDebug)
                {
                    Debug.LogWarning("⚠️ No hay siguiente nivel y no se configuró escena alternativa");
                }
            }
        }
    }
}


// ============================================================================
// BOTÓN NIVEL ANTERIOR (por índice en Build Settings)
// ============================================================================
[RequireComponent(typeof(Button))]
public class BotonNivelAnterior : MonoBehaviour
{
    [Header("Opciones")]
    [Tooltip("Mostrar mensaje de debug")]
    public bool mostrarDebug = false;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(NivelAnterior);
        
        if (mostrarDebug)
        {
            Debug.Log($"✅ Botón Nivel Anterior '{gameObject.name}' configurado");
        }
    }

    void NivelAnterior()
    {
        int nivelActual = SceneManager.GetActiveScene().buildIndex;
        
        if (mostrarDebug)
        {
            Debug.Log($"📊 Nivel actual: {nivelActual}");
        }
        
        // Verificar que no sea la primera escena
        if (nivelActual > 0)
        {
            if (mostrarDebug)
            {
                Debug.Log($"⬅️ Cargando nivel anterior (índice {nivelActual - 1})");
            }
            SceneManager.LoadScene(nivelActual - 1);
        }
        else
        {
            if (mostrarDebug)
            {
                Debug.LogWarning("⚠️ Ya estás en la primera escena");
            }
        }
    }
}


// ============================================================================
// BOTÓN ACTIVAR/DESACTIVAR PANEL
// ============================================================================
[RequireComponent(typeof(Button))]
public class BotonTogglePanel : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Panel a activar/desactivar")]
    public GameObject panel;
    
    [Header("Opciones")]
    [Tooltip("Mostrar mensaje de debug")]
    public bool mostrarDebug = false;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(TogglePanel);
        
        if (panel == null)
        {
            Debug.LogError($"⚠️ Botón '{gameObject.name}': No se ha asignado un panel!");
        }
        
        if (mostrarDebug)
        {
            Debug.Log($"✅ Botón Toggle Panel '{gameObject.name}' configurado");
        }
    }

    void TogglePanel()
    {
        if (panel != null)
        {
            bool nuevoEstado = !panel.activeSelf;
            panel.SetActive(nuevoEstado);
            
            if (mostrarDebug)
            {
                Debug.Log($"🔄 Panel '{panel.name}' ahora está: {(nuevoEstado ? "ACTIVO" : "INACTIVO")}");
            }
        }
    }
}


// ============================================================================
// BOTÓN PAUSAR/REANUDAR JUEGO
// ============================================================================
[RequireComponent(typeof(Button))]
public class BotonPausa : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Panel de pausa (se activa/desactiva automáticamente)")]
    public GameObject panelPausa;
    
    [Header("Opciones")]
    [Tooltip("Mostrar mensaje de debug")]
    public bool mostrarDebug = false;

    private bool juegoPausado = false;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(TogglePausa);
        
        // Asegurarse de que el panel esté desactivado al inicio
        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }
        
        if (mostrarDebug)
        {
            Debug.Log($"✅ Botón Pausa '{gameObject.name}' configurado");
        }
    }

    void TogglePausa()
    {
        juegoPausado = !juegoPausado;
        
        if (juegoPausado)
        {
            Pausar();
        }
        else
        {
            Reanudar();
        }
    }

    void Pausar()
    {
        Time.timeScale = 0f; // Detener el tiempo
        
        if (panelPausa != null)
        {
            panelPausa.SetActive(true);
        }
        
        if (mostrarDebug)
        {
            Debug.Log("⏸️ Juego PAUSADO");
        }
    }

    void Reanudar()
    {
        Time.timeScale = 1f; // Restaurar el tiempo
        
        if (panelPausa != null)
        {
            panelPausa.SetActive(false);
        }
        
        if (mostrarDebug)
        {
            Debug.Log("▶️ Juego REANUDADO");
        }
    }
}
