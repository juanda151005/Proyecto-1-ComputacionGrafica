using UnityEngine;

public class MeteorSpawner : MonoBehaviour
{
    [Header("Shaders")]
    [SerializeField] Shader litShader;
    [SerializeField] Shader particleUnlitShader;

    [SerializeField] float intervalMin  = 1.2f;
    [SerializeField] float intervalMax  = 2.4f;
    [SerializeField] float xRange       = 3.5f;
    [SerializeField] float spawnHeight  = 30f;
    [SerializeField] float fallSpeed    = 14f;
    [SerializeField] float groundY      = 0.5f;
    [SerializeField] float leadOffset   = 2f;

    float          timer;
    Transform      player;
    PlayerMovement playerMov;

    void Start()
    {
        Meteor.s_litShader           = litShader;
        Meteor.s_particleUnlitShader = particleUnlitShader;

        timer = 1.5f;

        var go = GameObject.FindWithTag("Player");
        if (go == null)
        {
            playerMov = FindFirstObjectByType<PlayerMovement>();
            if (playerMov != null) go = playerMov.gameObject;
        }
        else
        {
            playerMov = go.GetComponent<PlayerMovement>();
        }

        if (go != null) player = go.transform;
        else Debug.LogWarning("[MeteorSpawner] No se encontro el jugador.");
    }

    void Update()
    {
        if (player == null) return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Spawn();
            timer = Random.Range(intervalMin, intervalMax);
        }
    }

    void Spawn()
    {
        float fallTime = spawnHeight / fallSpeed;
        float pSpeed = playerMov != null ? playerMov.playerSpeed : 12f;
        float x = Random.Range(-xRange, xRange);
        float z = player.position.z + pSpeed * fallTime + leadOffset;

        GameObject go = new GameObject("Meteor");
        go.transform.position = new Vector3(x, groundY + spawnHeight, z);

        Meteor m    = go.AddComponent<Meteor>();
        m.fallSpeed = fallSpeed + Random.Range(-2f, 2f);
        m.groundY   = groundY;
    }
}
