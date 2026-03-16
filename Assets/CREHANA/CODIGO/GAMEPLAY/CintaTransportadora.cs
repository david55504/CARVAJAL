using UnityEngine;
using UnityEngine.Splines;

public class CintaTransportadora : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El SplineContainer que define el recorrido. Si se deja vacío, se buscará automáticamente en los hijos del objeto.")]
    public SplineContainer splineContainer;

    [Tooltip("Prefab del producto que recorrerá el spline.")]
    public GameObject productoPrefab;

    [Header("Configuración de Movimiento")]
    [Tooltip("Velocidad a la que los productos recorren el spline (unidades por segundo).")]
    public float velocidad = 2f;

    [Header("Configuración de Spawn")]
    [Tooltip("Tiempo en segundos entre la aparición de cada nuevo producto.")]
    public float intervaloSpawn = 2f;

    [Tooltip("¿Hacer spawn del primer producto al iniciar la escena?")]
    public bool spawnInmediato = true;

    private float timerSpawn = 0f;
    private bool sistemaListo = false;

    // Datos tomados del objeto de referencia en escena
    private Vector3 escalaOriginal;
    private Quaternion rotacionMundialOriginal;
    private float alturaOffset;

    void Start()
    {
        if (splineContainer == null)
            splineContainer = GetComponentInChildren<SplineContainer>();

        if (splineContainer == null)
        {
            Debug.LogError("⚠️ CINTATRANSPORTADORA: No se encontró ningún SplineContainer.");
            return;
        }

        if (productoPrefab == null)
        {
            Debug.LogError("⚠️ CINTATRANSPORTADORA: No hay prefab de producto asignado.");
            return;
        }

        // Escala mundial real del original (considera todos los padres)
        escalaOriginal = productoPrefab.transform.lossyScale;

        // Rotación mundial exacta del original
        rotacionMundialOriginal = productoPrefab.transform.rotation;

        // Solo guardamos la diferencia de altura (Y) entre la caja y el spline.
        // Esto hace que los clones queden encima de la barra sin hundirse,
        // sin importar hacia donde apunte el spline en XZ.
        Vector3 posSpline = splineContainer.EvaluatePosition(0f);
        alturaOffset = productoPrefab.transform.position.y - posSpline.y;

        sistemaListo = true;
        Debug.Log($"✅ CINTATRANSPORTADORA: Sistema listo. Altura offset: {alturaOffset}");

        if (spawnInmediato)
        {
            SpawnProducto();
            timerSpawn = 0f;
        }
    }

    void Update()
    {
        if (!sistemaListo) return;

        timerSpawn += Time.deltaTime;
        if (timerSpawn >= intervaloSpawn)
        {
            timerSpawn = 0f;
            SpawnProducto();
        }
    }

    void SpawnProducto()
    {
        Vector3 posSpline = splineContainer.EvaluatePosition(0f);

        // Posición: punto del spline + offset de altura en mundo
        Vector3 posicionFinal = posSpline + Vector3.up * alturaOffset;

        GameObject producto = Instantiate(productoPrefab, posicionFinal, rotacionMundialOriginal);
        producto.transform.localScale = escalaOriginal;

        MovimientoProducto mover = producto.AddComponent<MovimientoProducto>();
        mover.Inicializar(splineContainer, velocidad, alturaOffset, rotacionMundialOriginal, escalaOriginal);
    }
}


public class MovimientoProducto : MonoBehaviour
{
    private SplineContainer spline;
    private float velocidad;
    private float longitudSpline;
    private float t = 0f;

    private float alturaOffset;
    private Quaternion rotacionMundial;
    private Vector3 escala;

    public void Inicializar(SplineContainer splineContainer, float velocidadMovimiento,
        float altura, Quaternion rotacion, Vector3 escalaObj)
    {
        spline = splineContainer;
        velocidad = velocidadMovimiento;
        longitudSpline = spline.CalculateLength();
        alturaOffset = altura;
        rotacionMundial = rotacion;
        escala = escalaObj;
    }

    void Update()
    {
        if (spline == null) return;

        t += (velocidad / longitudSpline) * Time.deltaTime;

        if (t >= 1f)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 posSpline = spline.EvaluatePosition(t);

        // Mantener siempre la rotación y escala del original,
        // y solo desplazar en Y para no hundirse en la barra
        transform.position = posSpline + Vector3.up * alturaOffset;
        transform.rotation = rotacionMundial;
        transform.localScale = escala;
    }
}