using UnityEngine;

public class PlayerDoubleJump : MonoBehaviour
{
    [Header("Duración")]
    [SerializeField] float minDuration = 6f;
    [SerializeField] float maxDuration = 12f;

    public bool  IsActive            { get; private set; }
    public float PowerTimeNormalized { get; private set; }

    public event System.Action OnPowerActivated;
    public event System.Action OnPowerExpired;

    float powerTimer;
    float powerDuration;

    void Update()
    {
        if (!IsActive) return;

        powerTimer -= Time.deltaTime;
        PowerTimeNormalized = Mathf.Clamp01(powerTimer / powerDuration);

        if (powerTimer <= 0f)
            Expire();
    }

    public void Activate()
    {
        powerDuration       = Random.Range(minDuration, maxDuration);
        powerTimer          = powerDuration;
        PowerTimeNormalized = 1f;
        IsActive            = true;
        OnPowerActivated?.Invoke();
        Debug.Log($"[PlayerDoubleJump] Doble salto activado por {powerDuration:F1}s.");
    }

    public void Expire()
    {
        IsActive            = false;
        PowerTimeNormalized = 0f;
        OnPowerExpired?.Invoke();

        PowerExpireEffect.Spawn(transform.position + Vector3.up * 0.8f,
                                new Color(1f, 0.45f, 0.05f));

        Debug.Log("[PlayerDoubleJump] Doble salto expirado.");
    }
}
