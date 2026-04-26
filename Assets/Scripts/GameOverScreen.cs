using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// Pantalla de Game Over con distancia actual y mejor distancia.
/// Se construye sola en Awake() — no necesita configuración en el Inspector.
public class GameOverScreen : MonoBehaviour
{
    public static GameOverScreen Instance { get; private set; }

    const string BestKey = "BestDistance";

    TMP_Text distText;
    TMP_Text bestText;
    TMP_Text newBestText;

    void Awake()
    {
        Instance = this;
        BuildUI();
        gameObject.SetActive(false);
    }

    void BuildUI()
    {
        // Canvas propio (overlay encima de todo)
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 99;

        CanvasScaler cs = gameObject.AddComponent<CanvasScaler>();
        cs.uiScaleMode        = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        gameObject.AddComponent<GraphicRaycaster>();

        // Panel oscuro semitransparente
        GameObject panel = new GameObject("Panel");
        panel.transform.SetParent(transform, false);
        Image img = panel.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.78f);
        Stretch(panel.GetComponent<RectTransform>());

        // GAME OVER
        AddLabel(panel.transform, "LblTitle", "GAME OVER",
                 new Vector2(0f, 120f), 90, Color.white, FontStyles.Bold);

        // Distancia esta partida
        distText = AddLabel(panel.transform, "LblDist", "",
                            new Vector2(0f, 20f), 54, new Color(1f, 0.85f, 0.2f));

        // Mejor distancia
        bestText = AddLabel(panel.transform, "LblBest", "",
                            new Vector2(0f, -55f), 42, new Color(0.55f, 1f, 0.55f));

        // Nuevo récord (aparece solo si se supera)
        newBestText = AddLabel(panel.transform, "LblRecord", "¡NUEVO RÉCORD!",
                               new Vector2(0f, -130f), 40,
                               new Color(1f, 0.45f, 0f), FontStyles.Bold);
        newBestText.gameObject.SetActive(false);
    }

    // ── Helpers de UI ────────────────────────────────────────────────────────

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    static TMP_Text AddLabel(Transform parent, string name, string text,
                             Vector2 anchoredPos, float fontSize,
                             Color color, FontStyles style = FontStyles.Normal)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent, false);

        TMP_Text t = go.AddComponent<TextMeshProUGUI>();
        t.text      = text;
        t.fontSize  = fontSize;
        t.color     = color;
        t.fontStyle = style;
        t.alignment = TextAlignmentOptions.Center;

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0.5f, 0.5f);
        rt.anchorMax        = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta        = new Vector2(900f, 100f);
        return t;
    }

    // ── API pública ───────────────────────────────────────────────────────────

    /// Muestra la pantalla con la distancia actual y actualiza el récord.
    public void Show(int dist)
    {
        int best  = PlayerPrefs.GetInt(BestKey, 0);
        bool isNew = dist > best;

        if (isNew)
        {
            best = dist;
            PlayerPrefs.SetInt(BestKey, best);
            PlayerPrefs.Save();
        }

        distText.text = "DISTANCIA:       " + dist + " m";
        bestText.text  = "MEJOR DISTANCIA: " + best + " m";
        newBestText.gameObject.SetActive(isNew);

        gameObject.SetActive(true);
    }
}
