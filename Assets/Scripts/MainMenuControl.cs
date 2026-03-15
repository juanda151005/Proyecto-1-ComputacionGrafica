using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuControl : MonoBehaviour
{

    [SerializeField] GameObject fadeOut;
    [SerializeField] GameObject bounceText;
    [SerializeField] GameObject bigButton;
    [SerializeField] GameObject animCam;
    [SerializeField] GameObject mainCam;
    [SerializeField] GameObject menuControls;
    [SerializeField] AudioSource buttonSelect;
    public static bool hasClicked = false;
    [SerializeField] GameObject staticCam;
    [SerializeField] GameObject fadeIn;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(FadeInTurnOff());
        if(hasClicked == true)
        {
            staticCam.SetActive(true);
            mainCam.SetActive(false);
            menuControls.SetActive(true);
            bounceText.SetActive(false);
            bigButton.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MenuBeginButton()
    {
        StartCoroutine(AnimCam());
    
    }


    public void StartGame()
    {
        StartCoroutine(StartButton());
    }

    IEnumerator StartButton()
    {
        buttonSelect.Play();
        menuControls.SetActive(false); // Ocultar el botón inmediatamente
        fadeOut.SetActive(true);
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene("Run");
    }

    IEnumerator AnimCam()
    {
        animCam.GetComponent<Animator>().Play("AnimMenuCam");
        bounceText.SetActive(false);
        bigButton.SetActive(false);
        yield return new WaitForSeconds(1.5f);
        fadeIn.SetActive(false);
        mainCam.SetActive(true);
        animCam.SetActive(false);
        menuControls.SetActive(true);
        hasClicked = true;
    }

    IEnumerator FadeInTurnOff(){
        yield return new WaitForSeconds(1);
        fadeIn.SetActive(false);
    }
}
