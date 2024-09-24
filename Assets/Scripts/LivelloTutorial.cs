using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivelloTutorial : MonoBehaviour
{


    public Camera cam;
    public List<GameObject> tuttiOstacoli;
    public bool premuto = false;
    public List<Vector3> altezzeSchermo;
    public int ostacoloInMovimento;




    private void Start()
    {
        foreach (Transform ostacolo in gameObject.transform) // acquisisce le altezze iniziali degli ostacoli del livello e gli ostacoli stessi
        {
            if (ostacolo.gameObject.tag == "Ostacolo")
            {
                altezzeSchermo.Add(cam.WorldToScreenPoint(ostacolo.localPosition));
            }
        }
    }
    private void FixedUpdate()
    {
        AggiornaAltezze();
        if (Input.GetMouseButton(0))
        {
            MovimentoOstacoliMouse();
        }
        else
        {
            premuto = false;
        }
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began && Input.GetTouch(0).phase == TouchPhase.Stationary && Input.GetTouch(0).phase == TouchPhase.Moved)
        //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {

            MovimentoOstacoliTouch();
        }
        else
        {
            //premuto = false;
        }
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
                print(i);
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

}
