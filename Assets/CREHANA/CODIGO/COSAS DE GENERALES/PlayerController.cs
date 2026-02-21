using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    public InputMode currentInputMode = InputMode.Keyboard;

    [Header("Configuración de Movimiento")]
    [Tooltip("Velocidad de movimiento del personaje")]
    public float velocidadMovimiento = 5f;
    
    [Tooltip("Velocidad de rotación del personaje")]
    public float velocidadRotacion = 10f;

    [Header("Configuración de Salto ARCADE")]
    [Tooltip("Altura máxima del salto en unidades")]
    public float alturaSalto = 3f;
    
    [Tooltip("Tiempo que tarda en alcanzar la altura máxima (en segundos). Menor = más rápido")]
    [Range(0.1f, 2f)]
    public float tiempoSubida = 0.3f;
    
    [Tooltip("Gravedad personalizada al caer. Mayor = cae más rápido")]
    public float gravedadCaida = 25f;
    
    [Tooltip("Gravedad personalizada al subir. Mayor = sube más rápido y baja antes")]
    public float gravedadSubida = 20f;
    
    [Tooltip("Multiplicador de gravedad cuando sueltas el botón de salto (para saltos cortos)")]
    public float multiplicadorSaltoBajo = 2f;

    [Header("Detección de Suelo - IMPORTANTE")]
    [Tooltip("Posición desde donde se lanza el raycast (0 = pies, 0.5 = centro)")]
    public float offsetRaycast = 0.1f;
    
    [Tooltip("Distancia para detectar el suelo")]
    public float distanciaDeteccionSuelo = 0.3f;
    
    [Tooltip("Layer del suelo - DEBE estar configurado")]
    public LayerMask capaSuelo;
    
    [Tooltip("Usar esfera en lugar de raycast (más confiable)")]
    public bool usarSphereCast = true;
    
    [Tooltip("Radio de la esfera para detección")]
    public float radioEsfera = 0.3f;

    [Header("Referencias")]
    [Tooltip("Animator del personaje")]
    public Animator animator;
    
    [Header("Debug - Información en Tiempo Real")]
    [Tooltip("Mostrar información de debug en consola")]
    public bool mostrarDebug = true;

    // Variables privadas
    private Rigidbody rb;
    private Vector3 direccionMovimiento;
    private bool enSuelo;
    private bool estaSaltando;
    private Vector2 inputMovimiento;
    private bool inputSalto;
    private bool inputSaltoMantiene;
    private Collider personajeCollider;
    private float velocidadInicialSalto;

    // Guardamos la última rotación válida para que no se resetee sola en WebGL
    private Quaternion ultimaRotacionValida;

    // Nombres de parámetros del Animator
    private const string ANIM_CORRIENDO = "Corriendo";
    private const string ANIM_CAYENDO = "Cayendo";
    private const string ANIM_IDLE = "Idle";

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        rb.useGravity = false;

        personajeCollider = GetComponent<Collider>();

        // Inicializar con la rotación actual del objeto
        ultimaRotacionValida = transform.rotation;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
                Debug.LogWarning("No se encontró un Animator en el personaje. Asigna uno manualmente.");
        }

        if (capaSuelo == 0)
            Debug.LogError("⚠️ IMPORTANTE: No has asignado el Layer 'Suelo' en el Inspector!");

        CalcularVelocidadSalto();

        if (mostrarDebug)
        {
            Debug.Log($"=== PLAYERCONTROLLER INICIADO ===");
            Debug.Log($"Collider del personaje: {personajeCollider?.GetType().Name}");
            Debug.Log($"Layer del suelo configurado: {LayerMaskToString(capaSuelo)}");
            Debug.Log($"Velocidad de salto calculada: {velocidadInicialSalto:F2}");
        }
    }

    void Update()
    {
        ObtenerInput();
        DetectarSuelo();
        ManejarSalto();
        ActualizarAnimaciones();

        if (mostrarDebug && Time.frameCount % 60 == 0)
            Debug.Log($"En Suelo: {enSuelo} | Saltando: {estaSaltando} | Velocidad Y: {rb.linearVelocity.y:F2}");
    }

    void FixedUpdate()
    {
        MoverPersonaje();
        RotarPersonaje();
        AplicarGravedad();
    }

    void CalcularVelocidadSalto()
    {
        velocidadInicialSalto = (2f * alturaSalto) / tiempoSubida;
    }

    #region INPUT

    void ObtenerInput()
    {
        PlayerInputData input = InputManager.Instance.CurrentInput;
        Move(input.movement);
        inputSalto = input.jumpPressed;
    }

    void Move(Vector2 move)
    {
        Vector3 dir = new Vector3(move.x, 0f, move.y);

        if (dir.sqrMagnitude > 1f)
            dir.Normalize();

        direccionMovimiento = dir;
    }

    void Jump()
    {
        inputSalto = true;
    }

    #endregion


    void MoverPersonaje()
    {
        Vector3 velocidad = direccionMovimiento * velocidadMovimiento;
        velocidad.y = rb.linearVelocity.y;
        rb.linearVelocity = velocidad;
    }

    void AplicarGravedad()
    {
        if (enSuelo && rb.linearVelocity.y <= 0)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -2f, rb.linearVelocity.z);
            return;
        }

        float gravedadActual;

        if (rb.linearVelocity.y > 0)
        {
            gravedadActual = inputSaltoMantiene
                ? gravedadSubida
                : gravedadSubida * multiplicadorSaltoBajo;
        }
        else
        {
            gravedadActual = gravedadCaida;
        }

        rb.linearVelocity += Vector3.down * gravedadActual * Time.fixedDeltaTime;
    }

    void DetectarSuelo()
    {
        Vector3 posicionInicio = transform.position + Vector3.up * offsetRaycast;

        if (usarSphereCast)
        {
            enSuelo = Physics.SphereCast(
                posicionInicio,
                radioEsfera,
                Vector3.down,
                out RaycastHit hit,
                distanciaDeteccionSuelo,
                capaSuelo
            );

            if (mostrarDebug && enSuelo)
                Debug.DrawLine(posicionInicio, hit.point, Color.green);
        }
        else
        {
            enSuelo = Physics.Raycast(
                posicionInicio,
                Vector3.down,
                out RaycastHit hit,
                distanciaDeteccionSuelo,
                capaSuelo
            );
        }

        if (enSuelo && rb.linearVelocity.y <= 0)
            estaSaltando = false;
    }

    void ManejarSalto()
    {
        if (inputSalto && mostrarDebug)
            Debug.Log($"🎮 Espacio presionado | En Suelo: {enSuelo} | Ya Saltando: {estaSaltando}");

        if (inputSalto && enSuelo && !estaSaltando)
        {
            CalcularVelocidadSalto();
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, velocidadInicialSalto, rb.linearVelocity.z);
            estaSaltando = true;

            if (mostrarDebug)
                Debug.Log($"🚀 ¡SALTO! Velocidad aplicada: {velocidadInicialSalto:F2}");
        }
    }

    void RotarPersonaje()
    {
       
        if (direccionMovimiento.magnitude > 0.1f)
        {
            // Forzamos Y=0 y normalizamos explícitamente antes de pasarlo a LookRotation.
            // En WebGL los floats de 32 bits pueden acumular un residuo en Y que hace que
            // LookRotation devuelva un quaternion inclinado, limitando la rotación horizontal.
            Vector3 dirPlana = new Vector3(direccionMovimiento.x, 0f, direccionMovimiento.z).normalized;


            Quaternion rotacionObjetivo = Quaternion.LookRotation(dirPlana, Vector3.up);

            // Slerp igual que antes — el fix real está en sanear el vector de entrada
            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                rotacionObjetivo,
                velocidadRotacion * Time.deltaTime
            );

            // Guardar la rotación alcanzada para mantenerla cuando no hay input
            ultimaRotacionValida = transform.rotation;
        }
        else
        {
            // Sin input: fijamos explícitamente la última rotación válida.
            // Esto corrige el bug de WebGL donde la rotación vuelve sola a la posición inicial.
            transform.rotation = ultimaRotacionValida;
        }
    }

    void ActualizarAnimaciones()
    {
        if (animator == null) return;

        bool estaMoviendose = direccionMovimiento.magnitude > 0.1f;

        if (!enSuelo || estaSaltando)
        {
            animator.SetBool(ANIM_CAYENDO, true);
            animator.SetBool(ANIM_CORRIENDO, false);
            animator.SetBool(ANIM_IDLE, false);
        }
        else if (estaMoviendose)
        {
            animator.SetBool(ANIM_CORRIENDO, true);
            animator.SetBool(ANIM_CAYENDO, false);
            animator.SetBool(ANIM_IDLE, false);
        }
        else
        {
            animator.SetBool(ANIM_IDLE, true);
            animator.SetBool(ANIM_CORRIENDO, false);
            animator.SetBool(ANIM_CAYENDO, false);
        }
    }

    void OnDrawGizmos()
    {
        Vector3 posicionInicio = transform.position + Vector3.up * offsetRaycast;
        Gizmos.color = enSuelo ? Color.green : Color.red;

        if (usarSphereCast)
        {
            Gizmos.DrawWireSphere(posicionInicio, radioEsfera);
            Gizmos.DrawWireSphere(posicionInicio + Vector3.down * distanciaDeteccionSuelo, radioEsfera);
            Gizmos.DrawLine(posicionInicio, posicionInicio + Vector3.down * distanciaDeteccionSuelo);
        }
        else
        {
            Gizmos.DrawLine(posicionInicio, posicionInicio + Vector3.down * distanciaDeteccionSuelo);
        }
    }

    private string LayerMaskToString(LayerMask mask)
    {
        string result = "";
        for (int i = 0; i < 32; i++)
        {
            if ((mask.value & (1 << i)) != 0)
            {
                if (result.Length > 0) result += ", ";
                result += LayerMask.LayerToName(i);
            }
        }
        return string.IsNullOrEmpty(result) ? "NINGUNO" : result;
    }
}