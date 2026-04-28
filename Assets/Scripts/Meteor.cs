using UnityEngine;

public class Meteor : MonoBehaviour
{
    [HideInInspector] public float fallSpeed = 13f;
    [HideInInspector] public float groundY   = 0.5f;

    GameObject   shadowObj;
    Material     shadowMat;
    TrailRenderer trail;
    float        startY;
    bool         impacted;

    internal static Shader s_litShader;
    internal static Shader s_particleUnlitShader;

    static readonly Color colFar  = new Color(1f, 0.88f, 0f,  0.50f);
    static readonly Color colNear = new Color(1f, 0.08f, 0f,  0.88f);

    void Start()
    {
        startY = transform.position.y;

        gameObject.tag = "Obstacle";

        SphereCollider sc = gameObject.AddComponent<SphereCollider>();
        sc.isTrigger = true;
        sc.radius    = 0.8f;

        BuildVisual();
        BuildTrail();
        BuildShadow();
    }

    void BuildVisual()
    {
        GameObject vis = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        vis.name = "MeteorVis";
        vis.transform.SetParent(transform, false);
        vis.transform.localScale = Vector3.one * 1.5f;

        Destroy(vis.GetComponent<Collider>());

        Material mat = new Material(s_litShader);
        mat.color = new Color(0.10f, 0.03f, 0.01f);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", new Color(1f, 0.38f, 0.02f) * 2.5f);
        vis.GetComponent<MeshRenderer>().material = mat;
    }

    void BuildTrail()
    {
        trail             = gameObject.AddComponent<TrailRenderer>();
        trail.time        = 0.35f;
        trail.startWidth  = 1.0f;
        trail.endWidth    = 0.0f;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        Material tMat = new Material(s_particleUnlitShader);
        trail.material = tMat;

        Gradient g = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(new Color(1f, 0.65f, 0.10f), 0f),
                    new GradientColorKey(new Color(0.25f, 0.06f, 0f),  1f) },
            new[] { new GradientAlphaKey(0.9f, 0f),
                    new GradientAlphaKey(0.0f, 1f) });
        trail.colorGradient = g;
    }

    void BuildShadow()
    {
        shadowObj = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        shadowObj.name = "MeteorShadow";
        shadowObj.transform.position   = new Vector3(transform.position.x, groundY + 0.04f, transform.position.z);
        shadowObj.transform.localScale = new Vector3(5f, 0.01f, 5f);
        Destroy(shadowObj.GetComponent<Collider>());

        shadowMat = new Material(s_litShader);
        shadowMat.SetFloat("_Surface",   1f);
        shadowMat.SetFloat("_Blend",     0f);
        shadowMat.SetFloat("_SrcBlend",  5f);
        shadowMat.SetFloat("_DstBlend", 10f);
        shadowMat.SetFloat("_ZWrite",    0f);
        shadowMat.SetFloat("_AlphaClip", 0f);
        shadowMat.renderQueue = 3000;
        shadowMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        shadowMat.color = colFar;

        var mr = shadowObj.GetComponent<MeshRenderer>();
        mr.material = shadowMat;
        mr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows    = false;
    }

    void Update()
    {
        if (impacted) return;

        transform.position += Vector3.down * fallSpeed * Time.deltaTime;
        transform.Rotate(55f * Time.deltaTime, 35f * Time.deltaTime, 20f * Time.deltaTime, Space.World);

        float t = Mathf.Clamp01((transform.position.y - groundY) / (startY - groundY));

        float s = Mathf.Lerp(1.0f, 5.5f, t);
        float pulse = 1f + Mathf.Sin(Time.time * Mathf.Lerp(12f, 2f, t)) * 0.06f * (1f - t);
        shadowObj.transform.localScale = new Vector3(s * pulse, 0.01f, s * pulse);

        shadowMat.color = Color.Lerp(colNear, colFar, t);

        if (transform.position.y <= groundY + 0.35f)
            Impact();
    }

    void Impact()
    {
        impacted = true;

        if (shadowObj != null) Destroy(shadowObj);
        if (trail     != null) trail.Clear();

        SpawnBurst(transform.position,               new Color(1f, 0.42f, 0.04f), 90, 0.9f, 6f);
        SpawnBurst(transform.position + Vector3.up,  new Color(1f, 0.90f, 0.60f), 35, 0.5f, 4f);
        SpawnBurst(transform.position + Vector3.up * 0.5f, new Color(0.3f, 0.3f, 0.3f), 25, 1.2f, 2f);

        Destroy(gameObject, 0.05f);
    }

    static void SpawnBurst(Vector3 pos, Color color, int count, float duration, float speed)
    {
        GameObject go = new GameObject("MeteorBurst");
        go.transform.position = pos;

        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main              = ps.main;
        main.duration         = duration;
        main.loop             = false;
        main.startLifetime    = new ParticleSystem.MinMaxCurve(0.5f, 1.3f);
        main.startSpeed       = new ParticleSystem.MinMaxCurve(speed * 0.4f, speed);
        main.startSize        = new ParticleSystem.MinMaxCurve(0.15f, 0.55f);
        main.startColor       = new ParticleSystem.MinMaxGradient(color, Color.Lerp(color, Color.white, 0.3f));
        main.gravityModifier  = 0.35f;
        main.simulationSpace  = ParticleSystemSimulationSpace.World;
        main.maxParticles     = count + 20;

        var emission          = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, count) });

        var shape             = ps.shape;
        shape.enabled         = true;
        shape.shapeType       = ParticleSystemShapeType.Sphere;
        shape.radius          = 0.35f;

        var col               = ps.colorOverLifetime;
        col.enabled           = true;
        Gradient g            = new Gradient();
        g.SetKeys(
            new[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
            new[] { new GradientAlphaKey(1f,    0f), new GradientAlphaKey(0f,    1f) });
        col.color = new ParticleSystem.MinMaxGradient(g);

        if (s_particleUnlitShader != null)
        {
            var mat = new Material(s_particleUnlitShader);
            mat.SetColor("_BaseColor", color);
            go.GetComponent<ParticleSystemRenderer>().material = mat;
        }

        ps.Play();

        Destroy(go, main.duration + main.startLifetime.constantMax + 0.3f);
    }
}
