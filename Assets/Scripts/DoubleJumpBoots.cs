using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerDoubleJump))]
public class DoubleJumpBoots : MonoBehaviour
{
    [Header("Colores")]
    [SerializeField] Color coreColor   = new Color(1f,    0.72f, 0.05f);
    [SerializeField] Color accentColor = new Color(1f,    0.40f, 0.02f);
    [SerializeField] Color armorColor  = new Color(0.04f, 0.03f, 0.01f);
    [SerializeField] float glowPower   = 10f;

    [Header("Shaders")]
    [SerializeField] Shader unlitShader;
    [SerializeField] Shader litShader;
    [SerializeField] Shader particleUnlitShader;

    [Header("Posición")]
    [SerializeField] float downOffset  = 0.12f;

    Transform  leftFoot,  rightFoot;
    GameObject leftRoot,  rightRoot;
    Light      leftLight, rightLight;

    Material glowA, glowB, armorMat;

    float pulseT;
    bool  active;

    readonly List<Transform> orbitRings = new List<Transform>();

    void Start()
    {
        var dj = GetComponent<PlayerDoubleJump>();
        dj.OnPowerActivated += ShowBoots;
        dj.OnPowerExpired   += HideBoots;

        glowA    = UnlitMat(coreColor,   glowPower);
        glowB    = UnlitMat(accentColor, glowPower * 0.8f);
        armorMat = ArmorMat();

        leftFoot  = FindBone("mixamorig:LeftFoot");
        rightFoot = FindBone("mixamorig:RightFoot");

        if (leftFoot == null || rightFoot == null)
            Debug.LogWarning("[DoubleJumpBoots] Huesos de pie no encontrados. Revisa los nombres del rig.");

        leftRoot  = BuildBoot(false);
        rightRoot = BuildBoot(true);
        HideBoots();
    }

    // LateUpdate corre despues de que la animacion del personaje mueve los huesos,
    // evitando que las botas queden un frame atrasadas.
    void LateUpdate()
    {
        if (!active) return;

        pulseT += Time.deltaTime * 3.5f;
        float p = 0.65f + Mathf.Sin(pulseT) * 0.35f;
        glowA.SetColor("_BaseColor", coreColor   * (glowPower * p));
        glowB.SetColor("_BaseColor", accentColor * (glowPower * 0.8f * p));

        for (int i = 0; i < orbitRings.Count; i++)
        {
            float spd = (i % 3 == 0) ? 240f : (i % 3 == 1) ? -180f : 300f;
            Vector3 axis = (i % 2 == 0) ? Vector3.up : Vector3.forward;
            orbitRings[i].Rotate(axis, spd * Time.deltaTime, Space.World);
        }

        Follow(leftFoot,  leftRoot,  leftLight);
        Follow(rightFoot, rightRoot, rightLight);
    }

    void Follow(Transform foot, GameObject root, Light lt)
    {
        if (foot == null || root == null) return;

        root.transform.position = foot.position + Vector3.down * downOffset;

        Vector3 fwd = transform.forward; fwd.y = 0f;
        if (fwd.sqrMagnitude > 0.001f)
            root.transform.rotation = Quaternion.LookRotation(fwd, Vector3.up);

        if (lt != null) lt.transform.position = foot.position + Vector3.up * 0.08f;
    }

    GameObject BuildBoot(bool isRight)
    {
        var root = new GameObject(isRight ? "DJBoot_R" : "DJBoot_L");

        Cyl(root, "Sole",
            new Vector3(0f,  0.01f, 0.02f),
            new Vector3(0.28f, 0.04f, 0.44f), glowA);

        Cyl(root, "SoleAccent",
            new Vector3(0f, -0.012f, 0.02f),
            new Vector3(0.24f, 0.025f, 0.38f), glowB);

        Box(root, "ArmorCore",
            new Vector3(0f,    0.11f,  0.01f),
            new Vector3(0.22f, 0.20f, 0.34f), armorMat);

        Box(root, "PlateL",
            new Vector3(-0.13f, 0.10f, 0.01f),
            new Vector3(0.05f, 0.18f, 0.30f), armorMat);
        Box(root, "PlateR",
            new Vector3( 0.13f, 0.10f, 0.01f),
            new Vector3(0.05f, 0.18f, 0.30f), armorMat);

        Box(root, "Toe",
            new Vector3(0f, 0.09f, 0.18f),
            new Vector3(0.20f, 0.15f, 0.08f), armorMat);

        Box(root, "Heel",
            new Vector3(0f, 0.08f, -0.17f),
            new Vector3(0.16f, 0.12f, 0.09f), armorMat);

        Box(root, "StripeTop",
            new Vector3(0f, 0.15f, 0.01f),
            new Vector3(0.23f, 0.028f, 0.35f), glowA);

        Box(root, "StripeMid",
            new Vector3(0f, 0.07f, 0.01f),
            new Vector3(0.24f, 0.022f, 0.37f), glowB);

        Box(root, "StripeSideL",
            new Vector3(-0.15f, 0.10f, 0.01f),
            new Vector3(0.022f, 0.16f, 0.28f), glowA);
        Box(root, "StripeSideR",
            new Vector3( 0.15f, 0.10f, 0.01f),
            new Vector3(0.022f, 0.16f, 0.28f), glowA);

        Box(root, "ThrusterL",
            new Vector3(-0.07f, 0.17f, -0.18f),
            new Vector3(0.07f, 0.22f, 0.08f), armorMat);
        Box(root, "ThrusterR",
            new Vector3( 0.07f, 0.17f, -0.18f),
            new Vector3(0.07f, 0.22f, 0.08f), armorMat);

        Cyl(root, "NozzleL",
            new Vector3(-0.07f, 0.30f, -0.18f),
            new Vector3(0.08f, 0.035f, 0.08f), glowB);
        Cyl(root, "NozzleR",
            new Vector3( 0.07f, 0.30f, -0.18f),
            new Vector3(0.08f, 0.035f, 0.08f), glowB);

        Cyl(root, "FlameL",
            new Vector3(-0.07f, 0.34f, -0.18f),
            new Vector3(0.055f, 0.05f, 0.055f), glowA);
        Cyl(root, "FlameR",
            new Vector3( 0.07f, 0.34f, -0.18f),
            new Vector3(0.055f, 0.05f, 0.055f), glowA);

        var r1 = Cyl(root, "Ring1",
            new Vector3(0f, 0.16f, 0f),
            new Vector3(0.42f, 0.020f, 0.42f), glowA);
        orbitRings.Add(r1.transform);

        var r2 = Cyl(root, "Ring2",
            new Vector3(0f, 0.16f, 0f),
            new Vector3(0.34f, 0.017f, 0.34f), glowB);
        r2.transform.localRotation = Quaternion.Euler(50f, 30f, 0f);
        orbitRings.Add(r2.transform);

        var r3 = Cyl(root, "Ring3",
            new Vector3(0f, 0.16f, 0f),
            new Vector3(0.28f, 0.014f, 0.28f), glowA);
        r3.transform.localRotation = Quaternion.Euler(0f, 75f, 85f);
        orbitRings.Add(r3.transform);

        Cyl(root, "GroundGlow",
            new Vector3(0f, -0.10f, 0f),
            new Vector3(0.55f, 0.006f, 0.55f), glowA);

        Cyl(root, "GroundAccent",
            new Vector3(0f, -0.09f, 0f),
            new Vector3(0.38f, 0.005f, 0.38f), glowB);

        var ltGO = new GameObject("BootLight");
        ltGO.transform.SetParent(root.transform, false);
        ltGO.transform.localPosition = new Vector3(0f, 0.20f, 0f);
        var lt = ltGO.AddComponent<Light>();
        lt.type      = LightType.Point;
        lt.color     = coreColor;
        lt.intensity = 6f;
        lt.range     = 3.5f;
        lt.shadows   = LightShadows.None;
        if (isRight) rightLight = lt; else leftLight = lt;

        Sparks(root);
        JetParticles(root, -0.09f);
        JetParticles(root,  0.09f);
        EnergyTrail(root);

        return root;
    }

    void Sparks(GameObject parent)
    {
        var go = new GameObject("Sparks");
        go.transform.SetParent(parent.transform, false);
        go.transform.localPosition = new Vector3(0f, 0.01f, 0f);

        var ps   = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime   = new ParticleSystem.MinMaxCurve(0.4f, 1f);
        main.startSpeed      = new ParticleSystem.MinMaxCurve(1f, 3f);
        main.startSize       = new ParticleSystem.MinMaxCurve(0.04f, 0.10f);
        main.startColor      = new ParticleSystem.MinMaxGradient(
                                   new Color(1f, 0.95f, 0.4f),
                                   new Color(1f, 0.5f, 0.05f));
        main.maxParticles    = 100;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = 0.2f;

        var em = ps.emission; em.rateOverTime = 60f;

        var sh = ps.shape;
        sh.shapeType = ParticleSystemShapeType.Box;
        sh.scale     = new Vector3(0.25f, 0.015f, 0.42f);

        var col = ps.colorOverLifetime; col.enabled = true;
        var g   = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(new Color(1f, 0.95f, 0.3f), 0f),
                    new GradientColorKey(new Color(1f, 0.6f, 0.05f), 0.5f),
                    new GradientColorKey(new Color(1f, 0.2f, 0f), 1f) },
            new[] { new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.7f, 0.5f),
                    new GradientAlphaKey(0f, 1f) });
        col.color = new ParticleSystem.MinMaxGradient(g);

        go.GetComponent<ParticleSystemRenderer>().sharedMaterial =
            ParticleMat(new Color(1f, 0.8f, 0.1f));
    }

    void JetParticles(GameObject parent, float xOff)
    {
        var go = new GameObject("Jet");
        go.transform.SetParent(parent.transform, false);
        go.transform.localPosition = new Vector3(xOff, 0.31f, -0.18f);

        var ps   = go.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.startLifetime   = new ParticleSystem.MinMaxCurve(0.15f, 0.40f);
        main.startSpeed      = new ParticleSystem.MinMaxCurve(1.5f, 4f);
        main.startSize       = new ParticleSystem.MinMaxCurve(0.05f, 0.12f);
        main.startColor      = new ParticleSystem.MinMaxGradient(
                                   new Color(1f, 0.9f, 0.3f),
                                   new Color(1f, 0.45f, 0.02f));
        main.maxParticles    = 60;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.gravityModifier = -0.05f;

        var vel = ps.velocityOverLifetime;
        vel.enabled = true;
        vel.space   = ParticleSystemSimulationSpace.Local;
        vel.x = new ParticleSystem.MinMaxCurve(0f, 0f);
        vel.y = new ParticleSystem.MinMaxCurve(0f, 0f);
        vel.z = new ParticleSystem.MinMaxCurve(-3f, -1.5f);

        var em = ps.emission; em.rateOverTime = 40f;

        var sh = ps.shape;
        sh.shapeType = ParticleSystemShapeType.Cone;
        sh.angle     = 12f;
        sh.radius    = 0.03f;

        var col = ps.colorOverLifetime; col.enabled = true;
        var g   = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(new Color(1f, 1f, 0.5f), 0f),
                    new GradientColorKey(new Color(1f, 0.5f, 0.05f), 0.6f),
                    new GradientColorKey(new Color(0.8f, 0.15f, 0f), 1f) },
            new[] { new GradientAlphaKey(1f, 0f),
                    new GradientAlphaKey(0.8f, 0.4f),
                    new GradientAlphaKey(0f, 1f) });
        col.color = new ParticleSystem.MinMaxGradient(g);

        go.GetComponent<ParticleSystemRenderer>().sharedMaterial =
            ParticleMat(new Color(1f, 0.7f, 0.05f));
    }

    void EnergyTrail(GameObject parent)
    {
        var go = new GameObject("Trail");
        go.transform.SetParent(parent.transform, false);
        go.transform.localPosition = Vector3.zero;

        var tr = go.AddComponent<TrailRenderer>();
        tr.time         = 0.35f;
        tr.startWidth   = 0.20f;
        tr.endWidth     = 0f;
        tr.material     = TrailMat();
        tr.generateLightingData = false;
        tr.shadowCastingMode    = UnityEngine.Rendering.ShadowCastingMode.Off;

        var tg = new Gradient();
        tg.SetKeys(
            new[] { new GradientColorKey(coreColor,   0f),
                    new GradientColorKey(accentColor,  0.5f),
                    new GradientColorKey(accentColor,  1f) },
            new[] { new GradientAlphaKey(0.9f, 0f),
                    new GradientAlphaKey(0.4f, 0.5f),
                    new GradientAlphaKey(0f,   1f) });
        tr.colorGradient = tg;
    }

    GameObject Box(GameObject parent, string n, Vector3 pos, Vector3 scale, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = n;
        Destroy(go.GetComponent<Collider>());
        go.transform.SetParent(parent.transform, false);
        go.transform.localPosition = pos;
        go.transform.localScale    = scale;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        return go;
    }

    GameObject Cyl(GameObject parent, string n, Vector3 pos, Vector3 scale, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        go.name = n;
        Destroy(go.GetComponent<Collider>());
        go.transform.SetParent(parent.transform, false);
        go.transform.localPosition = pos;
        go.transform.localScale    = scale;
        go.GetComponent<Renderer>().sharedMaterial = mat;
        return go;
    }

    Material UnlitMat(Color color, float power)
    {
        var mat = new Material(unlitShader);
        mat.SetColor("_BaseColor", color * power);
        return mat;
    }

    Material ArmorMat()
    {
        var mat = new Material(litShader);
        mat.SetColor("_BaseColor",     armorColor);
        mat.SetFloat("_Metallic",      0.95f);
        mat.SetFloat("_Smoothness",    0.85f);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", coreColor * 2.5f);
        mat.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
        return mat;
    }

    Material ParticleMat(Color color)
    {
        var mat = new Material(particleUnlitShader);
        mat.SetColor("_BaseColor", color);
        return mat;
    }

    Material TrailMat()
    {
        var mat = new Material(particleUnlitShader);
        mat.SetColor("_BaseColor", coreColor);
        return mat;
    }

    Transform FindBone(string name)
    {
        foreach (Transform t in GetComponentsInChildren<Transform>(true))
            if (t.name == name) return t;
        return null;
    }

    void ShowBoots()
    {
        if (leftRoot  != null) leftRoot.SetActive(true);
        if (rightRoot != null) rightRoot.SetActive(true);
        active = true;
        pulseT = 0f;
    }

    void HideBoots()
    {
        active = false;
        if (leftRoot  != null) leftRoot.SetActive(false);
        if (rightRoot != null) rightRoot.SetActive(false);
    }
}
