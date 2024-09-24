using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using UnityEngine;

public class LivelloInf : MonoBehaviour
{
    public Camera cam;
    public Pallina accPallina;

    public LineRenderer linea;


    public Vector3 ultimoOstacolo;

    public int ostacoloInMovimento;
    public float stepInCorso = 1;
    //public float intervalloSpawn = 0;

    public GameObject ostacolo;
    public GameObject pallina;
    public GameObject camera;
    public GameObject tutorialScene;
    public GameObject pathScene;

    public bool premuto = false;
    public bool primoClick;
    public bool gameover = false;
    //public bool doppioOstacolo = false;
    public bool partiDa = false;
    public bool simulaStep;
    public int ultimoStep;
    public List<Vector3> nodiLinea;
    public List<Vector3> altezzeSchermo;
    // public List<Vector3> spawnOstacoli;
    public List<float> diffYNodi;
    public List<float> diffXNodi;
    public List<GameObject> tuttiOstacoli;
    public Text placeHolder;
    delegate void generaStep();
    delegate void aumentaDifficolta();

    List<generaStep> listaGenerazioni = new List<generaStep>();
    List<aumentaDifficolta> listaAumentiDifficolta = new List<aumentaDifficolta>();

    public float minOstacoloX;
    public float maxOstacoloX;
    public float minOstacoloDoppioX;
    public float maxOstacoloDoppioX;
    public float yMinOstacoli;
    public float yMaxOstacoli;
    public float yMinLinea;
    public float yMaxLinea;
    public float distanzaXNodi;
    public int probabilitaOstacoloSuLinea;
    public float minDistanzaOstacoloSuLinea;
    public float maxDistanzaOstacoloSuLinea;
    public int probabilitaDoppioOstacolo;

    public float durataSparizione;
    public float tempoTraSparizioni;
    //public float minDistanzaXOstacoloDaLinea;
    //public float maxDistanzaXOstacoloDaLinea;

    public float distanzaXOstacoloDaLinea;
    public int primoNodoDopoOstacolo;
    float timerGapTouch = 0.3f;
    float timer = 0;
    public Color lineaTrasperente;
    public Material trasparente;
    public Material lineaNormale;


    AsyncOperation async;

    private void Awake()
    {
        CompilaListaAumentiDifficolta();
    }
    private void Start()
    {
        //if (PlayerPrefX.GetBool("Creazione", true))
        //{
        //    StartCoroutine(LoadingScreen());
        //    PlayerPrefX.SetBool("Creazione", false);
        //}
        //else
        //{
        //    PlayerPrefX.SetBool("Creazione", true);
        //}

        if (PlayerPrefX.GetBool("PrimoAvvio", true))
        {
            PlayerPrefX.SetBool("PrimoAvvio", false);
            pathScene.SetActive(false);

            tutorialScene.SetActive(true);
        }
        CompilaListaGenerazioni();
        //minOstacoloX = 4;
        //maxOstacoloX = 8;
        //yMinOstacoli = 15;
        //yMaxOstacoli = 25;
        //yMinLinea = 15;
        //yMaxLinea = 25;
        //distanzaXNodi = 1;




        //Step1();
        foreach (Transform ostacolo in gameObject.transform) // acquisisce le altezze iniziali degli ostacoli del livello e gli ostacoli stessi
        {
            if (ostacolo.gameObject.tag == "Ostacolo")
            {
                altezzeSchermo.Add(cam.WorldToScreenPoint(ostacolo.localPosition));
            }
        }

        bool arrivatoLimite = false;

        while (!arrivatoLimite)
        {
            if ((stepInCorso) * 300 - nodiLinea[nodiLinea.Count - 1].y > yMaxLinea)
            {
                nodiLinea.Add(GeneraVector3(-9f, +9f, nodiLinea[nodiLinea.Count - 1].y + yMinLinea, nodiLinea[nodiLinea.Count - 1].y + yMaxLinea));
                diffYNodi.Add(Mathf.Abs(nodiLinea[nodiLinea.Count - 1].y - nodiLinea[nodiLinea.Count - 2].y));
                diffXNodi.Add(Mathf.Abs(nodiLinea[nodiLinea.Count - 1].x - nodiLinea[nodiLinea.Count - 2].x));
            }
            else
            {
                arrivatoLimite = true;
            }

        }


        linea.positionCount = nodiLinea.Count;
        linea.SetPositions(nodiLinea.ToArray());
    }
    private void FixedUpdate()
    {



    }
    private void Update()
    {

        if (primoClick && !gameover)
        {
            if (pallina.transform.position.y > tuttiOstacoli[tuttiOstacoli.Count - 1].transform.position.y)
            {
                DestroyImmediate(tuttiOstacoli[tuttiOstacoli.Count - 1]);
                tuttiOstacoli.RemoveAt(tuttiOstacoli.Count - 1);
                altezzeSchermo.RemoveAt(altezzeSchermo.Count - 1);
            }
            //if(pallina.transform.position.y > nodiLinea[nodiLinea.Count - 2].y)
            //{
            //    nodiLinea.RemoveAt(nodiLinea.Count - 2);
            //}
        }
        AggiornaAltezze();
        if (Input.GetMouseButton(0))
        {
            MovimentoOstacoliMouse();
            timerGapTouch = 0;
        }
        else
        {
            if (timerGapTouch > 0)
            {
                timerGapTouch -= Time.deltaTime;
            }
            else
            {
                premuto = false;

            }
        }
        if (Input.touchCount > 0 && (Input.GetTouch(0).phase == TouchPhase.Began || Input.GetTouch(0).phase == TouchPhase.Stationary || Input.GetTouch(0).phase == TouchPhase.Moved))
        {

            MovimentoOstacoliTouch();
        }
        else
        {
            //premuto = false;
        }
        //if (Input.GetMouseButtonUp(0))
        //{
        //    print("UPUP");
        //}
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            timer = tempoTraSparizioni + durataSparizione;

            StartCoroutine(SparizioneLinea());
        }
    }
    public void GeneraOstacoli()
    {
        linea.GetPositions(nodiLinea.ToArray());
    }
    public Vector3 GeneraVector3(float xMin, float xMax, float yMin, float yMax)
    {
        float x = Random.Range(xMin, xMax);
        float y = Random.Range(yMin, yMax);
        return new Vector3(x, y, 0);
    }
    public float GeneraFloat(float min, float max)
    {
        float randomFloat = Random.Range(min, max);
        return randomFloat;
    }
    public int GeneraInt(int min, int max)
    {
        int randomInt = Random.Range(min, max);
        return randomInt;
    }
    public void AggiornaAltezze() // aggiorna le altezze degli ostacoli sullo schermo per poterli cliccare
    {
        int i;
        for (i = 0; i < altezzeSchermo.Count; i++)
        {
            altezzeSchermo[i] = cam.WorldToScreenPoint(tuttiOstacoli[i].transform.position);

        }
    }
    public void MovimentoOstacoliMouse() // movimento ostacoli col mouse : tieni premuto a dx o a sx all altezza dell' ostacolo per spostarlo
    {
        Vector3 mousePosition = Input.mousePosition;
        float screenHeight = Screen.height;
        float screenWidth = Screen.width;
        int i;
        if (premuto == true)
        {
            //print("premuto");
            //i = ostacoloInMovimento;
            //float distanzaOstacoloClick = mousePosition.x - altezzeSchermo[ostacoloInMovimento].x;
            //if (distanzaOstacoloClick > 12 || distanzaOstacoloClick < -12)
            //{
            if (mousePosition.x > screenWidth / 2)
            {
                tuttiOstacoli[ostacoloInMovimento].transform.localPosition += new Vector3(Time.deltaTime * 15, 0, 0);
                //ostacoloInMovimento = i;
            }
            else if (mousePosition.x < screenWidth / 2)
            {
                tuttiOstacoli[ostacoloInMovimento].transform.localPosition -= new Vector3(Time.deltaTime * 15, 0, 0);
                //ostacoloInMovimento = i;
            }

        }
        else
        {
            i = 0;
            for (i = altezzeSchermo.Count - 1; i > -1; i--)
            {
                if (mousePosition.y > altezzeSchermo[i].y - 100 && mousePosition.y < altezzeSchermo[i].y + 100)
                {
                    if (mousePosition.x > screenWidth / 2)
                    {
                        tuttiOstacoli[i].transform.localPosition += new Vector3(Time.deltaTime * 15, 0, 0);
                        ostacoloInMovimento = i;
                    }
                    else if (mousePosition.x < screenWidth / 2)
                    {
                        tuttiOstacoli[i].transform.localPosition -= new Vector3(Time.deltaTime * 15, 0, 0);
                        ostacoloInMovimento = i;
                    }
                    premuto = true;
                    break;
                }
            }
        }
    }
    public void MovimentoOstacoliTouch() // movimento ostacoli touch : tieni premuto a dx o a sx all altezza dell' ostacolo per spostarlo
    {
        Vector3 touchPosition = Input.GetTouch(0).position;
        float screenHeight = Screen.height;
        float screenWidth = Screen.width;
        int i = 0;
        if (premuto == true)
        {
            i = ostacoloInMovimento;

            //float distanzaOstacoloTouch = touchPosition.x - altezzeSchermo[i].x;
            //if (distanzaOstacoloTouch > 12 || distanzaOstacoloTouch < -12)
            //{
            if (touchPosition.x > screenWidth / 2)
            {
                tuttiOstacoli[i].transform.localPosition += new Vector3(Time.deltaTime * 15, 0, 0);
                ostacoloInMovimento = i;
            }
            else if (touchPosition.x < screenWidth / 2)
            {
                tuttiOstacoli[i].transform.localPosition -= new Vector3(Time.deltaTime * 15, 0, 0);
                ostacoloInMovimento = i;
            }
        }
        else
        {
            for (i = 0; i < altezzeSchermo.Count; i++)
            {
                if ((touchPosition.y > altezzeSchermo[i].y - 100 && touchPosition.y < altezzeSchermo[i].y + 100))
                {
                    if (touchPosition.x > screenWidth / 2)
                    {
                        tuttiOstacoli[i].transform.localPosition += new Vector3(Time.deltaTime * 15, 0, 0);
                        ostacoloInMovimento = i;
                    }
                    else if (touchPosition.x < screenWidth / 2)
                    {
                        tuttiOstacoli[i].transform.localPosition -= new Vector3(Time.deltaTime * 15, 0, 0);
                        ostacoloInMovimento = i;
                    }
                    premuto = true;
                    break;
                }

            }
        }


    }

    public void GeneraStep()
    {

        print("GeneraStep");
        int stepincorso = Mathf.FloorToInt(stepInCorso);

        print(stepincorso);
        int random = Random.Range(0, 16);
        listaAumentiDifficolta[random]();
        print("Aumento");
        if (pallina.transform.position.y < 1400)
        {
            listaAumentiDifficolta[random]();
            listaAumentiDifficolta[random]();
            print("Aumento");
            print("Aumento");


        }
        int randomProb = Random.Range(1, 20);
        if (randomProb > probabilitaDoppioOstacolo)
        {
            GeneraStepNormale();
        }
        else
        {
            GeneraStepDoppioOstacolo();
        }
        stepInCorso += 0.2f;

        altezzeSchermo.Clear();
        foreach (Transform ostacolo in gameObject.transform) // acquisisce le altezze iniziali degli ostacoli del livello e gli ostacoli stessi
        {
            if (ostacolo.gameObject.tag == "Ostacolo")
            {
                altezzeSchermo.Add(cam.WorldToScreenPoint(ostacolo.localPosition));
            }
        }

    }
    public void GeneraStepNormale()
    {
        bool arrivatoLimite = false;
        int i;
        float x = GeneraFloat(minOstacoloX, maxOstacoloX);

        while (!arrivatoLimite)
        {
            bool destra = true;
            bool sinistra = true;
            if ((stepInCorso) * 300 - nodiLinea[nodiLinea.Count - 1].y > yMaxLinea)
            {
                //MECCANISMO DI SPAWN NODI RISPETTANDO LA DISTANZA MINIMA DA LA X DI UN NODO E LA X DI QUELLO DOPOss
                if (nodiLinea[nodiLinea.Count - 1].x + distanzaXNodi >= 9)
                {
                    destra = false;
                }
                else if (nodiLinea[nodiLinea.Count - 1].x - distanzaXNodi <= -9)
                {
                    sinistra = false;
                }
                if (destra && sinistra)
                {
                    int randomNumber = Random.Range(1, 2);
                    if (randomNumber == 1) // DESTRA
                    {
                        nodiLinea.Add(GeneraVector3(nodiLinea[nodiLinea.Count - 1].x + distanzaXNodi, +9f, nodiLinea[nodiLinea.Count - 1].y + yMinLinea, nodiLinea[nodiLinea.Count - 1].y + yMaxLinea));
                    }
                    else //SINISTRA
                    {
                        nodiLinea.Add(GeneraVector3(-9, nodiLinea[nodiLinea.Count - 1].x - distanzaXNodi, nodiLinea[nodiLinea.Count - 1].y + yMinLinea, nodiLinea[nodiLinea.Count - 1].y + yMaxLinea));
                    }
                }
                else if (!destra)
                {
                    nodiLinea.Add(GeneraVector3(-9, nodiLinea[nodiLinea.Count - 1].x - distanzaXNodi, nodiLinea[nodiLinea.Count - 1].y + yMinLinea, nodiLinea[nodiLinea.Count - 1].y + yMaxLinea));
                }
                else
                {
                    nodiLinea.Add(GeneraVector3(nodiLinea[nodiLinea.Count - 1].x + distanzaXNodi, +9f, nodiLinea[nodiLinea.Count - 1].y + yMinLinea, nodiLinea[nodiLinea.Count - 1].y + yMaxLinea));
                }
                diffYNodi.Add(Mathf.Abs(nodiLinea[nodiLinea.Count - 1].y - nodiLinea[nodiLinea.Count - 2].y));
                diffXNodi.Add(Mathf.Abs(nodiLinea[nodiLinea.Count - 1].x - nodiLinea[nodiLinea.Count - 2].x));
            }
            else
            {
                arrivatoLimite = true;
            }

        }
        linea.positionCount = nodiLinea.Count;
        linea.SetPositions(nodiLinea.ToArray());
        arrivatoLimite = false;

        if (tuttiOstacoli.Count == 0)
        {
            tuttiOstacoli.Insert(0, Instantiate(ostacolo, GeneraPuntoSpawnOstacolo(), Quaternion.identity));
            for (i = 0; i < tuttiOstacoli[0].transform.childCount; i++)
            {
                tuttiOstacoli[0].transform.GetChild(i).transform.localScale = new Vector3(x, 1, 1);
            }
            tuttiOstacoli[0].transform.parent = gameObject.transform;
            ultimoOstacolo = tuttiOstacoli[0].transform.position;

        }

        while (!arrivatoLimite && nodiLinea[nodiLinea.Count - 2].y > tuttiOstacoli[0].transform.position.y)

        {
            x = GeneraFloat(minOstacoloX, maxOstacoloX);

            if (stepInCorso * 200 - tuttiOstacoli[0].transform.position.y > yMaxOstacoli)
            {
                tuttiOstacoli.Insert(0, Instantiate(ostacolo, GeneraPuntoSpawnOstacolo(), Quaternion.identity));

                for (i = 0; i < tuttiOstacoli[0].transform.childCount; i++)
                {
                    tuttiOstacoli[0].transform.GetChild(i).transform.localScale = new Vector3(x, 1, 1);
                }
                tuttiOstacoli[0].transform.parent = gameObject.transform;
                ultimoOstacolo = tuttiOstacoli[0].transform.position;
            }
            else
            {
                arrivatoLimite = true;
            }
        }
    }

    //public void Step1()
    //{

    //}
    //public void Step2()
    //{
    //    yMinOstacoli -= 2f;
    //    print("Step2");
    //}
    //public void Step3()
    //{
    //    yMinLinea -= 1f;
    //    yMaxLinea -= 2f;
    //    distanzaXNodi++;
    //    print("Step3");
    //}
    //public void Step4()
    //{
    //    yMaxOstacoli -= 2f;
    //}
    //public void Step5()
    //{
    //    accPallina.velocitàPallina += 2;
    //}
    //public void Step6()
    //{
    //    minOstacoloX += 1.5f;
    //    maxOstacoloX += 2f;
    //}
    //public void Step7()
    //{
    //    yMinOstacoli -= 0.5f;
    //    yMaxOstacoli -= 0.5f;
    //}
    //public void Step8()
    //{
    //    yMinLinea -= 1f;
    //    yMaxLinea -= 2f;
    //    distanzaXNodi++;
    //}
    //public void Step9()
    //{
    //    yMaxOstacoli -= 0.5f;
    //}
    //public void Step10()
    //{
    //    accPallina.velocitàPallina += 1;
    //}
    //public void Step11()
    //{
    //    minOstacoloX += 1f;
    //    maxOstacoloX += 1f;
    //}
    //public void Step12()
    //{
    //    yMinOstacoli -= 0.5f;
    //    yMaxOstacoli -= 0.5f;
    //}
    //public void Step13()
    //{
    //    yMaxLinea -= 1f;
    //}
    //public void Step14()
    //{
    //    yMaxOstacoli -= 0.5f;
    //}
    //public void Step15()
    //{
    //    accPallina.velocitàPallina += 1;
    //}
    //public void Step16()
    //{
    //    minOstacoloX += 1f;
    //}
    //public void Step17()
    //{
    //    yMinOstacoli -= 0.5f;
    //    yMaxOstacoli -= 0.5f;
    //}
    //public void Step18()
    //{
    //    yMaxLinea -= 1f;
    //}
    //public void Step19()
    //{
    //    yMaxOstacoli -= 0.5f;
    //}
    //public void Step20()
    //{
    //    accPallina.velocitàPallina += 1;
    //}
    //// doppio step
    //public void Step21()
    //{
    //    maxOstacoloX += 1;
    //}
    //public void Step22()
    //{
    //    doppioOstacolo = true;
    //    minOstacoloDoppioX = 2;
    //    maxOstacoloDoppioX = 3.5f;
    //    minOstacoloX = 4;
    //    maxOstacoloX = 8;

    //}
    //public void Step23()
    //{
    //    minOstacoloX += 1;
    //}
    //public void Step24()
    //{
    //    maxOstacoloX += 1;
    //}
    //public void Step25()
    //{
    //    maxOstacoloDoppioX += 0.5f;
    //}

    //public void Step20()
    //{
    //    accPallina.velocitàPallina += 1;
    //}
    //public void Step21()
    //{
    //    minOstacoloX += 1f;
    //}
    //public void Step22()
    //{
    //    yMinOstacoli -= 1f;
    //    yMaxOstacoli -= 1f;
    //}
    //public void Step23()
    //{
    //    yMinLinea -= 1f;
    //    yMaxLinea -= 1f;
    //}
    //public void Step24()
    //{
    //    yMaxOstacoli -= 1f;
    //}
    //public void Step25()
    //{
    //    accPallina.velocitàPallina += 1;
    //}
    public void CompilaListaGenerazioni()
    {
        //listaGenerazioni.Add(Step1);
        //listaGenerazioni.Add(Step2);
        //listaGenerazioni.Add(Step3);
        //listaGenerazioni.Add(Step4);
        //listaGenerazioni.Add(Step5);
        //listaGenerazioni.Add(Step6);
        //listaGenerazioni.Add(Step7);
        //listaGenerazioni.Add(Step8);
        //listaGenerazioni.Add(Step9);
        //listaGenerazioni.Add(Step10);
        //listaGenerazioni.Add(Step11);
        //listaGenerazioni.Add(Step12);
        //listaGenerazioni.Add(Step13);
        //listaGenerazioni.Add(Step14);
        //listaGenerazioni.Add(Step15);
        //listaGenerazioni.Add(Step16);
        //listaGenerazioni.Add(Step17);
        //listaGenerazioni.Add(Step18);
        //listaGenerazioni.Add(Step19);
        //listaGenerazioni.Add(Step20);
        //listaGenerazioni.Add(Step21);
        //listaGenerazioni.Add(Step22);
        //listaGenerazioni.Add(Step23);
        //listaGenerazioni.Add(Step24);
        //listaGenerazioni.Add(Step25);
    }
    //public void DistruggiOstacoloPassato()
    //{
    //    DestroyImmediate(tuttiOstacoli[tuttiOstacoli.Count - 1]);

    //}

    // STEP CON SPAWN DI DUE OSTACOLI SULLA STESSA LINEA
    public void GeneraStepDoppioOstacolo()
    {
        int i;
        bool arrivatoLimite = false;
        float xDoppio = GeneraFloat(minOstacoloDoppioX, maxOstacoloDoppioX);
        float xSingolo = GeneraFloat(minOstacoloX, maxOstacoloX);
        float minPosXOstacolo2 = 0;
        float x = GeneraFloat(minOstacoloX, maxOstacoloX);


        while (!arrivatoLimite)
        {
            bool destra = true;
            bool sinistra = true;
            if ((stepInCorso) * 300 - nodiLinea[nodiLinea.Count - 1].y > yMaxLinea)
            {
                //MECCANISMO DI SPAWN NODI RISPETTANDO LA DISTANZA MINIMA DA LA X DI UN NODO E LA X DI QUELLO DOPOss
                if (nodiLinea[nodiLinea.Count - 1].x + distanzaXNodi >= 9)
                {
                    destra = false;
                }
                else if (nodiLinea[nodiLinea.Count - 1].x - distanzaXNodi <= -9)
                {
                    sinistra = false;
                }
                if (destra && sinistra)
                {
                    int randomNumber = Random.Range(1, 2);
                    if (randomNumber == 1) // DESTRA
                    {
                        nodiLinea.Add(GeneraVector3(nodiLinea[nodiLinea.Count - 1].x + distanzaXNodi, +9f, nodiLinea[nodiLinea.Count - 1].y + yMinLinea, nodiLinea[nodiLinea.Count - 1].y + yMaxLinea));
                    }
                    else //SINISTRA
                    {
                        nodiLinea.Add(GeneraVector3(-9, nodiLinea[nodiLinea.Count - 1].x - distanzaXNodi, nodiLinea[nodiLinea.Count - 1].y + yMinLinea, nodiLinea[nodiLinea.Count - 1].y + yMaxLinea));
                    }
                }
                else if (!destra)
                {
                    nodiLinea.Add(GeneraVector3(-9, nodiLinea[nodiLinea.Count - 1].x - distanzaXNodi, nodiLinea[nodiLinea.Count - 1].y + yMinLinea, nodiLinea[nodiLinea.Count - 1].y + yMaxLinea));
                }
                else
                {
                    nodiLinea.Add(GeneraVector3(nodiLinea[nodiLinea.Count - 1].x + distanzaXNodi, +9f, nodiLinea[nodiLinea.Count - 1].y + yMinLinea, nodiLinea[nodiLinea.Count - 1].y + yMaxLinea));
                }
                diffYNodi.Add(Mathf.Abs(nodiLinea[nodiLinea.Count - 1].y - nodiLinea[nodiLinea.Count - 2].y));
                diffXNodi.Add(Mathf.Abs(nodiLinea[nodiLinea.Count - 1].x - nodiLinea[nodiLinea.Count - 2].x));
            }
            else
            {
                arrivatoLimite = true;
            }
        }

        linea.positionCount = nodiLinea.Count;
        linea.SetPositions(nodiLinea.ToArray());

        arrivatoLimite = false;


        if (tuttiOstacoli.Count == 0)
        {
            tuttiOstacoli.Insert(0, Instantiate(ostacolo, GeneraPuntoSpawnOstacolo(), Quaternion.identity));
            for (i = 0; i < tuttiOstacoli[0].transform.childCount; i++)
            {
                tuttiOstacoli[0].transform.GetChild(i).transform.localScale = new Vector3(x, 1, 1);
            }
            tuttiOstacoli[0].transform.parent = gameObject.transform;
            ultimoOstacolo = tuttiOstacoli[0].transform.position;
        }
        else
        {
            while (!arrivatoLimite && nodiLinea[nodiLinea.Count - 2].y > tuttiOstacoli[0].transform.position.y)
            {
                if (stepInCorso * 200 - tuttiOstacoli[0].transform.position.y > yMaxOstacoli)
                {
                    float altezza2Ostacoli = GeneraFloat(ultimoOstacolo.y + yMinOstacoli, ultimoOstacolo.y + yMaxOstacoli);

                    bool destra = Random.value > 0.5f;
                    float minPosXOstacolo1 = 1;

                    if (destra)
                    {
                        tuttiOstacoli.Insert(0, Instantiate(ostacolo, GeneraVector3(minPosXOstacolo1, 7, altezza2Ostacoli, altezza2Ostacoli), Quaternion.identity));
                    }
                    else
                    {
                        tuttiOstacoli.Insert(0, Instantiate(ostacolo, GeneraVector3(-7, -minPosXOstacolo1, altezza2Ostacoli, altezza2Ostacoli), Quaternion.identity));
                    }


                    for (i = 0; i < tuttiOstacoli[0].transform.childCount; i++)
                    {
                        tuttiOstacoli[0].transform.GetChild(i).transform.localScale = new Vector3(xDoppio, 1, 1);
                    }

                    xDoppio = GeneraFloat(minOstacoloDoppioX, maxOstacoloDoppioX);
                    float posXOstacolo1 = tuttiOstacoli[0].transform.position.x;
                    minPosXOstacolo2 = tuttiOstacoli[0].transform.localScale.x / 2 + 1 + xDoppio / 2;

                    if (posXOstacolo1 < 0)
                    {
                        Instantiate(ostacolo, GeneraVector3(minPosXOstacolo2, 7f, altezza2Ostacoli, altezza2Ostacoli), Quaternion.identity).transform.parent = tuttiOstacoli[0].gameObject.transform;
                    }
                    else
                    {
                        Instantiate(ostacolo, GeneraVector3(-7, -minPosXOstacolo2, altezza2Ostacoli, altezza2Ostacoli), Quaternion.identity).transform.parent = tuttiOstacoli[0].gameObject.transform;
                    }

                    for (i = 0; i < tuttiOstacoli[0].transform.GetChild(3).transform.childCount; i++)
                    {
                        tuttiOstacoli[0].transform.GetChild(3).transform.GetChild(i).transform.localScale = new Vector3(xDoppio, 1, 1);
                    }




                    tuttiOstacoli[0].transform.parent = gameObject.transform;
                    ultimoOstacolo = tuttiOstacoli[0].transform.position;
                }
                else
                {
                    arrivatoLimite = true;
                }
            }
        }




    }
    public void SimulaStep()
    {
        int i;
        for (i = ultimoStep - 1; i > 0; i--)
        {
            listaGenerazioni[i]();
        }

    }
    public void PartiDa()
    {
        if (partiDa)
        {
            float.TryParse(placeHolder.text, out stepInCorso);
            pallina.transform.position = new Vector3(0, stepInCorso, 0);
            stepInCorso /= 100;

        }
    }
    public IEnumerator LoadingScreen()
    {
        async = SceneManager.LoadSceneAsync(0);
        async.allowSceneActivation = false;
        float tempo = 0;

        nodiLinea.Add(GeneraVector3(-9, +9f, 20, 22));

        while (nodiLinea.Count < 100) //  genera nodi linea
        {
            bool destra = true;
            bool sinistra = true;

            if (nodiLinea[nodiLinea.Count - 1].x + distanzaXNodi >= 9)
            {
                destra = false;
            }
            else if (nodiLinea[nodiLinea.Count - 1].x - distanzaXNodi <= -9)
            {
                sinistra = false;
            }
            if (destra && sinistra)
            {
                int randomNumber = Random.Range(1, 2);
                if (randomNumber == 1) // DESTRA
                {
                    nodiLinea.Add(GeneraVector3(nodiLinea[nodiLinea.Count - 1].x + distanzaXNodi, +9f, nodiLinea[nodiLinea.Count - 1].y + yMinLinea, nodiLinea[nodiLinea.Count - 1].y + yMaxLinea));
                }
                else //SINISTRA
                {
                    nodiLinea.Add(GeneraVector3(-9, nodiLinea[nodiLinea.Count - 1].x - distanzaXNodi, nodiLinea[nodiLinea.Count - 1].y + yMinLinea, nodiLinea[nodiLinea.Count - 1].y + yMaxLinea));
                }
            }
            else if (!destra)
            {
                nodiLinea.Add(GeneraVector3(-9, nodiLinea[nodiLinea.Count - 1].x - distanzaXNodi, nodiLinea[nodiLinea.Count - 1].y + yMinLinea, nodiLinea[nodiLinea.Count - 1].y + yMaxLinea));
            }
            else
            {
                nodiLinea.Add(GeneraVector3(nodiLinea[nodiLinea.Count - 1].x + distanzaXNodi, +9f, nodiLinea[nodiLinea.Count - 1].y + yMinLinea, nodiLinea[nodiLinea.Count - 1].y + yMaxLinea));
            }
            diffYNodi.Add(Mathf.Abs(nodiLinea[nodiLinea.Count - 1].y - nodiLinea[nodiLinea.Count - 2].y));
            diffXNodi.Add(Mathf.Abs(nodiLinea[nodiLinea.Count - 1].x - nodiLinea[nodiLinea.Count - 2].x));
            tempo += Time.deltaTime;
            yield return null;
        }


        //spawnOstacoli.Add(GeneraVector3(-7,7,22,24));


        //while (spawnOstacoli.Count < 100)
        {
            //Vector3 ultimoOstacolo = spawnOstacoli[spawnOstacoli.Count - 1];

            while (nodiLinea[primoNodoDopoOstacolo].y < ultimoOstacolo.y)
            {
                primoNodoDopoOstacolo++;
            }
            Vector3 nodoLineaSotto = nodiLinea[primoNodoDopoOstacolo - 1];
            Vector3 nodoLineaSopra = nodiLinea[primoNodoDopoOstacolo];


            float XProssimoOstacolo = 0;
            float YProssimoOstacolo = 0;
            float differenzaNodoPiuPiccoloY = 0;
            float differenzaNodoPiuPiccoloX = 0;


            YProssimoOstacolo = GeneraFloat(ultimoOstacolo.y + yMinOstacoli, ultimoOstacolo.y + yMaxOstacoli);
            differenzaNodoPiuPiccoloY = Mathf.Abs(YProssimoOstacolo - nodoLineaSotto.y);
            differenzaNodoPiuPiccoloX = Mathf.Abs(differenzaNodoPiuPiccoloY * diffXNodi[primoNodoDopoOstacolo - 1] / diffYNodi[primoNodoDopoOstacolo - 1]);


            if (nodoLineaSopra.x < nodoLineaSotto.x)
            {
                XProssimoOstacolo = nodoLineaSotto.x - differenzaNodoPiuPiccoloX;
            }
            else
            {
                XProssimoOstacolo = nodoLineaSotto.x + differenzaNodoPiuPiccoloX;
            }
            bool destra = Random.value > 0.5f;


            if (destra)
            {
                XProssimoOstacolo = XProssimoOstacolo + distanzaXOstacoloDaLinea;
            }
            else
            {
                XProssimoOstacolo = XProssimoOstacolo - distanzaXOstacoloDaLinea;
            }
            //spawnOstacoli.Add(new Vector3(XProssimoOstacolo, XProssimoOstacolo, 0));
            print(XProssimoOstacolo);
            print(YProssimoOstacolo);
            tempo += Time.deltaTime;
            yield return null;
        }
        async.allowSceneActivation = true;
        print(tempo);
        yield return null;


    }
    public Vector3 GeneraPuntoSpawnOstacolo()
    {
        float XProssimoOstacolo = 0;
        float differenzaNodoPiuPiccoloY = 0;
        float differenzaNodoPiuPiccoloX = 0;
        if (tuttiOstacoli.Count == 0)
        {
            Vector3 ultimoOstacolo = new Vector3(0, 25, 0);
        }
        else
        {
            Vector3 ultimoOstacolo = tuttiOstacoli[0].transform.position;
        }
        float YProssimoOstacolo = 0;

        YProssimoOstacolo = GeneraFloat(ultimoOstacolo.y + yMinOstacoli, ultimoOstacolo.y + yMaxOstacoli);

        int random = Random.Range(1, 20);
        if (random > probabilitaOstacoloSuLinea)
        {
            XProssimoOstacolo = GeneraFloat(-7, 7);
        }
        else
        {

            if (!(nodiLinea[primoNodoDopoOstacolo].y > YProssimoOstacolo))
            {
                while (nodiLinea[primoNodoDopoOstacolo].y < YProssimoOstacolo)
                {
                    primoNodoDopoOstacolo++;
                }
            }


            Vector3 nodoLineaSotto = nodiLinea[primoNodoDopoOstacolo - 1];
            Vector3 nodoLineaSopra = nodiLinea[primoNodoDopoOstacolo];

            differenzaNodoPiuPiccoloY = Mathf.Abs(YProssimoOstacolo - nodoLineaSotto.y);
            differenzaNodoPiuPiccoloX = Mathf.Abs(differenzaNodoPiuPiccoloY * diffXNodi[primoNodoDopoOstacolo - 1] / diffYNodi[primoNodoDopoOstacolo - 1]);

            if (nodoLineaSopra.x < nodoLineaSotto.x)
            {
                XProssimoOstacolo = nodoLineaSotto.x - differenzaNodoPiuPiccoloX;
            }
            else
            {
                XProssimoOstacolo = nodoLineaSotto.x + differenzaNodoPiuPiccoloX;
            }

            bool destra;

            distanzaXOstacoloDaLinea = GeneraFloat(minDistanzaOstacoloSuLinea, maxDistanzaOstacoloSuLinea);


            if (XProssimoOstacolo + distanzaXOstacoloDaLinea > 7.5f)
            {
                destra = false;
            }
            else if (XProssimoOstacolo - distanzaXOstacoloDaLinea < -7.5f)
            {
                destra = true;
            }
            else
            {
                destra = Random.value > 0.5f;
            }




            if (destra)
            {
                XProssimoOstacolo = XProssimoOstacolo + distanzaXOstacoloDaLinea;
            }
            else
            {
                XProssimoOstacolo = XProssimoOstacolo - distanzaXOstacoloDaLinea;
            }
        }

        //spawnOstacoli.Add(new Vector3(XProssimoOstacolo, XProssimoOstacolo, 0));
        return new Vector3(XProssimoOstacolo, YProssimoOstacolo, 0);
    }
    public void MinOstacoloX()
    {
        if (minOstacoloX < 6)
        {
            minOstacoloX += 0.5f;
        }
    }
    public void MaxOstacoloX()
    {
        if (maxOstacoloX < 8)
        {
            maxOstacoloX += 0.5f;
        }
    }
    public void MinYOstacoli()
    {
        if (yMinOstacoli > 10)
        {
            yMinOstacoli -= 0.5f;
        }

    }
    public void MaxYOstacoli()
    {
        if (yMaxOstacoli > 13)
        {
            yMaxOstacoli -= 0.5f;
        }
    }
    public void MinXOstacoloDoppio()
    {
        if (minOstacoloDoppioX < 4f)
        {
            maxOstacoloDoppioX += 0.5f;
        }
    }
    public void MaxXOstacoloDoppio()
    {
        if (maxOstacoloDoppioX < 5.5f)
        {
            maxOstacoloDoppioX += 0.5f;
        }
    }
    public void MinYNodi()
    {
        if (yMinLinea > 10)
        {
            yMinLinea -= 0.5f;
        }
    }
    public void MaxYNodi()
    {
        if (yMaxLinea > 13)
        {
            yMaxLinea -= 0.5f;
        }
    }
    public void DistanzaXNodi()
    {
        if (distanzaXNodi < 4)
        {
            distanzaXNodi += 0.5f;
        }

    }
    public void ProbabilitaOstacoloSuLinea()
    {
        if (probabilitaOstacoloSuLinea < 15)
        {
            probabilitaOstacoloSuLinea++;
        }
    }
    public void ProbabilitaOstacoloDoppio()
    {
        if (probabilitaDoppioOstacolo < 15)
        {
            probabilitaDoppioOstacolo++;
        }
    }
    public void MaxDistanzaXSuLinea()
    {
        if (maxDistanzaOstacoloSuLinea > 0.5f)
        {
            maxDistanzaOstacoloSuLinea -= 0.5f;
        }
    }
    public void MinDistanzaXSuLinea()
    {
        if (minDistanzaOstacoloSuLinea > 0.5f)
        {
            minDistanzaOstacoloSuLinea -= 0.5f;
        }
    }
    public void VelocitaPallina()
    {
        accPallina.velocitàPallina++;
    }
    public void DurataSparizione()
    {
        durataSparizione += 0.2f;
    }
    public void TempoTraSparizioni()
    {
        tempoTraSparizioni -= 2f;
    }
    public void CompilaListaAumentiDifficolta()
    {
        listaAumentiDifficolta.Add(MinOstacoloX);
        listaAumentiDifficolta.Add(MaxOstacoloX);
        listaAumentiDifficolta.Add(MinXOstacoloDoppio);
        listaAumentiDifficolta.Add(MaxXOstacoloDoppio);
        listaAumentiDifficolta.Add(MinYOstacoli);
        listaAumentiDifficolta.Add(MaxYOstacoli);
        listaAumentiDifficolta.Add(MinYNodi);
        listaAumentiDifficolta.Add(MaxYNodi);
        listaAumentiDifficolta.Add(DistanzaXNodi);
        listaAumentiDifficolta.Add(ProbabilitaOstacoloSuLinea);
        listaAumentiDifficolta.Add(ProbabilitaOstacoloDoppio);
        listaAumentiDifficolta.Add(MinDistanzaXSuLinea);
        listaAumentiDifficolta.Add(MaxDistanzaXSuLinea);
        listaAumentiDifficolta.Add(VelocitaPallina);
        listaAumentiDifficolta.Add(DurataSparizione);
        listaAumentiDifficolta.Add(TempoTraSparizioni);
    }
    public IEnumerator SparizioneLinea()
    {
        print("Sparizione");
        linea.GetComponent<LineRenderer>().material = trasparente;
        linea.GetComponent<LineRenderer>().material = trasparente;
        linea.GetComponent<LineRenderer>().startColor = lineaTrasperente;
        linea.GetComponent<LineRenderer>().endColor = lineaTrasperente;

        yield return new WaitForSeconds(durataSparizione);

        linea.GetComponent<LineRenderer>().material = lineaNormale;
        linea.GetComponent<LineRenderer>().material = lineaNormale;
        linea.GetComponent<LineRenderer>().startColor = Color.white;
        linea.GetComponent<LineRenderer>().endColor = Color.white;

        yield return null;
    }
}





