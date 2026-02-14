using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Sistema de ajustes gráficos para juego móvil con URP
/// Controla: Post-Processing, SSAO, Niebla, Render Scale y persistencia
/// Proyecto: Carvajal - Diorama 3D Cartoon/Pixar
/// </summary>
public class SettingsManager : MonoBehaviour
{
    [Header("Referencias de UI")]
    [Tooltip("Toggle principal de calidad gráfica")]
    public Toggle toggleCalidad;
    
    [Tooltip("Texto que muestra 'Alta calidad' o 'Calidad rápida'")]
    public TextMeshProUGUI textoCalidad;

    [Header("Configuración de Calidad")]
    [Tooltip("URP Asset para Alta Calidad (con SSAO activado)")]
    public UniversalRenderPipelineAsset urpAssetAltaCalidad;
    
    [Tooltip("URP Asset para Calidad Rápida (sin SSAO)")]
    public UniversalRenderPipelineAsset urpAssetCalidadRapida;

    [Header("Textos de UI")]
    [SerializeField] private string textoAltaCalidad = "Alta calidad";
    [SerializeField] private string textoCalidadRapida = "Calidad rápida";

    [Header("Debug")]
    [Tooltip("Mostrar logs de información")]
    public bool mostrarDebug = true;

    // Referencias privadas
    private Volume globalVolume;
    private const string PLAYER_PREF_CALIDAD = "ConfiguracionCalidadGrafica";

    void Start()
    {
        // Buscar el Global Volume en la escena
        BuscarGlobalVolume();

        // Verificar que los URP Assets estén asignados
        VerificarAssets();

        // Cargar configuración guardada
        CargarConfiguracion();

        // Suscribirse al evento del Toggle
        if (toggleCalidad != null)
        {
            toggleCalidad.onValueChanged.AddListener(AlCambiarToggle);
        }
        else
        {
            Debug.LogError("⚠️ SETTINGSMANAGER: No se ha asignado el Toggle en el Inspector!");
        }

        // Aplicar configuración inicial
        AplicarConfiguracion(toggleCalidad.isOn);
    }

    /// <summary>
    /// Busca el objeto "Global Volume" en la escena
    /// </summary>
    void BuscarGlobalVolume()
    {
        GameObject volumeObj = GameObject.Find("Global Volume");
        
        if (volumeObj != null)
        {
            globalVolume = volumeObj.GetComponent<Volume>();
            
            if (globalVolume != null)
            {
                if (mostrarDebug)
                    Debug.Log($"✅ Global Volume encontrado: {volumeObj.name}");
            }
            else
            {
                Debug.LogError("⚠️ El objeto 'Global Volume' no tiene componente Volume!");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No se encontró un objeto llamado 'Global Volume' en la escena.");
        }
    }

    /// <summary>
    /// Verifica que los URP Assets estén asignados correctamente
    /// </summary>
    void VerificarAssets()
    {
        if (urpAssetAltaCalidad == null)
        {
            Debug.LogError("⚠️ SETTINGSMANAGER: No se ha asignado el URP Asset de Alta Calidad!");
        }

        if (urpAssetCalidadRapida == null)
        {
            Debug.LogError("⚠️ SETTINGSMANAGER: No se ha asignado el URP Asset de Calidad Rápida!");
        }

        if (mostrarDebug && urpAssetAltaCalidad != null && urpAssetCalidadRapida != null)
        {
            Debug.Log("✅ URP Assets configurados correctamente");
        }
    }

    /// <summary>
    /// Se ejecuta cuando el usuario cambia el Toggle
    /// </summary>
    void AlCambiarToggle(bool altaCalidad)
    {
        if (mostrarDebug)
        {
            Debug.Log($"🎮 Usuario cambió calidad a: {(altaCalidad ? "ALTA" : "RÁPIDA")}");
        }

        AplicarConfiguracion(altaCalidad);
        GuardarConfiguracion(altaCalidad);
    }

    /// <summary>
    /// Aplica todos los ajustes gráficos según la configuración elegida
    /// </summary>
    void AplicarConfiguracion(bool altaCalidad)
    {
        // 1. Actualizar texto del Toggle
        ActualizarTextoUI(altaCalidad);

        // 2. Configurar Post-Processing (Global Volume)
        ConfigurarPostProcessing(altaCalidad);

        // 3. Configurar SSAO (cambiando URP Asset)
        ConfigurarSSAO(altaCalidad);

        // 4. Configurar Niebla
        ConfigurarNiebla(altaCalidad);

        // 5. Configurar Render Scale
        ConfigurarRenderScale(altaCalidad);

        if (mostrarDebug)
        {
            Debug.Log($"✅ Configuración aplicada: {(altaCalidad ? "ALTA CALIDAD" : "CALIDAD RÁPIDA")}");
        }
    }

    /// <summary>
    /// Actualiza el texto del UI según el estado
    /// </summary>
    void ActualizarTextoUI(bool altaCalidad)
    {
        if (textoCalidad != null)
        {
            textoCalidad.text = altaCalidad ? textoAltaCalidad : textoCalidadRapida;
        }
    }

    /// <summary>
    /// Configura el peso del Global Volume para activar/desactivar Post-Processing
    /// </summary>
    void ConfigurarPostProcessing(bool altaCalidad)
    {
        if (globalVolume != null)
        {
            // Alta Calidad: Weight = 1 (Post-Processing completo)
            // Calidad Rápida: Weight = 0 (Sin Post-Processing)
            globalVolume.weight = altaCalidad ? 1f : 0f;

            if (mostrarDebug)
            {
                Debug.Log($"📊 Post-Processing weight: {globalVolume.weight}");
            }
        }
    }

    /// <summary>
    /// Configura SSAO alternando entre dos URP Assets
    /// (Alta Calidad: con SSAO, Calidad Rápida: sin SSAO)
    /// </summary>
    void ConfigurarSSAO(bool altaCalidad)
    {
        if (urpAssetAltaCalidad == null || urpAssetCalidadRapida == null)
        {
            Debug.LogWarning("⚠️ No se pueden cambiar los URP Assets porque no están asignados");
            return;
        }

        // Seleccionar el asset apropiado
        UniversalRenderPipelineAsset assetAUsar = altaCalidad ? urpAssetAltaCalidad : urpAssetCalidadRapida;

        // Cambiar el URP Asset activo
        QualitySettings.renderPipeline = assetAUsar;

        if (mostrarDebug)
        {
            Debug.Log($"🎨 URP Asset cambiado a: {assetAUsar.name} (SSAO: {(altaCalidad ? "ON" : "OFF")})");
        }
    }

    /// <summary>
    /// Activa/desactiva la niebla global de la escena
    /// </summary>
    void ConfigurarNiebla(bool altaCalidad)
    {
        // Alta Calidad: Niebla activada
        // Calidad Rápida: Niebla desactivada (mejor rendimiento)
        RenderSettings.fog = altaCalidad;

        if (mostrarDebug)
        {
            Debug.Log($"🌫️ Niebla (Fog): {(RenderSettings.fog ? "ON" : "OFF")}");
        }
    }

    /// <summary>
    /// Configura el Render Scale del URP Asset
    /// </summary>
    void ConfigurarRenderScale(bool altaCalidad)
    {
        // Obtener el asset activo actual
        UniversalRenderPipelineAsset urpAsset = QualitySettings.renderPipeline as UniversalRenderPipelineAsset;

        if (urpAsset != null)
        {
            // Alta Calidad: Render Scale 1.0 (resolución completa)
            // Calidad Rápida: Render Scale 0.8 (80% de resolución, mejor rendimiento)
            float nuevoRenderScale = altaCalidad ? 1.0f : 0.8f;
            urpAsset.renderScale = nuevoRenderScale;

            if (mostrarDebug)
            {
                Debug.Log($"🖥️ Render Scale: {nuevoRenderScale} ({nuevoRenderScale * 100}%)");
            }
        }
        else
        {
            Debug.LogWarning("⚠️ No se pudo obtener el URP Asset activo para configurar Render Scale");
        }
    }

    /// <summary>
    /// Guarda la configuración del usuario en PlayerPrefs
    /// </summary>
    void GuardarConfiguracion(bool altaCalidad)
    {
        // Guardar como 1 (Alta) o 0 (Rápida)
        PlayerPrefs.SetInt(PLAYER_PREF_CALIDAD, altaCalidad ? 1 : 0);
        PlayerPrefs.Save();

        if (mostrarDebug)
        {
            Debug.Log($"💾 Configuración guardada: {(altaCalidad ? "Alta" : "Rápida")}");
        }
    }

    /// <summary>
    /// Carga la configuración guardada o usa valores por defecto
    /// </summary>
    void CargarConfiguracion()
    {
        // Por defecto: Alta Calidad (1)
        int calidadGuardada = PlayerPrefs.GetInt(PLAYER_PREF_CALIDAD, 1);
        bool altaCalidad = calidadGuardada == 1;

        // Aplicar al Toggle
        if (toggleCalidad != null)
        {
            toggleCalidad.isOn = altaCalidad;
        }

        if (mostrarDebug)
        {
            Debug.Log($"📂 Configuración cargada: {(altaCalidad ? "Alta Calidad" : "Calidad Rápida")}");
        }
    }

    /// <summary>
    /// Método público para cambiar la calidad desde código
    /// </summary>
    public void CambiarCalidad(bool altaCalidad)
    {
        if (toggleCalidad != null)
        {
            toggleCalidad.isOn = altaCalidad;
        }
        else
        {
            // Si no hay toggle, aplicar directamente
            AplicarConfiguracion(altaCalidad);
            GuardarConfiguracion(altaCalidad);
        }
    }

    /// <summary>
    /// Método público para obtener la configuración actual
    /// </summary>
    public bool EsAltaCalidad()
    {
        return toggleCalidad != null ? toggleCalidad.isOn : true;
    }

    void OnDestroy()
    {
        // Desuscribirse del evento del Toggle
        if (toggleCalidad != null)
        {
            toggleCalidad.onValueChanged.RemoveListener(AlCambiarToggle);
        }
    }
}