using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIGioco : MonoBehaviour
{
    public float timerAnimazioni = 0;
    public bool gameOver = false;
    public GameObject endGameTXT;
    public GameObject shopBTN;
    public GameObject restartBTN;
    public GameObject contatoreMoneteTotali;
    public GameObject moneteTotaliImg;
    public GameObject monetePartitaImg;
    public GameObject monetePartita;
    public GameObject velocitaLivello;
    public GameObject checkpoint;
    public GameObject linea;
    public GameObject pallina;
    public GameObject pausePanel;
    public GameObject volumeBTNPause;
    public GameObject restartBTNPause;
    public GameObject homeBTNPause;
    public Text monetePartitaLBL;
    public Text moltiplicatoreVelocitàLBL;
    public Text moneteTotaliLBL;
    public Text score;

    public Pallina accPallina;
    public MainCamera accMainCamera;

    public Image pauseButton;

    public Sprite volumeOn;
    public Sprite volumeOff;
    public AudioSource track;
    public Image volumeBTN;
    public float velocitaPallina;
    public float velocitaCamera;



    void Start()
    {
        checkpoint.transform.position = new Vector3(0, PlayerPrefs.GetInt("PuntoDiPartenza", 0) + 1000, 0);
    }

    // Update is called once per frame
    void Update()
    {
        score.text = (Mathf.FloorToInt(accPallina.transform.position.y) + PlayerPrefs.GetInt("PuntoDiPartenza",0)).ToString();
    }

    public void Restart()
    {
        StartCoroutine(iRestart());
    }

    public void ConteggioMoneteFineLivello()
    {

        float distanzaPercorsa; 
        float.TryParse(score.text,out distanzaPercorsa);
        float monetePartita = 0;
        float moneteGenerali = PlayerPrefs.GetInt("MoneteTotali", 100);

        moneteTotaliLBL.text = PlayerPrefs.GetInt("MoneteTotali", 100).ToString();
        int moneteGuadagnate = Mathf.FloorToInt(distanzaPercorsa);
        PlayerPrefs.SetInt("MoneteTotali", PlayerPrefs.GetInt("MoneteTotali", 100) + Mathf.FloorToInt(distanzaPercorsa));


        //while (monetePartita < distanzaPercorsa)
        //{
        //    monetePartita += Time.deltaTime * distanzaPercorsa / 2;
        //    monetePartitaLBL.text = monetePartita.ToString();
        //}
        
        StartCoroutine(CrescitaMonete(monetePartita, moneteGenerali));


        //while (monetePartita > 0)
        //{
        //    monetePartita -= Time.deltaTime * monetePartita / 3;
        //    monetePartitaImg.transform.Rotate(new Vector3(0, 5, 0));
        //    moneteTotaliImg.transform.Rotate(new Vector3(0, 5, 0));
        //    moneteGenerali += Time.deltaTime * monetePartita / 3;
        //    moneteTotaliLBL.text = moneteGenerali.ToString();
        //}



    }
    public IEnumerator CrescitaMonete(float monetepartita, float monetegenerali) // incrementa le monete guadagnbate a fine livello. secondo while per "trasferirle nel conteggio generale delle monete
    {
        yield return new WaitForSeconds(1);
        //float divisoreVelocita = 3.5f;
        float distanzaPercorsa;
        float.TryParse(score.text, out distanzaPercorsa);

        float velocitaScorrereMonete = distanzaPercorsa / 2.5f;

        monetePartitaImg.GetComponent<Animator>().enabled = false;
        //velocitaLivello.GetComponent<Animator>().SetBool("ScaleLoop", true);
        
        while (monetepartita < distanzaPercorsa)
        {
            if (distanzaPercorsa - monetepartita < 11)
            {
                velocitaScorrereMonete = 8;
            }
            monetepartita += Time.deltaTime * velocitaScorrereMonete;
            monetePartitaImg.transform.Rotate(new Vector3(0, 5, 0));
            monetePartitaLBL.text = Mathf.FloorToInt(monetepartita).ToString();
            
            //if (monetepartita > PlayerPrefs.GetInt("LivelloInCorso", 1) * 1000 * velocitalivello - 20)
            //{
            //    divisoreVelocita = 80;
            //}
            yield return null;
        }

        yield return new WaitForSeconds(0.5f);

        //velocitaLivello.GetComponent<Animator>().SetBool("FadeOut", true);

        yield return new WaitForSeconds(0.5f);

        //float velocitaScorrereMoneteIniziale = velocitaScorrereMonete;

        moneteTotaliImg.GetComponent<Animator>().enabled = false;

        velocitaScorrereMonete = monetepartita / 2.5f;

        while (monetepartita > 0)
        {
            if (monetepartita < 10)
            {
                velocitaScorrereMonete = 8;
            }

            monetepartita -= Time.deltaTime * velocitaScorrereMonete;
            monetegenerali += Time.deltaTime * velocitaScorrereMonete;

            monetePartitaImg.transform.Rotate(new Vector3(0, 6, 0));
            moneteTotaliImg.transform.Rotate(new Vector3(0, 6, 0));


            moneteTotaliLBL.text = Mathf.RoundToInt(monetegenerali).ToString();
            monetePartitaLBL.text = Mathf.RoundToInt(monetepartita).ToString();

            
            yield return null;
        }
        moneteTotaliLBL.text = PlayerPrefs.GetInt("MoneteTotali", 100).ToString();
        moneteTotaliImg.GetComponent<Animator>().enabled = true;
        monetePartitaImg.GetComponent<Animator>().enabled = true;

        yield return new WaitForSeconds(1);

        monetePartita.GetComponent<Animator>().SetBool("FadeOut", true);
        monetePartitaImg.GetComponent<Animator>().SetBool("FadeOut", true);
    }

    public IEnumerator iRestart()
    {
        restartBTN.GetComponent<Animator>().SetBool("FadeOut", true);
        yield return new WaitForSeconds(0.1f);

        shopBTN.GetComponent<Animator>().SetBool("FadeOut", true);
        yield return new WaitForSeconds(0.1f);

        endGameTXT.GetComponent<Animator>().SetBool("FadeOut", true);
        yield return new WaitForSeconds(0.1f);

        moneteTotaliImg.GetComponent<Animator>().SetBool("FadeOut", true);

        yield return new WaitForSeconds(0.1f);
        moneteTotaliLBL.GetComponent<Animator>().SetBool("FadeOut", true);

        yield return new WaitForSeconds(0.5f);
        PlayerPrefX.SetBool("NextLevel", true);
        //gameObject.GetComponent<Gameplay>().livelloInCorso.SetActive(false);
        PlayerPrefs.SetInt("LivelloInCorso", PlayerPrefs.GetInt("LivelloInCorso", 1) + 1);
        SceneManager.LoadScene(0);

        yield return null;

    }
    public void FadeOutPallinaLinea()
    {
        linea.GetComponent<Animator>().SetBool("FadeOut", true);
        pallina.GetComponent<Animator>().SetBool("FadeOut", true);
    }

    public IEnumerator FadeInMenu()
    {
        shopBTN.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        restartBTN.SetActive(true);

        yield return new WaitForSeconds(0.1f);
        contatoreMoneteTotali.SetActive(true);
        moneteTotaliImg.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        monetePartitaImg.SetActive(true);

        yield return new WaitForSeconds(0.05f);
        monetePartita.SetActive(true);

        yield return null;

    }
    
    public void Pause()
    {

        if (PlayerPrefX.GetBool("Volume", true))
        {
            volumeBTN.sprite = volumeOn;
        }
        else
        {
            volumeBTN.sprite = volumeOff;
        }
        if (!pausePanel.activeSelf)
        {
            pauseButton.GetComponent<Button>().interactable = false;

            pausePanel.SetActive(true);
            pauseButton.color = new Color(255,255,255,255);
            velocitaPallina = accPallina.velocitàPallina;
            accPallina.velocitàPallina = 0;
            velocitaCamera = accMainCamera.velocitàCamera;
            accMainCamera.velocitàCamera = 0;
            StartCoroutine(FadeInPauseMenu());

            pauseButton.GetComponent<Button>().interactable = true;

        }
        else
        {
            pauseButton.GetComponent<Button>().interactable = false;

            var color = pauseButton.color;
            color.a = 0.4f;

            pauseButton.color = color;
            accPallina.velocitàPallina = velocitaPallina;
            accMainCamera.velocitàCamera = velocitaCamera;
            StartCoroutine(FadeOutPauseMenu());

            Time.timeScale = 0;
            StartCoroutine(RiattivazioneTempo());
            pauseButton.GetComponent<Button>().interactable = true;

        }

    }
    public IEnumerator FadeInPauseMenu()
    {
        restartBTNPause.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        volumeBTNPause.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        homeBTNPause.SetActive(true);
    }
    public IEnumerator FadeOutPauseMenu()
    {
        restartBTNPause.GetComponent<Animator>().SetBool("FadeOut", true);
        yield return new WaitForSeconds(0.05f);
        volumeBTNPause.GetComponent<Animator>().SetBool("FadeOut", true);
        yield return new WaitForSeconds(0.05f);
        homeBTNPause.GetComponent<Animator>().SetBool("FadeOut", true);
        yield return new WaitForSeconds(1);
        pausePanel.SetActive(false);
        yield return new WaitForSeconds(1);


    }
    public IEnumerator RiattivazioneTempo()
    {
        
        while (Time.timeScale < 1)
        {
            Time.timeScale += Time.fixedDeltaTime / 3;
            yield return null;
        }
        yield return null;
    }
    public void Home()
    {
        PlayerPrefs.SetFloat("Red", accMainCamera.GetComponent<Camera>().backgroundColor.r);
        PlayerPrefs.SetFloat("Green", accMainCamera.GetComponent<Camera>().backgroundColor.g);
        PlayerPrefs.SetFloat("Blu", accMainCamera.GetComponent<Camera>().backgroundColor.b);
        SceneManager.LoadScene(0);
        //PlayerPrefX.SetColor("ColoreSfondo", accMainCamera.GetComponent<Camera>().backgroundColor);
        

    }
    public void VolumePause()
    {
        if (PlayerPrefX.GetBool("Volume", true))
        {
            volumeBTN.sprite = volumeOff;
            track.volume = 0;
            PlayerPrefX.SetBool("Volume", false);

        }
        else
        {
            volumeBTN.sprite = volumeOn;
            track.volume = 1;
            PlayerPrefX.SetBool("Volume", true);
        }
    }
    public void Resume()
    {
        
    }
}
