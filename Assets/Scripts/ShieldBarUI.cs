using UnityEngine;
using UnityEngine.UI;

/// Barra de escudo arriba a la derecha. Usa RawImage igual que el contador de monedas.
public class ShieldBarUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] RectTransform fillRect;   // RectTransform del fill (su anchorMax.x = 0..1)
    [SerializeField] CanvasGroup   panelGroup;

    [Header("Colores")]
    [SerializeField] Color colorFull = new Color(0.25f, 0.78f, 1f, 1f);
    [SerializeField] Color colorLow  = new Color(1f, 0.35f, 0.2f, 1f);
    [SerializeField] float lowThreshold = 0.3f;

    [Header("Animacion")]
    [SerializeField] float fadeSpeed = 6f;

    PlayerShield shield;
    RawImage     fillImage;
    float        targetAlpha;

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) shield = player.GetComponent<PlayerShield>();

        if (shield == null)
        {
            Debug.LogWarning("[ShieldBarUI] No encontre PlayerShield en el jugador.");
            return;
        }

        fillImage = fillRect != null ? fillRect.GetComponent<RawImage>() : null;

        shield.OnShieldActivated += Show;
        shield.OnShieldExpired   += Hide;

        // Oculto al inicio
        targetAlpha          = 0f;
        panelGroup.alpha     = 0f;
        panelGroup.interactable   = false;
        panelGroup.blocksRaycasts = false;

        SetFill(1f);
    }

    void OnDestroy()
    {
        if (shield == null) return;
        shield.OnShieldActivated -= Show;
        shield.OnShieldExpired   -= Hide;
    }

    void Update()
    {
        // Fade suave
        panelGroup.alpha = Mathf.Lerp(panelGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);

        if (shield == null || !shield.IsShielded) return;

        SetFill(shield.ShieldTimeNormalized);
    }

    void SetFill(float t)
    {
        if (fillRect != null)
        {
            // Ancho de la barra por ancla: de 0 a 1
            Vector2 aMax  = fillRect.anchorMax;
            aMax.x        = Mathf.Clamp01(t);
            fillRect.anchorMax = aMax;
        }

        if (fillImage != null)
            fillImage.color = t <= lowThreshold
                ? Color.Lerp(colorLow, colorFull, t / lowThreshold)
                : colorFull;
    }

    void Show()
    {
        targetAlpha = 1f;
        panelGroup.interactable   = false;
        panelGroup.blocksRaycasts = false;
        SetFill(1f);
    }

    void Hide() => targetAlpha = 0f;
}
