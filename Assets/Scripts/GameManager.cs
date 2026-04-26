using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Audio Global")]
    [SerializeField] AudioSource collisionSFX;
    [SerializeField] AudioSource coinSFX;

    [Header("Referencias de escena")]
    [SerializeField] Animator mainCamAnim;
    [SerializeField] GameObject fadeOut;

    [Header("Config")]
    [SerializeField] string menuSceneName = "MainMenu";
    [SerializeField] float stumbleDelay = 1f;
    [SerializeField] float fadeDelay = 3f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void PlayCoinSFX()
    {
        if (coinSFX != null) coinSFX.Play();
    }

    public void StartDeathSequence(PlayerMovement player, Animator playerAnim)
    {
        StartCoroutine(DeathSequence(player, playerAnim));
    }

    IEnumerator DeathSequence(PlayerMovement player, Animator playerAnim)
    {
        if (collisionSFX != null) collisionSFX.Play();

        // Detener al jugador en seco para que no atraviese el obstáculo,
        // pero mezclar la animación de tropiezo con un crossfade para que
        // la transición no se vea cortada.
        if (player != null) player.enabled = false;
        if (playerAnim != null) playerAnim.CrossFadeInFixedTime("Stumble Backwards", 0.01f);
        if (mainCamAnim != null) mainCamAnim.Play("CollisionCam");

        yield return new WaitForSeconds(stumbleDelay);

        // Mostrar distancia recorrida y mejor distancia
        if (GameOverScreen.Instance != null)
            GameOverScreen.Instance.Show(MasterInfo.CurrentDistance);

        if (fadeOut != null) fadeOut.SetActive(true);

        yield return new WaitForSeconds(fadeDelay);

        SceneManager.LoadScene(menuSceneName);
    }
}
