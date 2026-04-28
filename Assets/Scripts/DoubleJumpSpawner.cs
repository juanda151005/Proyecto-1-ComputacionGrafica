using UnityEngine;

public class DoubleJumpSpawner : MonoBehaviour
{
    [Header("Shaders")]
    [SerializeField] Shader litShader;

    [Header("Frecuencia de aparición")]
    [SerializeField] float minSpawnInterval = 200f;
    [SerializeField] float maxSpawnInterval = 600f;
    [SerializeField] float spawnHeight      = 1.2f;

    static readonly float[] Lanes = { -4f, 0f, 4f };

    Transform  player;
    float      nextSpawnZ;
    GameObject activePickup;

    void Start()
    {
        DoubleJumpPickup.s_litShader = litShader;

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

        activePickup = new GameObject("DoubleJumpPickup");
        activePickup.transform.position = new Vector3(x, spawnHeight, nextSpawnZ);
        SphereCollider col = activePickup.AddComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius    = 1.2f;
        activePickup.AddComponent<DoubleJumpPickup>();

        nextSpawnZ += Random.Range(minSpawnInterval, maxSpawnInterval);
    }
}
