using UnityEngine;

/// Genera items de escudo en el recorrido de forma autom\u00e1tica.
/// No necesita prefab ni configuraci\u00f3n: se agrega solo al SegmentGenerator en la escena.
public class ShieldSpawner : MonoBehaviour
{
    [Header("Frecuencia de aparici\u00f3n")]
    [SerializeField] float minSpawnInterval = 120f;
    [SerializeField] float maxSpawnInterval = 220f;
    [SerializeField] float spawnHeight = 1.2f;

    static readonly float[] Lanes = { -4f, 0f, 4f };

    Transform   player;
    float       nextSpawnZ;
    GameObject  activePickup;

    void Start()
    {
        GameObject found = GameObject.FindGameObjectWithTag("Player");
        if (found != null) player = found.transform;

        nextSpawnZ = 30f + Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void Update()
    {
        if (player == null) return;

        if (player.position.z + 80f >= nextSpawnZ)
            SpawnPickup();

        if (activePickup != null && activePickup.activeSelf &&
            activePickup.transform.position.z < player.position.z - 40f)
            activePickup.SetActive(false);
    }

    void SpawnPickup()
    {
        if (activePickup != null) Destroy(activePickup);

        float x = Lanes[Random.Range(0, Lanes.Length)];
        Vector3 pos = new Vector3(x, spawnHeight, nextSpawnZ);

        // Construir el pickup en c\u00f3digo, sin necesidad de prefab
        activePickup = new GameObject("ShieldPickup");
        activePickup.transform.position = pos;
        SphereCollider col = activePickup.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = 1.2f;
        activePickup.AddComponent<ShieldPickup>();

        nextSpawnZ += Random.Range(minSpawnInterval, maxSpawnInterval);
    }
}
