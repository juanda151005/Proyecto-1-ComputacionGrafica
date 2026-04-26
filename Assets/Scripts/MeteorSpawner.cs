using UnityEngine;

/// Spawnea meteoros que caen del cielo.
/// Predice dónde estará el jugador cuando el meteoro aterrice
/// usando su velocidad actual, para que siempre caigan delante.
public class MeteorSpawner : MonoBehaviour
{
    [SerializeField] float intervalMin  = 1.2f;
    [SerializeField] float intervalMax  = 2.4f;
    [SerializeField] float xRange       = 3.5f;
    [SerializeField] float spawnHeight  = 30f;
    [SerializeField] float fallSpeed    = 14f;   // velocidad de caida fija para predecir bien
    [SerializeField] float groundY      = 0.5f;
    [SerializeField] float leadOffset   = 2f;    // metros extra adelante del punto predicho

    float          timer;
    Transform      player;
    PlayerMovement playerMov;

    void Start()
    {
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
        // Tiempo que tardará el meteoro en caer
        float fallTime = spawnHeight / fallSpeed;

        // Velocidad actual del jugador (fallback 12 si no se encuentra)
        float pSpeed = playerMov != null ? playerMov.playerSpeed : 12f;

        // Z predicho = donde estará el jugador cuando el meteoro aterrice
        float x = Random.Range(-xRange, xRange);
        float z = player.position.z + pSpeed * fallTime + leadOffset;

        GameObject go = new GameObject("Meteor");
        go.transform.position = new Vector3(x, groundY + spawnHeight, z);

        Meteor m    = go.AddComponent<Meteor>();
        m.fallSpeed = fallSpeed + Random.Range(-2f, 2f);  // ligera variación visual
        m.groundY   = groundY;
    }
}
