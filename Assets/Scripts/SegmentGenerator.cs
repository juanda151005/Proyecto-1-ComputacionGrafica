using System.Collections.Generic;
using UnityEngine;

public class SegmentGenerator : MonoBehaviour
{
    public enum Difficulty { Easy, Medium, Hard }

    [System.Serializable]
    public class SegmentEntry
    {
        public GameObject prefab;
        public Difficulty difficulty = Difficulty.Easy;
        [Tooltip("Peso base. A mayor peso, mayor probabilidad de salir.")]
        public float weight = 1f;
    }

    [Header("Segmentos")]
    public SegmentEntry[] segments;

    [Header("Referencia al jugador")]
    [SerializeField] Transform player;

    [Header("Configuración de spawn")]
    [Tooltip("Largo en Z de cada segmento.")]
    [SerializeField] float segmentLength = 50f;
    [Tooltip("Cuántos segmentos debe haber siempre por delante del jugador (buffer visual).")]
    [SerializeField] int segmentsAhead = 5;
    [Tooltip("Z donde aparece el primer segmento.")]
    [SerializeField] float startZ = 50f;
    [Tooltip("Distancia detrás del jugador a la que se destruyen los segmentos viejos.")]
    [SerializeField] float recycleDistanceBehind = 60f;

    [Header("Dificultad progresiva")]
    [Tooltip("A partir de esta distancia (en Z) empieza a aparecer dificultad Medium.")]
    [SerializeField] float mediumUnlockDistance = 300f;
    [Tooltip("A partir de esta distancia empieza a aparecer dificultad Hard.")]
    [SerializeField] float hardUnlockDistance = 700f;
    [Tooltip("Qué tan rápido crece el peso relativo de los segmentos difíciles con la distancia.")]
    [SerializeField] float difficultyRampRate = 0.0015f;

    float nextSpawnZ;
    readonly List<GameObject> activeSegments = new List<GameObject>();

    void Start()
    {
        if (player == null)
        {
            GameObject found = GameObject.FindGameObjectWithTag("Player");
            if (found != null) player = found.transform;
        }

        nextSpawnZ = startZ;

        for (int i = 0; i < segmentsAhead; i++)
        {
            SpawnNext();
        }
    }

    void Update()
    {
        if (player == null || segments == null || segments.Length == 0) return;

        float frontierZ = nextSpawnZ - segmentsAhead * segmentLength;
        while (nextSpawnZ < player.position.z + segmentsAhead * segmentLength)
        {
            SpawnNext();
        }

        CleanupBehind();
    }

    void SpawnNext()
    {
        SegmentEntry chosen = PickWeightedSegment();
        if (chosen == null || chosen.prefab == null) return;

        GameObject seg = Instantiate(chosen.prefab, new Vector3(0, 0, nextSpawnZ), Quaternion.identity);
        activeSegments.Add(seg);
        nextSpawnZ += segmentLength;
    }

    SegmentEntry PickWeightedSegment()
    {
        float playerZ = player != null ? player.position.z : 0f;

        bool mediumUnlocked = playerZ >= mediumUnlockDistance;
        bool hardUnlocked = playerZ >= hardUnlockDistance;

        float mediumBias = mediumUnlocked ? (playerZ - mediumUnlockDistance) * difficultyRampRate : 0f;
        float hardBias = hardUnlocked ? (playerZ - hardUnlockDistance) * difficultyRampRate : 0f;

        float total = 0f;
        for (int i = 0; i < segments.Length; i++)
        {
            total += GetEffectiveWeight(segments[i], mediumUnlocked, hardUnlocked, mediumBias, hardBias);
        }

        if (total <= 0f) return segments[0];

        float roll = Random.value * total;
        float acc = 0f;
        for (int i = 0; i < segments.Length; i++)
        {
            float w = GetEffectiveWeight(segments[i], mediumUnlocked, hardUnlocked, mediumBias, hardBias);
            acc += w;
            if (roll <= acc) return segments[i];
        }
        return segments[segments.Length - 1];
    }

    float GetEffectiveWeight(SegmentEntry entry, bool mediumUnlocked, bool hardUnlocked, float mediumBias, float hardBias)
    {
        if (entry == null || entry.prefab == null) return 0f;

        switch (entry.difficulty)
        {
            case Difficulty.Easy:
                return entry.weight;
            case Difficulty.Medium:
                return mediumUnlocked ? entry.weight * (1f + mediumBias) : 0f;
            case Difficulty.Hard:
                return hardUnlocked ? entry.weight * (1f + hardBias) : 0f;
            default:
                return entry.weight;
        }
    }

    void CleanupBehind()
    {
        float cutoffZ = player.position.z - recycleDistanceBehind;
        for (int i = activeSegments.Count - 1; i >= 0; i--)
        {
            GameObject seg = activeSegments[i];
            if (seg == null)
            {
                activeSegments.RemoveAt(i);
                continue;
            }
            if (seg.transform.position.z + segmentLength < cutoffZ)
            {
                Destroy(seg);
                activeSegments.RemoveAt(i);
            }
        }
    }
}
