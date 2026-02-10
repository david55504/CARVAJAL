using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class APIManager : MonoBehaviour
{
    private static APIManager instance;
    public static APIManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("APIManager");
                instance = go.AddComponent<APIManager>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private string apiURL = "https://www.miguelprdeveloper.com/api/usuarios.php";
    private const string USER_ID_KEY = "userId";


    void Awake()
    {
        // Prevenir duplicados
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        // Debug para verificar si hay un ID guardado
        Debug.Log("APIManager inicializado. UserID actual: " + GetUserId());
    }
    public string GetUserId()
    {
        // SIEMPRE lee desde PlayerPrefs para asegurar persistencia
        string userId = PlayerPrefs.GetString(USER_ID_KEY, "");

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogWarning("No hay usuario registrado en PlayerPrefs");
        }
        else
        {
            Debug.Log("Usuario ID recuperado: " + userId);
        }

        return userId;
    }

    public void SetUserId(string userId)
    {
        PlayerPrefs.SetString(USER_ID_KEY, userId);
        PlayerPrefs.Save(); // IMPORTANTE: Forzar guardado inmediato
        Debug.Log("Usuario ID guardado: " + userId);
    }
    public bool TieneUsuarioRegistrado()
    {
        return !string.IsNullOrEmpty(GetUserId());
    }

    public void LimpiarUsuario()
    {
        PlayerPrefs.DeleteKey(USER_ID_KEY);
        PlayerPrefs.Save();
        Debug.Log("Usuario eliminado de PlayerPrefs");
    }

    public IEnumerator RegistrarUsuario(UserData userData, System.Action<bool, string> callback)
    {
        string json = JsonUtility.ToJson(userData);
        Debug.Log("Enviando registro: " + json);

        using (UnityWebRequest request = new UnityWebRequest(apiURL, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Respuesta del servidor: " + request.downloadHandler.text);

                RegistroResponse response = JsonUtility.FromJson<RegistroResponse>(request.downloadHandler.text);

                if (response.success)
                {
                    // Guardar usando el método SetUserId
                    SetUserId(response.userId);

                    Debug.Log("Usuario registrado exitosamente. ID: " + response.userId);
                    callback?.Invoke(true, response.userId);
                }
                else
                {
                    Debug.LogError("Error en registro: " + response.mensaje);
                    callback?.Invoke(false, "");
                }
            }
            else
            {
                Debug.LogError("Error de red al registrar: " + request.error);
                Debug.LogError("Código de respuesta: " + request.responseCode);
                callback?.Invoke(false, "");
            }
        }
    }

    public IEnumerator GuardarDecision(Decision decision, System.Action<bool> callback)
    {
        string userId = GetUserId();

        if (string.IsNullOrEmpty(userId))
        {
            Debug.LogError("❌ No hay usuario registrado. No se puede guardar la decisión.");
            callback?.Invoke(false);
            yield break;
        }

        Debug.Log("Guardando decisión para usuario: " + userId);

        DecisionRequest requestData = new DecisionRequest
        {
            userId = userId,
            decision = decision
        };

        string json = JsonUtility.ToJson(requestData);
        Debug.Log("JSON enviado: " + json);

        using (UnityWebRequest request = new UnityWebRequest(apiURL, "PUT"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("Respuesta del servidor: " + request.downloadHandler.text);

                DecisionResponse response = JsonUtility.FromJson<DecisionResponse>(request.downloadHandler.text);
                Debug.Log("✅ Decisión guardada: " + response.mensaje);
                callback?.Invoke(true);
            }
            else
            {
                Debug.LogError("❌ Error al guardar decisión: " + request.error);
                Debug.LogError("Código de respuesta: " + request.responseCode);
                callback?.Invoke(false);
            }
        }
    }
}

