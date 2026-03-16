using UnityEngine;

public enum Rol
{
    Ninguno,
    Empresarial,
    Industrial
}

/// <summary>
/// Script estático que guarda el rol del jugador entre escenas.
/// </summary>
public static class RolJugador
{
    public static Rol RolActual { get; set; } = Rol.Ninguno;
}

/// <summary>
/// Cuando el Player entra al trigger asigna el rol configurado.
/// </summary>
public class AsignadorRol : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Rol que se asignará al jugador al entrar en este trigger.")]
    public Rol rolAAsignar = Rol.Empresarial;

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

        RolJugador.RolActual = rolAAsignar;
        Debug.Log($"✅ ROL ASIGNADO: {rolAAsignar}");

        if (col != null) col.enabled = false;
    }
}