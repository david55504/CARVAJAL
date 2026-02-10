using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneResult : MonoBehaviour
{
    public event Action<bool> OnResultSaved; 

    public Decision sceneResult;
    [SerializeField] int sceneToLoadOnSave = 0;



    void Start()
    {
        // IMPORTANTE: Verificar que tenemos un usuario al iniciar la escena
        if (!APIManager.Instance.TieneUsuarioRegistrado())
        {
            Debug.LogError("No hay usuario registrado. El jugador debe registrarse primero.");
            // Opcional: redirigir a la escena de registro
            // UnityEngine.SceneManagement.SceneManager.LoadScene("RegistroEscena");
        }
        else
        {
            Debug.Log("Usuario verificado: " + APIManager.Instance.GetUserId());
        }
    }
    public void RegistrarDecision(Decision decision)
    {
        // Verificar nuevamente antes de guardar
        if (!APIManager.Instance.TieneUsuarioRegistrado())
        {
            Debug.LogError("No se puede guardar decisión. No hay usuario registrado.");
            return;
        }

        Debug.Log($"Registrando decisión - Escena: {decision.escena}, Tipo: {decision.tipoDecision}, Valor: {decision.valorSeleccionado}");

        StartCoroutine(APIManager.Instance.GuardarDecision(decision, OnDecisionGuardada));
    }
    public void RegistrarDecision(string escena, string tipoDecision, string valor)
    {
        // Verificar nuevamente antes de guardar
        if (!APIManager.Instance.TieneUsuarioRegistrado())
        {
            Debug.LogError("No se puede guardar decisión. No hay usuario registrado.");
            return;
        }

        Debug.Log($"Registrando decisión - Escena: {escena}, Tipo: {tipoDecision}, Valor: {valor}");

        Decision decision = new Decision
        {
            escena = escena,
            tipoDecision = tipoDecision,
            valorSeleccionado = valor
        };

        StartCoroutine(APIManager.Instance.GuardarDecision(decision, OnDecisionGuardada));
    }

    void OnDecisionGuardada(bool exito)
    {
        OnResultSaved?.Invoke(exito);
        if (exito)
        {
            Debug.Log("Decisión registrada correctamente");
        }
        else
        {
            Debug.LogWarning("No se pudo guardar la decisión");
        }

        SceneManager.LoadScene(sceneToLoadOnSave);
    }

    // Ejemplo de uso
    [ContextMenu("Guardar Descision Escena")]
    public void OnBotonOpcionAClick()
    {
        RegistrarDecision(sceneResult);
    }


}
