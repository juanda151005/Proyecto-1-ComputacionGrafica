using UnityEngine;

public class CollectCoin : MonoBehaviour
{
    [SerializeField] AudioSource coinSFX;

    void OnTriggerEnter(Collider other)
    {
        if (coinSFX != null)
        {
            coinSFX.Play();
        }
        else if (GameManager.Instance != null)
        {
            GameManager.Instance.PlayCoinSFX();
        }

        MasterInfo.coinCount += 1;
        this.gameObject.SetActive(false);
    }
}
