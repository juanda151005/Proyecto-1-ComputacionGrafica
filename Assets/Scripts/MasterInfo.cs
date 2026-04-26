using UnityEngine;

public class MasterInfo : MonoBehaviour
{
    public static int coinCount = 0;

    [SerializeField] TMPro.TMP_Text coinDisplay;
    [SerializeField] TMPro.TMP_Text distanceDisplay;

    Transform player;
    float     startZ;

    void Start()
    {
        coinCount = 0;
        GameObject found = GameObject.FindGameObjectWithTag("Player");
        if (found != null)
        {
            player = found.transform;
            startZ = player.position.z;
        }
    }

    /// Distancia recorrida en la partida actual (accesible desde GameManager).
    public static int CurrentDistance { get; private set; }

    void Update()
    {
        if (coinDisplay != null)
            coinDisplay.text = "COINS: " + coinCount;

        if (player != null)
        {
            CurrentDistance = Mathf.Max(0, Mathf.FloorToInt(player.position.z - startZ));
            if (distanceDisplay != null)
                distanceDisplay.text = "DIST: " + CurrentDistance + " m";
        }
    }
}
