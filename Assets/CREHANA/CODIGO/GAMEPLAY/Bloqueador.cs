using UnityEngine;

public class Bloqueador : MonoBehaviour
{
    void Start()
    {
        // Por si el objeto ya estaba activo antes de que InputManager existiera
        if (InputManager.Instance != null)
            InputManager.Instance.InputBloqueado = true;
    }

    void OnEnable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.InputBloqueado = true;
    }

    void OnDisable()
    {
        if (InputManager.Instance != null)
            InputManager.Instance.InputBloqueado = false;
    }
}