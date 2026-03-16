using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

/// <summary>
/// Persiste entre escenas y avanza a la siguiente al presionar F.
/// Ponlo en un GameObject desde la primera escena; no hace falta repetirlo.
/// </summary>
public class AtajoSiguienteEscena : MonoBehaviour
{
    void Awake()
    {
        if (FindObjectsByType<AtajoSiguienteEscena>(FindObjectsSortMode.None).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            int siguiente = SceneManager.GetActiveScene().buildIndex + 1;

            if (siguiente < SceneManager.sceneCountInBuildSettings)
                SceneManager.LoadScene(siguiente);
            else
                Debug.Log("⚠️ ATAJO: Ya estás en la última escena del Build Settings.");
        }
    }
}