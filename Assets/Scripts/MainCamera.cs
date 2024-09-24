using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public bool primoClick = false;
    public bool multicolor = false;
    public GameObject pallina;
    public Pallina accPallina;
    public LivelloInf accLivelloInf;
    public UIMenu accUIMenu;
    public UIGioco accUIGioco;
    public float velocitàRidimens;
    public float velocitàCamera;
    public bool gameOver = false;
    public float dimensioneIniziale;
    public float dimensioneGioco;
    float redCamera = 0;
    float greenCamera = 0;
    float bluCamera = 0;

    float defRed = 0 ;
    float defGreen = 0;
    float defBlu = 0 ;
    float red = 0;
    float green = 0;
    float blu = 0;
    Camera camera;
    public Color startColor;

    public float timerColore = 0;

    private void Awake()
    {

    }
    void Start()
    {
        camera = gameObject.GetComponent<Camera>();
        CalcoliSchermo();
        gameObject.GetComponent<Camera>().orthographicSize = dimensioneIniziale;
        camera.clearFlags = CameraClearFlags.SolidColor;
        camera.backgroundColor = new Color(PlayerPrefs.GetFloat("Red", startColor.r), PlayerPrefs.GetFloat("Green", startColor.g), PlayerPrefs.GetFloat("Blu", startColor.b));
        GeneraColore();
        StartCoroutine(LerpColore());

      
    }

    // Update is called once per frame
    void Update()
    {
        //if ((Input.touchCount > 0 || Input.GetMouseButtonUp(0)) && primoClick == false)
        //{
        //    //StartCoroutine("RidimensionamentoCamera");
        //    RidimensionamentoCamera();
        //}
        if(timerColore < 10)
        {
            timerColore += Time.deltaTime;
        }
        else
        {
            timerColore = 0;
            GeneraColore();
            StartCoroutine(LerpColore());
        }
    }
    private void LateUpdate()
    {
        if(primoClick && !gameOver)
        {
            SeguiPallina();
        }
        

    }

    public void SeguiPallina() //Lega la camera alla pallina per seguire l' andamento del livello
    {
        //transform.position = new Vector3(0, pallina.transform.position.y + 15, -10);
        transform.Translate(Vector3.up * Time.deltaTime * velocitàCamera * 0.99f);


    }

    public void RidimensionamentoCamera() // animazione camera che stringe la visuale per poi far cominciare il livello
    {
        accLivelloInf.PartiDa();
        accLivelloInf.pallina.SetActive(true);
        accLivelloInf.linea.gameObject.SetActive(true);
        accUIMenu.primoClick = true;

        //accLivelloInf.AttivaOstacoli();

        StartCoroutine(ZoomInizioPartita());
        primoClick = true;
        if (accLivelloInf.simulaStep)
        {
            accLivelloInf.SimulaStep();
        }
        accLivelloInf.GeneraStep();
        accPallina.primoClick = true;
        accLivelloInf.primoClick = true;
        accUIGioco.score.gameObject.SetActive(true);
        accUIGioco.pauseButton.gameObject.SetActive(true);
        StartCoroutine(accUIMenu.FadeOutMenu());
        StartCoroutine(accUIMenu.AumentoVolume());
    }
    public void CalcoliSchermo()
    {
        float width = Screen.width;
        float height = Screen.height;
        dimensioneIniziale = 23 + (10.9f * ((height / width) - 1.833f));
        dimensioneGioco = 20 + (10.9f * ((height / width) - 1.833f));
    }

    public IEnumerator ZoomInizioPartita()
    {
        while (gameObject.GetComponent<Camera>().orthographicSize > dimensioneGioco)
        {
            gameObject.GetComponent<Camera>().orthographicSize -= Time.deltaTime * velocitàRidimens;
            yield return null;
        }
        yield return null;
    }
    public void GeneraColore()
    {
        //int maxColorSomma = 600;

        //int random = Random.Range(1, 4);
        //if(random == 1)
        //{
        //    red = Random.Range(1, 255);
        //    green = Random.Range(1,maxColorSomma - red);
        //    blu = Random.Range(1, maxColorSomma - red - green);
        //}
        //else if(random == 2)
        //{
        //    green = Random.Range(1, 255);
        //    blu = Random.Range(1, maxColorSomma - green);
        //    red = Random.Range(1, maxColorSomma - blu - green);
        //}
        //else
        //{
        //    blu = Random.Range(1, 255);
        //    red = Random.Range(1, maxColorSomma - blu);
        //    green = Random.Range(1, maxColorSomma - red - blu);
        //}

        Color prossimoColore = Random.ColorHSV(0f, 1f, 1f, 1f, 0f, 1f);

        red = prossimoColore.r;
        green = prossimoColore.g;
        blu = prossimoColore.b;

        redCamera = GetComponent<Camera>().backgroundColor.r;
        greenCamera = GetComponent<Camera>().backgroundColor.g;
        bluCamera = GetComponent<Camera>().backgroundColor.b;
        defRed = red - redCamera;
        defGreen = green - greenCamera;
        defBlu = blu - bluCamera;
        //if (multicolor)
        //{
        //    StartCoroutine(LerpColore());
        //}
        //Color prossimoColore = new Color(red, green, blu);
    }
    public IEnumerator LerpColore()
    {
        float i = 0;
        while(i < 10)
        {

            redCamera += defRed * Time.deltaTime / 10;
            greenCamera += defGreen * Time.deltaTime / 10;
            bluCamera += defBlu * Time.deltaTime / 10;
            camera.backgroundColor = new Color(redCamera, greenCamera, bluCamera);
            
            
            i += Time.deltaTime;
            yield return null;
        }
        //GeneraColore();
        yield return null;
        
       



        //if (redCamera < red)
        //{
        //    while (redCamera < red)
        //    {
        //        Mathf.Lerp(redCamera, red, 0.5f);
        //        yield return null;
        //    }

        //}
        //else
        //{
        //    while (redCamera > red)
        //    {
        //        redCamera -= Time.deltaTime * red * velocitaLerp;
        //        yield return null;
        //    }
        //}

    }
}
