using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

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


    // Nombres de parámetros del Animator
    private const string ANIM_CORRIENDO = "Corriendo";
    private const string ANIM_CAYENDO = "Cayendo";
    private const string ANIM_IDLE = "Idle";

    void Start()
    {
        // Obtener el Rigidbody
        rb = GetComponent<Rigidbody>();
        
        // Congelar rotación para evitar que el personaje se caiga
        rb.freezeRotation = true;
        
        // IMPORTANTE: Desactivar la gravedad de Unity, usaremos nuestra propia gravedad
        rb.useGravity = false;

        // Obtener el collider del personaje
        personajeCollider = GetComponent<Collider>();


        // Si no se asignó el Animator, intentar obtenerlo
        if (animator == null)
        {
            animator = GetComponent<Animator>();
            if (animator == null)
            {
                Debug.LogWarning("No se encontró un Animator en el personaje. Asigna uno manualmente.");
            }
        }

        // Verificar configuración del Layer
        if (capaSuelo == 0)
        {
            Debug.LogError("⚠️ IMPORTANTE: No has asignado el Layer 'Suelo' en el Inspector!");
        }
        
        // Calcular velocidad inicial del salto basada en altura y tiempo
        CalcularVelocidadSalto();

        // Info inicial
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
        // Obtener input del jugador
        ObtenerInput();

        // Detectar si está en el suelo
        DetectarSuelo();

        // Manejar el salto
        ManejarSalto();

        // Rotar hacia la dirección de movimiento
        RotarPersonaje();

        // Actualizar animaciones
        ActualizarAnimaciones();

        // Debug info cada segundo
        if (mostrarDebug && Time.frameCount % 60 == 0)
        {
            Debug.Log($"En Suelo: {enSuelo} | Saltando: {estaSaltando} | Velocidad Y: {rb.linearVelocity.y:F2}");
        }
    }

    void FixedUpdate()
    {
        // Mover el personaje
        MoverPersonaje();
        
        // Aplicar gravedad personalizada
        AplicarGravedad();
    }

    void CalcularVelocidadSalto()
    {
        // Calcular la velocidad inicial necesaria para alcanzar la altura en el tiempo especificado
        // Fórmula: v = (2 * h) / t
        velocidadInicialSalto = (2f * alturaSalto) / tiempoSubida;
    }

    #region INPUT
    /*void ObtenerInput()
    {
        // Obtener input de movimiento desde el nuevo Input System
        inputMovimiento = Vector2.zero;

        //TOUCH MOVE INPUT 
        if (playerTouchMovement != null && playerTouchMovement.touchActive)
        {
            inputMovimiento = playerTouchMovement.movementAmount;
        }

        // Keyboard (WASD y Flechas)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                inputMovimiento.y += 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                inputMovimiento.y -= 1;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                inputMovimiento.x -= 1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                inputMovimiento.x += 1;
        }

        // Gamepad (opcional, por si quieres soporte para mando)
        if (Gamepad.current != null)
        {
            Vector2 stickInput = Gamepad.current.leftStick.ReadValue();
            if (stickInput.magnitude > 0.1f)
            {
                inputMovimiento = stickInput;
            }
        }

    

        // Crear vector de dirección (en el plano XZ)
        direccionMovimiento = new Vector3(inputMovimiento.x, 0f, inputMovimiento.y).normalized;



        // Obtener input de salto
        inputSalto = false;
        inputSaltoMantiene = false;


        //TOUCH BUTTON INPUT 
        if (playerTouchMovement != null && DeviceDetector.isMobileWebGl)
        {
            inputSalto = playerTouchMovement.IsJumpButtonPressed();
        }

        if (Keyboard.current != null)
        {
            inputSalto = Keyboard.current.spaceKey.wasPressedThisFrame;
            inputSaltoMantiene = Keyboard.current.spaceKey.isPressed;
        }
        
        // Gamepad - botón sur (A en Xbox, X en PlayStation)
        if (Gamepad.current != null)
        {
            inputSalto = inputSalto || Gamepad.current.buttonSouth.wasPressedThisFrame;
            inputSaltoMantiene = inputSaltoMantiene || Gamepad.current.buttonSouth.isPressed;
        }

    
    }*/
    void ObtenerInput()
    {

        PlayerInputData input = InputManager.Instance.CurrentInput;

        Move(input.movement);
        inputSalto = input.jumpPressed;
       
        //touchUI.SetActive(DeviceDetector.isMobileWebGl && currentInputMode == InputMode.Touch);
    }

    void Move(Vector2 move)
    {
        direccionMovimiento = new Vector3(
           move.x,
           0f,
           move.y
       ).normalized;
    }

    void Jump()
    {
        inputSalto = true;
    }


    #endregion


    void MoverPersonaje()
    {
        // Calcular velocidad de movimiento
        Vector3 velocidad = direccionMovimiento * velocidadMovimiento;

        // Mantener la velocidad vertical actual (para la gravedad y el salto)
        velocidad.y = rb.linearVelocity.y;

        // Aplicar la velocidad al Rigidbody
        rb.linearVelocity = velocidad;
    }

    void AplicarGravedad()
    {
        // Si está en el suelo, no aplicar gravedad
        if (enSuelo && rb.linearVelocity.y <= 0)
        {
            // Mantener al personaje pegado al suelo
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, -2f, rb.linearVelocity.z);
            return;
        }

        // Determinar qué gravedad usar
        float gravedadActual;
        
        if (rb.linearVelocity.y > 0)
        {
            // Subiendo
            if (inputSaltoMantiene)
            {
                // Mantiene presionado el botón - salto completo
                gravedadActual = gravedadSubida;
            }
            else
            {
                // Soltó el botón - salto corto (cae más rápido)
                gravedadActual = gravedadSubida * multiplicadorSaltoBajo;
            }
        }
        else
        {
            // Cayendo - usar gravedad de caída
            gravedadActual = gravedadCaida;
        }

        // Aplicar gravedad personalizada
        rb.linearVelocity += Vector3.down * gravedadActual * Time.fixedDeltaTime;
    }

    void DetectarSuelo()
    {
        // Calcular posición de inicio del raycast
        Vector3 posicionInicio = transform.position + Vector3.up * offsetRaycast;
        
        if (usarSphereCast)
        {
            // Usar SphereCast (más confiable)
            enSuelo = Physics.SphereCast(
                posicionInicio, 
                radioEsfera, 
                Vector3.down, 
                out RaycastHit hit,
                distanciaDeteccionSuelo, 
                capaSuelo
            );

            // Debug visual
            if (mostrarDebug && enSuelo)
            {
                Debug.DrawLine(posicionInicio, hit.point, Color.green);
            }
        }
        else
        {
            // Usar Raycast simple
            enSuelo = Physics.Raycast(
                posicionInicio, 
                Vector3.down, 
                out RaycastHit hit,
                distanciaDeteccionSuelo, 
                capaSuelo
            );
        }
        
        // Si tocó el suelo, ya no está saltando
        if (enSuelo && rb.linearVelocity.y <= 0)
        {
            estaSaltando = false;
        }
    }

    void ManejarSalto()
    {
        // Debug cuando presiona espacio
        if (inputSalto && mostrarDebug)
        {
            Debug.Log($"🎮 Espacio presionado | En Suelo: {enSuelo} | Ya Saltando: {estaSaltando}");
        }

        // Si presiona espacio y está en el suelo
        if (inputSalto && enSuelo && !estaSaltando)
        {
            // Recalcular por si cambiaste los valores en runtime
            CalcularVelocidadSalto();
            
            // Aplicar velocidad de salto
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, velocidadInicialSalto, rb.linearVelocity.z);
            estaSaltando = true;

            if (mostrarDebug)
            {
                Debug.Log($"🚀 ¡SALTO! Velocidad aplicada: {velocidadInicialSalto:F2}");
            }
        }
    }

    void RotarPersonaje()
    {
        // Solo rotar si hay movimiento
        if (direccionMovimiento.magnitude > 0.1f)
        {
            // Calcular la rotación objetivo
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccionMovimiento);

            // Interpolar suavemente hacia la rotación objetivo
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadRotacion * Time.deltaTime);
        }
    }

    void ActualizarAnimaciones()
    {
        if (animator == null) return;

        // Determinar qué animación reproducir
        bool estaMoviendose = direccionMovimiento.magnitude > 0.1f;

        if (!enSuelo || estaSaltando)
        {
            // Animación de caer/saltar
            animator.SetBool(ANIM_CAYENDO, true);
            animator.SetBool(ANIM_CORRIENDO, false);
            animator.SetBool(ANIM_IDLE, false);
        }
        else if (estaMoviendose)
        {
            // Animación de correr
            animator.SetBool(ANIM_CORRIENDO, true);
            animator.SetBool(ANIM_CAYENDO, false);
            animator.SetBool(ANIM_IDLE, false);
        }
        else
        {
            // Animación idle
            animator.SetBool(ANIM_IDLE, true);
            animator.SetBool(ANIM_CORRIENDO, false);
            animator.SetBool(ANIM_CAYENDO, false);
        }
    }

    // Visualizar el raycast en el editor
    void OnDrawGizmos()
    {
        Vector3 posicionInicio = transform.position + Vector3.up * offsetRaycast;
        
        // Color según si está en suelo o no
        Gizmos.color = enSuelo ? Color.green : Color.red;
        
        if (usarSphereCast)
        {
            // Visualizar SphereCast
            Gizmos.DrawWireSphere(posicionInicio, radioEsfera);
            Gizmos.DrawWireSphere(posicionInicio + Vector3.down * distanciaDeteccionSuelo, radioEsfera);
            Gizmos.DrawLine(posicionInicio, posicionInicio + Vector3.down * distanciaDeteccionSuelo);
        }
        else
        {
            // Visualizar Raycast
            Gizmos.DrawLine(posicionInicio, posicionInicio + Vector3.down * distanciaDeteccionSuelo);
        }
    }

    // Método helper para debug
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
