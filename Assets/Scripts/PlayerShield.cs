using UnityEngine;

/// Gestiona el estado del escudo del jugador y su efecto visual.
/// Coloca este componente en el mismo GameObject que PlayerMovement.
public class PlayerShield : MonoBehaviour
{
    [Header("Visual")]
    [SerializeField] float shieldScale    = 2.4f;
    [SerializeField] Color shieldColor    = new Color(0.25f, 0.75f, 1f, 0.28f);
    [SerializeField] float pulseSpeed     = 2.2f;
    [SerializeField] float pulseAmount    = 0.07f;
    [SerializeField] Color emissionColor  = new Color(0.1f, 0.55f, 1f);
    [SerializeField] float emissionIntensity = 1.2f;

    [Header("Duraci\u00f3n")]
    [SerializeField] float minDuration = 5f;
    [SerializeField] float maxDuration = 10f;

    public bool  IsShielded          { get; private set; }
    /// Tiempo restante normalizado 0→1 (1 = recién activado, 0 = expirado).
    public float ShieldTimeNormalized { get; private set; }

    public event System.Action OnShieldActivated;
    public event System.Action OnShieldExpired;

    GameObject shieldVisual;
    Material   shieldMat;
    float      shieldTimer;
    float      shieldDuration;

    void Awake()
    {
        BuildVisual();
        shieldVisual.SetActive(false);
    }

    void BuildVisual()
    {
        shieldVisual = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        shieldVisual.name = "ShieldVisual";
        shieldVisual.transform.SetParent(transform, false);
        shieldVisual.transform.localPosition = new Vector3(0f, 0.5f, 0f);
        shieldVisual.transform.localScale    = Vector3.one * shieldScale;

        // Sin collider para no interferir con nada
        Destroy(shieldVisual.GetComponent<Collider>());

        // Intentar usar el shader CrystalShield; si no carga (editor sin compilar), caer al URP/Lit
        Shader crystalShader = Shader.Find("Custom/CrystalShield");
        bool   useCrystal    = crystalShader != null;

        if (useCrystal)
        {
            shieldMat = new Material(crystalShader);
            shieldMat.SetColor("_BaseColor",     shieldColor);
            shieldMat.SetColor("_EmissionColor", emissionColor);
            shieldMat.SetFloat("_EmissionPower", emissionIntensity * 1.5f);
        }
        else
        {
            // Fallback: URP/Lit transparente (mismo comportamiento anterior)
            shieldMat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            shieldMat.SetFloat("_Surface",   1f);
            shieldMat.SetFloat("_Blend",     0f);
            shieldMat.SetFloat("_SrcBlend",  5f);
            shieldMat.SetFloat("_DstBlend", 10f);
            shieldMat.SetFloat("_ZWrite",    0f);
            shieldMat.SetFloat("_AlphaClip", 0f);
            shieldMat.renderQueue = 3000;
            shieldMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
            shieldMat.color = shieldColor;
            shieldMat.EnableKeyword("_EMISSION");
            shieldMat.SetColor("_EmissionColor", emissionColor * emissionIntensity);
        }

        shieldVisual.GetComponent<Renderer>().material = shieldMat;
    }

    void Update()
    {
        if (!IsShielded) return;

        // Pulso suave de escala
        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        shieldVisual.transform.localScale = Vector3.one * shieldScale * pulse;

        // Temporizador: se apaga solo al vencer el tiempo
        shieldTimer -= Time.deltaTime;
        ShieldTimeNormalized = Mathf.Clamp01(shieldTimer / shieldDuration);
        if (shieldTimer <= 0f)
            AbsorbHit();
    }

    /// Activa el escudo con duración aleatoria entre minDuration y maxDuration.
    public void Activate()
    {
        shieldDuration           = Random.Range(minDuration, maxDuration);
        shieldTimer              = shieldDuration;
        ShieldTimeNormalized     = 1f;
        IsShielded               = true;
        shieldVisual.SetActive(true);
        OnShieldActivated?.Invoke();
        Debug.Log($"[PlayerShield] Escudo activado por {shieldDuration:F1}s.");
    }

    /// Absorbe un golpe o expira y desactiva el escudo.
    public void AbsorbHit()
    {
        IsShielded           = false;
        ShieldTimeNormalized = 0f;
        shieldVisual.SetActive(false);
        OnShieldExpired?.Invoke();

        // Puff azul al expirar
        PowerExpireEffect.Spawn(transform.position + Vector3.up * 0.8f,
                                new Color(0.25f, 0.75f, 1f));

        Debug.Log("[PlayerShield] Escudo desactivado.");
    }
}
