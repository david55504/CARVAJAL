using UnityEngine;

/// <summary>
/// Rotación automática simple con control independiente por ejes
/// </summary>
public class RotacionAutomatica : MonoBehaviour
{
    [Header("Configuración de Rotación")]
    [Tooltip("Velocidad de rotación en grados por segundo")]
    public float velocidadRotacion = 50f;

    [Header("Ejes de Rotación")]
    [Tooltip("Rotar en el eje X")]
    public bool rotarX = false;
    
    [Tooltip("Rotar en el eje Y")]
    public bool rotarY = true;
    
    [Tooltip("Rotar en el eje Z")]
    public bool rotarZ = false;

    void Update()
    {
        // Calcular la rotación para este frame
        float rotacionFrame = velocidadRotacion * Time.deltaTime;

        // Crear vector de rotación según los ejes activados
        Vector3 rotacion = new Vector3(
            rotarX ? rotacionFrame : 0f,
            rotarY ? rotacionFrame : 0f,
            rotarZ ? rotacionFrame : 0f
        );

        // Aplicar la rotación
        transform.Rotate(rotacion);
    }
}