using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Control de música con Button (para niveles)
/// Cambia sprite según estado
/// </summary>
public class MusicaBoton : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Button que controla la música")]
    public Button botonMusica;
    
    [Tooltip("Image del botón (para cambiar sprite)")]
    public Image imagenBoton;
    
    [Tooltip("AudioSource de la música")]
    public AudioSource audioSourceMusica;

    [Header("Sprites")]
    [Tooltip("Sprite cuando la música está activada")]
    public Sprite spriteActivado;
    
    [Tooltip("Sprite cuando la música está desactivada")]
    public Sprite spriteDesactivado;

    private const string PREF_MUSICA = "MusicaActivada";
    private bool musicaActivada;

    void Start()
    {
        if (botonMusica != null)
        {
            // Cargar estado guardado
            musicaActivada = PlayerPrefs.GetInt(PREF_MUSICA, 1) == 1;

            // Aplicar estado inicial
            AplicarEstado(musicaActivada);

            // Suscribirse al botón
            botonMusica.onClick.AddListener(AlPresionarBoton);
        }

        if (imagenBoton == null && botonMusica != null)
        {
            imagenBoton = botonMusica.GetComponent<Image>();
        }
    }

    void AlPresionarBoton()
    {
        // Alternar estado
        musicaActivada = !musicaActivada;
        AplicarEstado(musicaActivada);
        GuardarEstado(musicaActivada);
    }

    void AplicarEstado(bool activado)
    {
        // Controlar música
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

        // Cambiar sprite
        if (imagenBoton != null)
        {
            imagenBoton.sprite = activado ? spriteActivado : spriteDesactivado;
        }
    }

    void GuardarEstado(bool activado)
    {
        PlayerPrefs.SetInt(PREF_MUSICA, activado ? 1 : 0);
        PlayerPrefs.Save();
    }

    void OnDestroy()
    {
        if (botonMusica != null)
        {
            botonMusica.onClick.RemoveListener(AlPresionarBoton);
        }
    }
}