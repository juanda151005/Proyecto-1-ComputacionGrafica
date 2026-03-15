using UnityEngine;

public class CollectCoin: MonoBehaviour
{
    [SerializeField]  AudioSource coinSFX;
    
    void OnTriggerEnter(Collider other)
    {
        coinSFX.Play();
        MasterInfo.coinCount += 1;
        this.gameObject.SetActive(false);
    }
}
