using System.Collections.Generic;
using UnityEngine;

public class NPCInteractuable : MonoBehaviour
{
    [Header("Animaciones")]
    [Tooltip("Clip inicial que se reproduce en IdleA al inicio.")]
    public AnimationClip clipIdleInicial;

    [Tooltip("Clip que se reproduce en IdleB tras la interacción.")]
    public AnimationClip clipIdleSecundario;

    [Header("Transform al Interactuar")]
    public bool aplicarTransform = false;
    public Transform transformObjetivo;

    [Header("Objetos a Activar")]
    public List<GameObject> objetosAActivar = new List<GameObject>();

    [Header("Objetos a Apagar")]
    public List<GameObject> objetosAApagar = new List<GameObject>();

    [Header("Configuración")]
    public string tagJugador = "Player";

    private Animator anim;
    private AnimatorOverrideController overrideController;
    private Collider col;

    void Awake()
    {
        anim = GetComponent<Animator>();
        col  = GetComponent<Collider>();

        if (anim == null || anim.runtimeAnimatorController == null) return;

        overrideController = new AnimatorOverrideController(anim.runtimeAnimatorController);
        anim.runtimeAnimatorController = overrideController;

        // Obtener los slots reales del controller base
        var overrides = new List<KeyValuePair<AnimationClip, AnimationClip>>();
        overrideController.GetOverrides(overrides);

        // Reemplazar usando los clips originales del controller base como clave
        for (int i = 0; i < overrides.Count; i++)
        {
            AnimationClip original = overrides[i].Key;
            if (original == null) continue;

            // El primer slot corresponde a IdleA, el segundo a IdleB
            if (i == 0 && clipIdleSecundario != null)
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(original, clipIdleSecundario);
            else if (i == 1 && clipIdleInicial != null)
                overrides[i] = new KeyValuePair<AnimationClip, AnimationClip>(original, clipIdleInicial);
        }

        overrideController.ApplyOverrides(overrides);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag(tagJugador)) return;

        if (anim != null)
            anim.SetBool("Secundaria", true);

        if (aplicarTransform && transformObjetivo != null)
        {
            transform.position   = transformObjetivo.position;
            transform.rotation   = transformObjetivo.rotation;
            transform.localScale = transformObjetivo.localScale;
        }

        foreach (GameObject obj in objetosAActivar)
            if (obj != null) obj.SetActive(true);

        foreach (GameObject obj in objetosAApagar)
            if (obj != null) obj.SetActive(false);

        if (col != null) col.enabled = false;
    }
}