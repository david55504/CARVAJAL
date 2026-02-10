using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class RegistroUI : MonoBehaviour
{
    public TMP_InputField nombreInput;
    public TMP_InputField correoInput;
    public TMP_InputField telefonoInput;
    public Button registrarButton;
    public TextMeshProUGUI mensajeText;

    void Start()
    {
        registrarButton.onClick.AddListener(OnRegistrarClick);

        // Verificar si ya hay un usuario registrado
        if (APIManager.Instance.TieneUsuarioRegistrado())
        {
            Debug.Log("Ya hay un usuario registrado: " + APIManager.Instance.GetUserId());
            mensajeText.text = "Ya estás registrado. ID: " + APIManager.Instance.GetUserId();

            // Opcional: saltar automáticamente al juego
            // CargarJuego();
        }
    }

    void OnRegistrarClick()
    {
        // Validar campos
        if (string.IsNullOrEmpty(nombreInput.text) ||
            string.IsNullOrEmpty(correoInput.text) ||
            string.IsNullOrEmpty(telefonoInput.text))
        {
            mensajeText.text = "Por favor completa todos los campos";
            return;
        }

        registrarButton.interactable = false;
        mensajeText.text = "Registrando...";

        UserData userData = new UserData
        {
            nombre = nombreInput.text,
            correo = correoInput.text,
            telefono = telefonoInput.text
        };

        StartCoroutine(APIManager.Instance.RegistrarUsuario(userData, OnRegistroCompleto));
    }

    void OnRegistroCompleto(bool exito, string userId)
    {
        if (exito)
        {
            mensajeText.text = "¡Registro exitoso! ID: " + userId;
            Debug.Log("Registro completado. Cambiando de escena en 2 segundos...");

            // Verificar que se guardó correctamente
            string verificacion = APIManager.Instance.GetUserId();
            Debug.Log("Verificación inmediata del ID: " + verificacion);

            // Cargar siguiente escena
            Invoke("CargarJuego", 2f);
        }
        else
        {
            mensajeText.text = "Error al registrar. Intenta de nuevo.";
            registrarButton.interactable = true;
        }
    }

    void CargarJuego()
    {
        SceneManager.LoadScene("01_LOBBY"); // Cambia por el nombre de tu escena
    }
}
