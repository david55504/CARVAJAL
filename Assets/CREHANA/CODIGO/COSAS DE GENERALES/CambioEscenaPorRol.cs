using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Ponlo en el mismo objeto que el botón.
/// Se conecta automáticamente al OnClick sin necesidad de configurar nada en el Canvas.
/// </summary>
public class CambioEscenaPorRol : MonoBehaviour
{
    [Header("Escenas")]
    [Tooltip("Nombre exacto de la escena del mundo empresarial (debe estar en Build Settings).")]
    public string escenaEmpresarial;

    [Tooltip("Nombre exacto de la escena del mundo industrial (debe estar en Build Settings).")]
    public string escenaIndustrial;

    void Awake()
    {
        // Conectarse automáticamente al botón del mismo objeto
        Button boton = GetComponent<Button>();
        if (boton != null)
            boton.onClick.AddListener(IrAEscena);
        else
            Debug.LogError("⚠️ CAMBIOESCENA: No hay componente Button en este objeto.");
    }

    void IrAEscena()
    {
        switch (RolJugador.RolActual)
        {
            case Rol.Empresarial:
                CargarEscena(escenaEmpresarial);
                break;
            case Rol.Industrial:
                CargarEscena(escenaIndustrial);
                break;
            default:
                Debug.LogWarning("⚠️ CAMBIOESCENA: No hay rol asignado.");
                break;
        }
    }

    void CargarEscena(string nombreEscena)
    {
        if (string.IsNullOrEmpty(nombreEscena))
        {
            Debug.LogError("⚠️ CAMBIOESCENA: El nombre de la escena está vacío.");
            return;
        }

        Debug.Log($"➡️ Cargando escena '{nombreEscena}' para rol {RolJugador.RolActual}");
        SceneManager.LoadScene(nombreEscena);
    }
}