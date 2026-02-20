using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Cronómetro circular simple usando fillAmount
/// Se reinicia automáticamente cada vez que se activa el objeto
/// </summary>
public class CronometroCircular : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Tiempo del cronómetro en segundos")]
    public float tiempoCronometro = 10f;

    [Tooltip("Image del círculo (debe tener Image Type: Filled)")]
    public Image circuloCronometro;

    private float tiempoRestante;
    private bool cronometroActivo = false;

    void OnEnable()
    {
        // Reiniciar cada vez que se activa
        ReiniciarCronometro();
    }

    void Update()
    {
        if (cronometroActivo)
        {
            tiempoRestante -= Time.deltaTime;

            // Actualizar fillAmount (1 a 0)
            if (circuloCronometro != null)
            {
                circuloCronometro.fillAmount = tiempoRestante / tiempoCronometro;
            }

            // Verificar si terminó
            if (tiempoRestante <= 0f)
            {
                tiempoRestante = 0f;
                cronometroActivo = false;
                
                if (circuloCronometro != null)
                {
                    circuloCronometro.fillAmount = 0f;
                }
            }
        }
    }

    void ReiniciarCronometro()
    {
        tiempoRestante = tiempoCronometro;
        cronometroActivo = true;

        if (circuloCronometro != null)
        {
            circuloCronometro.fillAmount = 1f;
        }
    }

    /// <summary>
    /// Método público para reiniciar manualmente
    /// </summary>
    public void Reiniciar()
    {
        ReiniciarCronometro();
    }

    /// <summary>
    /// Método público para pausar
    /// </summary>
    public void Pausar()
    {
        cronometroActivo = false;
    }

    /// <summary>
    /// Método público para reanudar
    /// </summary>
    public void Reanudar()
    {
        cronometroActivo = true;
    }
}