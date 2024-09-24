using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine;

public class Pallina : MonoBehaviour
{
    public MainCamera accMainCamera;
    public UIGioco accUIGioco;
    public LineRenderer Linea;
    public LivelloInf accLivelloInf;

    public int targetIndex;
    public float velocitàPallina;
    public int distanzaPercorsa;
    public float timer = 1;

    public bool primoClick = false;
    public bool gameOver = false;
    public bool newRecord = false;

    public GameObject checkpoint;
    public GameObject[] fireworks;

    public AudioSource track;
    //public GameObject fineLivelloPanel;
    //public Gameplay accGameplay;
    // public Vector3[] targetLinea;


    void Start()
    {


        // targetIndex = 0;

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (primoClick && !gameOver)
        {
            SeguiLinea();

            
        }

    }
    private void OnCollisionEnter(Collision collision)
    {
        
        if (transform.position.y > PlayerPrefs.GetInt("Record", 0))
        {
            PlayerPrefs.SetInt("Record", Mathf.FloorToInt(transform.position.y));
            accUIGioco.endGameTXT.SetActive(true);
        }

        PosizioneFireworks();
        fireworks[0].SetActive(true);
        fireworks[1].SetActive(true);
        fireworks[2].SetActive(true);
        accUIGioco.gameOver = true;
        gameOver = true;
        accMainCamera.gameOver = true;
        //fineLivelloPanel.SetActive(true);
        accUIGioco.ConteggioMoneteFineLivello();
        distanzaPercorsa = Mathf.RoundToInt(gameObject.transform.position.y);
        foreach (GameObject ostacolo in accLivelloInf.tuttiOstacoli)
        {
            Destroy(ostacolo.gameObject);
        }
        accLivelloInf.altezzeSchermo.Clear();
        accLivelloInf.nodiLinea.Clear();
        accLivelloInf.tuttiOstacoli.Clear();
        accLivelloInf.gameover = true;
        accUIGioco.FadeOutPallinaLinea();
        Wait(5);
        accUIGioco.score.alignment = TextAnchor.MiddleCenter;
        accUIGioco.score.GetComponent<Animator>().SetBool("GameOver", true);
        StartCoroutine(accUIGioco.FadeInMenu());
        accUIGioco.pauseButton.gameObject.SetActive(false);

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "CheckpointSpawn")
        {
            accLivelloInf.GeneraStep();

            //if (other.transform.position.y != 10)
            //{
            //    checkpoint = Instantiate(checkpoint, new Vector3(0, checkpoint.transform.position.y + 30, 0), Quaternion.identity);
            //}
            //else

            checkpoint.transform.position += new Vector3(0, 40, 0);

            

           // Destroy(other.gameObject);

        }
        if (other.tag == "Checkpoint")
        {
            checkpoint = Instantiate(checkpoint, new Vector3(0, checkpoint.transform.position.y + 200, 0), Quaternion.identity);
            PlayerPrefs.SetInt("PuntoDiPartenza", PlayerPrefs.GetInt("PuntoDiPartenza", 0) + 200);
            Destroy(other.gameObject);

        }
    }
    public void SeguiLinea() // movimento palla che segue la traiettoria della linea raggiungendo un dopo l altro i target della linea
    {
        
        if (transform.position.y >= accLivelloInf.nodiLinea[targetIndex].y)
        {
            if (targetIndex + 1 < Linea.positionCount)
            {
                targetIndex++;
                CalcoloAngolo();
            }
        }
        else
        {
            transform.Translate(Vector3.up * Time.deltaTime * velocitàPallina); // il * 1.01 è perche se no piano piano la pallina va sempre piu in basso
        }
    }

    public void CalcoloAngolo() // calcola l' angolazione che deve assumere la pallina ad ogni target della linea. Assegna la velocità alla telecamera
    {
        float catetoBase = Mathf.Abs(transform.position.x - accLivelloInf.nodiLinea[targetIndex].x);
        float catetoDue = Mathf.Abs(transform.position.y - accLivelloInf.nodiLinea[targetIndex].y);
        float ipotenusa = Mathf.Sqrt(catetoDue * catetoDue + catetoBase * catetoBase);
        float cosAngolo = catetoDue / ipotenusa;
        float angolo = Mathf.Acos(cosAngolo) * Mathf.Rad2Deg;
        if (transform.position.x < accLivelloInf.nodiLinea[targetIndex].x)
        {
            angolo = -angolo;
        }
        transform.rotation = Quaternion.Euler(0, 0, angolo);

        accMainCamera.velocitàCamera = cosAngolo * velocitàPallina;
    }

    //public void DatiLinea() // aquisisce i dati sulla linea
    //{
    //    accLivelloInf.nodiLinea = new Vector3[Linea.positionCount];
    //    Linea.GetPositions(accLivelloInf.nodiLinea);

    //}

    public void PosizioneInizialePallina()
    {
        transform.position = accLivelloInf.nodiLinea[0];
    }

    public void PosizioneFireworks()
    {
        fireworks[0].transform.position = new Vector3(-5, transform.position.y + 15, 0);
        fireworks[1].transform.position = new Vector3(5, transform.position.y + 15, 0);
        fireworks[2].transform.position = new Vector3(0, transform.position.y + 40, 0);
    }
    public IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
    }
}
