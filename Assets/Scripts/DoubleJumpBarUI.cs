using UnityEngine;
using UnityEngine.UI;

/// Barra del poder de doble salto.
/// Aparece en la misma posición que el escudo. Si ambos están activos, sube para dejar espacio.
public class DoubleJumpBarUI : MonoBehaviour
{
    [Header("Referencias UI")]
    [SerializeField] RectTransform fillRect;
    [SerializeField] CanvasGroup   panelGroup;

    [Header("Colores")]
    [SerializeField] Color colorFull    = new Color(1f, 0.65f, 0.1f, 1f);
    [SerializeField] Color colorLow     = new Color(1f, 0.2f,  0.1f, 1f);
    [SerializeField] float lowThreshold = 0.3f;

    [Header("Animación")]
    [SerializeField] float fadeSpeed = 6f;
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float stackOffset = 50f;   // cuánto sube cuando hay dos barras activas

    PlayerDoubleJump power;
    PlayerShield     shield;
    RawImage         fillImage;
    RectTransform    panelRT;
    float            targetAlpha;
    float            baseY;

    void Start()
    {
        fillImage = fillRect != null ? fillRect.GetComponent<RawImage>() : null;
        panelRT   = GetComponent<RectTransform>();
        baseY     = panelRT != null ? panelRT.anchoredPosition.y : 516f;

        targetAlpha               = 0f;
        if (panelGroup != null)
        {
            panelGroup.alpha          = 0f;
            panelGroup.interactable   = false;
            panelGroup.blocksRaycasts = false;
        }

        SetFill(1f);

        TryBindPlayer();
    }

    void TryBindPlayer()
    {
        if (power != null) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        power  = player.GetComponent<PlayerDoubleJump>();
        shield = player.GetComponent<PlayerShield>();

        if (power == null) return;

        power.OnPowerActivated += Show;
        power.OnPowerExpired   += Hide;
    }

    void OnDestroy()
    {
        if (power == null) return;
        power.OnPowerActivated -= Show;
        power.OnPowerExpired   -= Hide;
    }

    void Update()
    {
        // Fade
        if (panelGroup != null)
            panelGroup.alpha = Mathf.Lerp(panelGroup.alpha, targetAlpha, Time.deltaTime * fadeSpeed);

        if (power == null)
        {
            TryBindPlayer();
            return;
        }

        // Posición: si escudo también activo, subir para que se vean las dos
        if (panelRT != null)
        {
            bool bothActive = power.IsActive && shield != null && shield.IsShielded;
            float targetY   = bothActive ? baseY - stackOffset : baseY;
            Vector2 pos     = panelRT.anchoredPosition;
            pos.y           = Mathf.Lerp(pos.y, targetY, Time.deltaTime * moveSpeed);
            panelRT.anchoredPosition = pos;
        }

        if (!power.IsActive) return;
        SetFill(power.PowerTimeNormalized);
    }

    void SetFill(float t)
    {
        if (fillRect != null)
        {
            Vector2 aMax       = fillRect.anchorMax;
            aMax.x             = Mathf.Clamp01(t);
            fillRect.anchorMax = aMax;
        }
        if (fillImage != null)
            fillImage.color = t <= lowThreshold
                ? Color.Lerp(colorLow, colorFull, t / lowThreshold)
                : colorFull;
    }

    void Show()
    {
        targetAlpha               = 1f;
        panelGroup.interactable   = false;
        panelGroup.blocksRaycasts = false;
        SetFill(1f);
    }

    void Hide() => targetAlpha = 0f;
}
