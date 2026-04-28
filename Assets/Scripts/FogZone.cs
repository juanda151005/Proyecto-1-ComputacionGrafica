using UnityEngine;

// Activa niebla densa de tormenta con transicion suave al entrar y salir del trigger.
// Guarda el estado previo del RenderSettings para restaurarlo al salir.
public class FogZone : MonoBehaviour
{
    [SerializeField] Color  fogColor       = new Color(0.85f, 0.7f, 0.5f, 1f);
    [SerializeField] float  fogDensity     = 0.06f;
    [SerializeField] FogMode fogMode       = FogMode.ExponentialSquared;
    [SerializeField] float  transitionSpeed = 1.8f; // velocidad del fade in/out

    bool  playerInside;
    float baseDensity;   // densidad original de la escena
    Color baseColor;     // color original
    bool  baseFogOn;
    FogMode baseMode;

    void Start()
    {
        // Guardamos al inicio para no depender del momento exacto de la colision
        baseFogOn   = RenderSettings.fog;
        baseColor   = RenderSettings.fogColor;
        baseDensity = RenderSettings.fogDensity;
        baseMode    = RenderSettings.fogMode;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!IsPlayer(other)) return;
        playerInside = true;
        RenderSettings.fog     = true;
        RenderSettings.fogMode = fogMode;
    }

    void OnTriggerExit(Collider other)
    {
        if (!IsPlayer(other)) return;
        playerInside = false;
    }

    void Update()
    {
        float targetDensity = playerInside ? fogDensity : baseDensity;
        Color targetColor   = playerInside ? fogColor   : baseColor;

        RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity, targetDensity, Time.deltaTime * transitionSpeed);
        RenderSettings.fogColor   = Color.Lerp(RenderSettings.fogColor,   targetColor,   Time.deltaTime * transitionSpeed);

        // Al salir completamente, restauramos el flag de fog al original
        if (!playerInside && Mathf.Abs(RenderSettings.fogDensity - baseDensity) < 0.0005f)
        {
            RenderSettings.fog        = baseFogOn;
            RenderSettings.fogDensity = baseDensity;
            RenderSettings.fogColor   = baseColor;
            RenderSettings.fogMode    = baseMode;
        }
    }

    bool IsPlayer(Collider other)
    {
        if (other.CompareTag("Player")) return true;
        return other.GetComponentInParent<PlayerMovement>() != null;
    }
}
