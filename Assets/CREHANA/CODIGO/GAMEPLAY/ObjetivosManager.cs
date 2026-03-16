using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Gestiona el conteo de objetivos del jugador mediante triggers.
/// Cuando el jugador atraviesa el mínimo de triggers requeridos, se activan los elementos de victoria.
/// </summary>
public class ObjetivosManager : MonoBehaviour
{
    [Header("Objetivos (Triggers)")]
    [Tooltip("Lista de GameObjects que tienen un Collider con 'Is Trigger' activado. " +
             "El script detectará automáticamente cuándo el Player los atraviesa.")]
    public List<GameObject> objetivos = new List<GameObject>();

    [Header("Condición de Victoria")]
    [Tooltip("Cantidad mínima de triggers que el jugador debe atravesar para ganar. " +
             "Si es menor o igual a 0, se usará el total de objetivos en la lista.")]
    [Min(0)]
    public int minimoObjetivosRequeridos = 0;

    [Header("Puntuación")]
    [Tooltip("Texto del canvas donde se mostrará el puntaje actual.")]
    public TextMeshProUGUI textoPuntuacion;

    [Tooltip("Puntos que se suman cada vez que el jugador atraviesa un trigger de la lista.")]
    public int puntosPorObjetivo = 10;

    [Tooltip("Velocidad a la que suben los números en pantalla (puntos por segundo). Ej: 200 = tarda 1.5s en subir 300 puntos.")]
    public float velocidadAnimacionPuntos = 200f;

    [Header("Elementos de Victoria")]
    [Tooltip("GameObjects que se ACTIVARÁN cuando se cumpla la meta (ej: panel de victoria, botones, etc.)")]
    public List<GameObject> elementosVictoria = new List<GameObject>();

    [Header("Configuración")]
    [Tooltip("Tag que debe tener el jugador para que se cuente el objetivo.")]
    public string tagJugador = "Player";

    [Tooltip("¿Desactivar los triggers ya atravesados para que no cuenten dos veces?")]
    public bool desactivarTriggerAlTocar = true;

    [Tooltip("Segundos de espera entre que se cumple la meta y que se activan los elementos de victoria. " +
             "Útil para evitar solapamientos con timers u otras animaciones.")]
    [Min(0f)]
    public float esperaAntesDeVictoria = 0f;

    // Estado interno
    private int objetivosCompletados = 0;
    private bool metaCumplida = false;
    private int metaEfectiva = 0;
    private int puntuacionActual = 0;
    private float puntuacionMostrada = 0f;

    // Referencia interna para conectar cada trigger con este manager
    private List<TriggerObjetivo> componentesTrigger = new List<TriggerObjetivo>();

    void Start()
    {
        // Calcular la meta efectiva
        metaEfectiva = (minimoObjetivosRequeridos <= 0) ? objetivos.Count : minimoObjetivosRequeridos;
        metaEfectiva = Mathf.Clamp(metaEfectiva, 1, int.MaxValue);

        // Desactivar elementos de victoria al inicio
        foreach (GameObject elemento in elementosVictoria)
        {
            if (elemento != null)
                elemento.SetActive(false);
        }

        // Inicializar el texto de puntuación
        ActualizarTextoPuntuacion();

        // Inyectar el componente detector en cada objetivo
        foreach (GameObject obj in objetivos)
        {
            if (obj == null)
            {
                Debug.LogWarning("⚠️ OBJETIVOSMANAGER: Hay un objetivo nulo en la lista, será ignorado.");
                continue;
            }

            // Verificar que tenga un Collider trigger
            Collider col = obj.GetComponent<Collider>();
            if (col == null)
            {
                Debug.LogWarning($"⚠️ OBJETIVOSMANAGER: '{obj.name}' no tiene Collider. No se podrá detectar el trigger.");
                continue;
            }
            if (!col.isTrigger)
            {
                Debug.LogWarning($"⚠️ OBJETIVOSMANAGER: El Collider de '{obj.name}' no tiene 'Is Trigger' activado.");
            }

            // Agregar o reutilizar el componente detector
            TriggerObjetivo detector = obj.GetComponent<TriggerObjetivo>();
            if (detector == null)
                detector = obj.AddComponent<TriggerObjetivo>();

            detector.Inicializar(this, tagJugador, desactivarTriggerAlTocar);
            componentesTrigger.Add(detector);
        }

        Debug.Log($"✅ OBJETIVOSMANAGER: Sistema iniciado. Meta: {metaEfectiva} de {objetivos.Count} objetivos.");
    }

    /// <summary>
    /// Llamado por TriggerObjetivo cuando el jugador atraviesa un trigger válido.
    /// </summary>
    public void RegistrarObjetivoCompletado(GameObject objetivoCompletado)
    {
        if (metaCumplida) return;

        objetivosCompletados++;
        puntuacionActual += puntosPorObjetivo;
        ActualizarTextoPuntuacion();

        Debug.Log($"🎯 Objetivo completado: '{objetivoCompletado.name}' | Progreso: {objetivosCompletados}/{metaEfectiva} | Puntos: {puntuacionActual}");

        if (objetivosCompletados >= metaEfectiva)
        {
            metaCumplida = true;
            StartCoroutine(EsperarYActivarVictoria());
        }
    }

    /// <summary>
    /// Lanza la corrutina que anima el contador en pantalla hacia el valor objetivo.
    /// Si ya hay una animación en curso, simplemente actualiza el objetivo y continúa desde donde va.
    /// </summary>
    private void ActualizarTextoPuntuacion()
    {
        StopCoroutine("AnimarPuntuacion");
        StartCoroutine("AnimarPuntuacion");
    }

    /// <summary>
    /// Incrementa el número visible en pantalla gradualmente hasta alcanzar puntuacionActual.
    /// </summary>
    System.Collections.IEnumerator AnimarPuntuacion()
    {
        while (puntuacionMostrada < puntuacionActual)
        {
            puntuacionMostrada = Mathf.MoveTowards(
                puntuacionMostrada,
                puntuacionActual,
                velocidadAnimacionPuntos * Time.deltaTime
            );

            if (textoPuntuacion != null)
                textoPuntuacion.text = Mathf.FloorToInt(puntuacionMostrada).ToString();

            yield return null;
        }

        // Asegurarse de que el número final sea exacto
        puntuacionMostrada = puntuacionActual;
        if (textoPuntuacion != null)
            textoPuntuacion.text = puntuacionActual.ToString();
    }

    /// <summary>
    /// Espera el tiempo configurado y luego activa los elementos de victoria.
    /// </summary>
    System.Collections.IEnumerator EsperarYActivarVictoria()
    {
        if (esperaAntesDeVictoria > 0f)
        {
            Debug.Log($"⏳ Meta cumplida. Activando victoria en {esperaAntesDeVictoria} segundos...");
            yield return new WaitForSeconds(esperaAntesDeVictoria);
        }

        Debug.Log($"🏆 ¡META CUMPLIDA! El jugador completó {objetivosCompletados} objetivos con {puntuacionActual} puntos.");

        foreach (GameObject elemento in elementosVictoria)
        {
            if (elemento != null)
                elemento.SetActive(true);
        }
    }

    /// <summary>
    /// Reinicia el contador, la puntuación y el estado de victoria.
    /// </summary>
    public void ReiniciarObjetivos()
    {
        objetivosCompletados = 0;
        metaCumplida = false;
        puntuacionActual = 0;
        puntuacionMostrada = 0f;
        ActualizarTextoPuntuacion();

        // Desactivar elementos de victoria
        foreach (GameObject elemento in elementosVictoria)
        {
            if (elemento != null)
                elemento.SetActive(false);
        }

        // Reactivar todos los triggers
        foreach (TriggerObjetivo detector in componentesTrigger)
        {
            if (detector != null)
                detector.Reactivar();
        }

        Debug.Log("🔄 OBJETIVOSMANAGER: Objetivos reiniciados.");
    }

    /// <summary>
    /// Devuelve cuántos objetivos ha completado el jugador.
    /// </summary>
    public int ObtenerObjetivosCompletados() => objetivosCompletados;

    /// <summary>
    /// Devuelve la meta efectiva (mínimo requerido).
    /// </summary>
    public int ObtenerMeta() => metaEfectiva;

    /// <summary>
    /// Indica si la meta ya fue cumplida.
    /// </summary>
    public bool MetaCumplida() => metaCumplida;

    /// <summary>
    /// Devuelve la puntuación actual del jugador.
    /// </summary>
    public int ObtenerPuntuacion() => puntuacionActual;
}


/// <summary>
/// Componente auxiliar que se inyecta automáticamente en cada GameObject objetivo.
/// Detecta cuando el jugador entra al trigger y notifica al ObjetivosManager.
/// No hace falta asignarlo manualmente en el Inspector.
/// </summary>
public class TriggerObjetivo : MonoBehaviour
{
    private ObjetivosManager manager;
    private string tagJugador;
    private bool desactivarAlTocar;
    private bool yaCompletado = false;

    public void Inicializar(ObjetivosManager manager, string tagJugador, bool desactivarAlTocar)
    {
        this.manager = manager;
        this.tagJugador = tagJugador;
        this.desactivarAlTocar = desactivarAlTocar;
    }

    void OnTriggerEnter(Collider other)
    {
        if (yaCompletado) return;
        if (!other.CompareTag(tagJugador)) return;

        yaCompletado = true;
        manager.RegistrarObjetivoCompletado(gameObject);

        if (desactivarAlTocar)
        {
            // Desactivar solo el collider para que no vuelva a contar,
            // manteniendo el objeto visible en escena.
            Collider col = GetComponent<Collider>();
            if (col != null) col.enabled = false;
        }
    }

    /// <summary>
    /// Reactiva el trigger para cuando se reinicien los objetivos.
    /// </summary>
    public void Reactivar()
    {
        yaCompletado = false;
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;
    }
}