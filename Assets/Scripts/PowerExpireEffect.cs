using UnityEngine;

/// Genera un puff de partículas cuando un poder expira.
/// Llama PowerExpireEffect.Spawn(position, color) desde cualquier script.
public class PowerExpireEffect : MonoBehaviour
{
    /// Crea un burst de humo/partículas en la posición dada con el color indicado.
    /// Se destruye solo al terminar.
    public static void Spawn(Vector3 position, Color color)
    {
        GameObject go = new GameObject("PowerPuff");
        go.transform.position = position;

        ParticleSystem ps = go.AddComponent<ParticleSystem>();
        // Detener inmediatamente para poder configurar sin warnings
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        // ── Módulo principal ──
        var main = ps.main;
        main.duration        = 0.6f;
        main.loop            = false;
        main.startLifetime   = new ParticleSystem.MinMaxCurve(0.4f, 0.9f);
        main.startSpeed      = new ParticleSystem.MinMaxCurve(1.5f, 4.0f);
        main.startSize       = new ParticleSystem.MinMaxCurve(0.15f, 0.45f);
        main.startColor      = new ParticleSystem.MinMaxGradient(
                                   color, Color.Lerp(color, Color.white, 0.4f));
        main.gravityModifier = -0.25f;   // partículas suben levemente
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles    = 60;

        // ── Burst instantáneo ──
        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 40) });

        // ── Forma esférica (explosión omnidireccional) ──
        var shape = ps.shape;
        shape.enabled      = true;
        shape.shapeType    = ParticleSystemShapeType.Sphere;
        shape.radius       = 0.4f;

        // ── Las partículas se agrandan y luego se desvanecen ──
        var sizeOverLife = ps.sizeOverLifetime;
        sizeOverLife.enabled = true;
        AnimationCurve sizeCurve = new AnimationCurve();
        sizeCurve.AddKey(0f,   0.2f);
        sizeCurve.AddKey(0.3f, 1.0f);
        sizeCurve.AddKey(1f,   0.0f);
        sizeOverLife.size = new ParticleSystem.MinMaxCurve(1f, sizeCurve);

        // ── Alpha fade out ──
        var colorOverLife = ps.colorOverLifetime;
        colorOverLife.enabled = true;
        Gradient grad = new Gradient();
        grad.SetKeys(
            new[] { new GradientColorKey(color, 0f), new GradientColorKey(color, 1f) },
            new[] { new GradientAlphaKey(1f, 0f),   new GradientAlphaKey(0f, 1f) });
        colorOverLife.color = new ParticleSystem.MinMaxGradient(grad);

        // ── Renderer: usar el shader particles/standard ──
        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        Shader particleShader = Shader.Find("Particles/Standard Unlit");
        if (particleShader == null) particleShader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (particleShader != null)
        {
            Material mat = new Material(particleShader);
            mat.SetColor("_BaseColor", color);
            renderer.material = mat;
        }

        ps.Play();

        // Auto-destruir cuando terminen las partículas
        Destroy(go, main.duration + main.startLifetime.constantMax + 0.2f);
    }
}
