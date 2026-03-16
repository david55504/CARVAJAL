using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mueve un objeto a través de una lista de waypoints.
/// Permite configurar qué eje del modelo apunta hacia adelante y hacia arriba,
/// para que la rotación sea correcta sin importar cómo esté orientado el modelo.
/// </summary>
public class CarritoMovimiento : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Objeto que se moverá. Si se deja vacío se usará el objeto del script.")]
    public Transform objetivoAMover;

    [Header("Trayectoria")]
    [Tooltip("Lista de puntos. El primero es el inicio, el último es el destino.")]
    public List<Transform> waypoints = new List<Transform>();

    [Tooltip("¿Repetir el recorrido en loop?")]
    public bool loop = false;

    [Header("Movimiento")]
    [Tooltip("Velocidad en unidades por segundo.")]
    public float velocidad = 3f;

    [Tooltip("Distancia mínima al waypoint para pasar al siguiente.")]
    public float distanciaLlegada = 0.1f;

    [Header("Rotación")]
    [Tooltip("Velocidad de rotación en grados por segundo.")]
    public float velocidadRotacion = 180f;

    [Tooltip("Eje LOCAL del modelo que apunta hacia adelante (la dirección de movimiento). " +
             "Ej: si el morro del carro apunta en +X local, elige X.")]
    public EjeLocal ejeAdelante = EjeLocal.Z;

    [Tooltip("Eje LOCAL del modelo que apunta hacia arriba. " +
             "Ej: si el techo del carro apunta en +Y local, elige Y.")]
    public EjeLocal ejeArriba = EjeLocal.Y;

    [Header("Altura")]
    [Tooltip("Bloquear Y para evitar que el objeto levite.")]
    public bool bloquearAltura = true;

    [Tooltip("Altura fija. Se inicializa automáticamente con la Y del objeto al Start.")]
    public float alturaFija = 0f;

    private int indiceActual = 0;
    private bool enMovimiento = true;
    private Transform carro;

    void Start()
    {
        carro = (objetivoAMover != null) ? objetivoAMover : transform;

        if (waypoints == null || waypoints.Count < 2)
        {
            Debug.LogError("⚠️ CARRITOMOVIMIENTO: Necesitas al menos 2 waypoints.");
            enMovimiento = false;
            return;
        }

        if (bloquearAltura)
            alturaFija = carro.position.y;

        carro.position = PosicionWaypoint(0);

        Debug.Log($"✅ CARRITOMOVIMIENTO: Listo. Objeto: '{carro.name}' | Adelante: {ejeAdelante} | Arriba: {ejeArriba}");
    }

    void Update()
    {
        if (!enMovimiento) return;

        if (waypoints[indiceActual] == null) { AvanzarWaypoint(); return; }

        Vector3 posObjetivo = PosicionWaypoint(indiceActual);
        Vector3 posActual = carro.position;

        // Movimiento
        carro.position = Vector3.MoveTowards(posActual, posObjetivo, velocidad * Time.deltaTime);

        // Bloqueo altura
        if (bloquearAltura)
            carro.position = new Vector3(carro.position.x, alturaFija, carro.position.z);

        // Rotación
        Vector3 direccion = posObjetivo - posActual;
        direccion.y = 0f;

        if (direccion.sqrMagnitude > 0.001f)
        {
            // Construimos la rotación objetivo sabiendo qué eje del modelo
            // debe alinearse con la dirección de movimiento y cuál con el mundo arriba
            Quaternion rotacionObjetivo = RotacionHaciaDir(direccion.normalized);

            carro.rotation = Quaternion.RotateTowards(
                carro.rotation,
                rotacionObjetivo,
                velocidadRotacion * Time.deltaTime
            );
        }

        if (Vector3.Distance(posActual, posObjetivo) <= distanciaLlegada)
            AvanzarWaypoint();
    }

    /// <summary>
    /// Calcula la rotación para que el eje "adelante" del modelo apunte a la dirección dada
    /// y el eje "arriba" del modelo apunte a Vector3.up del mundo.
    /// </summary>
    Quaternion RotacionHaciaDir(Vector3 direccionMundo)
    {
        // Queremos que:  modelAdelante → direccionMundo
        //                modelArriba   → Vector3.up
        //
        // Quaternion.LookRotation(fwd, up) hace que Z+ → fwd y Y+ → up.
        // Para otros ejes, calculamos la rotación correctiva.

        Vector3 modelAdelante = EjeAVector(ejeAdelante);
        Vector3 modelArriba   = EjeAVector(ejeArriba);

        // Rotación que llevaría Z+ → direccionMundo y Y+ → up (la rotación "ideal" en mundo)
        Quaternion rotMundo = Quaternion.LookRotation(direccionMundo, Vector3.up);

        // Rotación correctiva para compensar que el modelo no tiene su frente en Z+ ni su arriba en Y+
        // FromToRotation: rota el espacio para que modelAdelante coincida con Z+ (el "frente" de LookRotation)
        Quaternion correccionAdelante = Quaternion.FromToRotation(modelAdelante, Vector3.forward);
        Quaternion correccionArriba   = Quaternion.FromToRotation(modelArriba,   Vector3.up);

        // Aplicamos primero la corrección de arriba, luego la de adelante, luego la rotación mundo
        return rotMundo * Quaternion.Inverse(correccionAdelante) * Quaternion.Inverse(correccionArriba);
    }

    /// <summary>
    /// Convierte el enum de eje a un Vector3 unitario en espacio local.
    /// </summary>
    Vector3 EjeAVector(EjeLocal eje)
    {
        switch (eje)
        {
            case EjeLocal.X:  return Vector3.right;
            case EjeLocal.MX: return Vector3.left;
            case EjeLocal.Y:  return Vector3.up;
            case EjeLocal.MY: return Vector3.down;
            case EjeLocal.Z:  return Vector3.forward;
            case EjeLocal.MZ: return Vector3.back;
            default:          return Vector3.forward;
        }
    }

    Vector3 PosicionWaypoint(int indice)
    {
        Vector3 pos = waypoints[indice].position;
        if (bloquearAltura) pos.y = alturaFija;
        return pos;
    }

    void AvanzarWaypoint()
    {
        indiceActual++;
        if (indiceActual >= waypoints.Count)
        {
            if (loop)
            {
                indiceActual = 0;
                Debug.Log("🔄 CARRITOMOVIMIENTO: Reiniciando loop.");
            }
            else
            {
                indiceActual = waypoints.Count - 1;
                enMovimiento = false;
                Debug.Log("🏁 CARRITOMOVIMIENTO: Llegó al destino.");
            }
        }
    }

    public void ReiniciarRecorrido()
    {
        indiceActual = 0;
        enMovimiento = true;
        if (carro != null) carro.position = PosicionWaypoint(0);
    }

    void OnDrawGizmos()
    {
        if (waypoints == null || waypoints.Count < 2) return;
        Gizmos.color = Color.yellow;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i] == null || waypoints[i + 1] == null) continue;
            Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            Gizmos.DrawWireSphere(waypoints[i].position, 0.15f);
        }
        if (waypoints[waypoints.Count - 1] != null)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(waypoints[waypoints.Count - 1].position, 0.2f);
        }
    }
}

public enum EjeLocal
{
    X,
    [InspectorName("-X")] MX,
    Y,
    [InspectorName("-Y")] MY,
    Z,
    [InspectorName("-Z")] MZ
}