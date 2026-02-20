using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Control de música con Toggle (para escena de registro)
/// </summary>
public class MusicaToggle : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Toggle que controla la música")]
    public Toggle toggleMusica;
    
    [Tooltip("AudioSource de la música")]
    public AudioSource audioSourceMusica;

    private const string PREF_MUSICA = "MusicaActivada";

    void Start()
    {
        if (toggleMusica != null)
        {
            // Cargar estado guardado (por defecto activado)
            bool musicaActivada = PlayerPrefs.GetInt(PREF_MUSICA, 1) == 1;
            toggleMusica.isOn = musicaActivada;

            // Aplicar estado
            AplicarEstado(musicaActivada);

            // Suscribirse al cambio
            toggleMusica.onValueChanged.AddListener(AlCambiarToggle);
        }
    }

    void AlCambiarToggle(bool activado)
    {
        AplicarEstado(activado);
        GuardarEstado(activado);
    }

    void AplicarEstado(bool activado)
    {
        if (audioSourceMusica != null)
        {
            if (activado)
            {
                if (!audioSourceMusica.isPlaying)
                {
                    audioSourceMusica.Play();
                }
            }
            else
            {
                audioSourceMusica.Pause();
            }
        }
    }

    void GuardarEstado(bool activado)
    {
        PlayerPrefs.SetInt(PREF_MUSICA, activado ? 1 : 0);
        PlayerPrefs.Save();
    }

    void OnDestroy()
    {
        if (toggleMusica != null)
        {
            toggleMusica.onValueChanged.RemoveListener(AlCambiarToggle);
        }
    }
}