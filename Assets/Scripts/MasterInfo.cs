using UnityEngine;

public class MasterInfo : MonoBehaviour
{

    public static int coinCount = 0;
    [SerializeField] GameObject coinDisplay;

    // Update is called once per frame
    void Update()
    {
        coinDisplay.GetComponent<TMPro.TMP_Text>().text = "COINS: " + coinCount;
    }
}
