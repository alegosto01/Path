using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PallinaTutorial : MonoBehaviour
{
    public MainCamera accMainCamera;
    public LineRenderer Linea;
    public int targetIndex;
    public float velocitàPallina;
    public List<Vector3> nodiLinea;
    public GameObject Camera;
    public bool primoClick = false;
    public GameObject letsPlay;
    
    //public GameObject fineLivelloPanel;
    //public Gameplay accGameplay;
    // public Vector3[] targetLinea;
    void Start()
    {
        CalcoloAngolo();
        print(Linea.GetPosition(1).y);
        // targetIndex = 0;
    }
    // Update is called once per frame
    private void FixedUpdate()
    {
        if(Input.GetMouseButton(0) || Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            primoClick = true;
        }
        if (primoClick)
        {
            SeguiLinea();
        }
        Camera.transform.position = new Vector3(0, transform.position.y + 19, -10);
        if(transform.position.y > 38)
        {
            letsPlay.SetActive(true);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        SceneManager.LoadScene(1);
    }
    public void SeguiLinea() // movimento palla che segue la traiettoria della linea raggiungendo un dopo l altro i target della linea
    {
        if (transform.position.y >= Linea.GetPosition(targetIndex).y)
        {
            if (targetIndex + 1 < Linea.positionCount)
            {
                targetIndex++;
                CalcoloAngolo();
            }
        }
        else
        {
            transform.Translate(Vector3.up * Time.deltaTime * velocitàPallina * 1.01f); // il * 1.01 è perche se no piano piano la pallina va sempre piu in basso
        }
    }
    public void CalcoloAngolo() // calcola l' angolazione che deve assumere la pallina ad ogni target della linea. Assegna la velocità alla telecamera
    {
        float catetoBase = Mathf.Abs(transform.position.x - Linea.GetPosition(targetIndex).x);
        float catetoDue = Mathf.Abs(transform.position.y - Linea.GetPosition(targetIndex).y);
        float ipotenusa = Mathf.Sqrt(catetoDue * catetoDue + catetoBase * catetoBase);
        float cosAngolo = catetoDue / ipotenusa;
        float angolo = Mathf.Acos(cosAngolo) * Mathf.Rad2Deg;
        if (transform.position.x < Linea.GetPosition(targetIndex).x)
        {
            angolo = -angolo;
        }
        transform.rotation = Quaternion.Euler(0, 0, angolo);
        print(angolo);
        accMainCamera.velocitàCamera = cosAngolo * velocitàPallina;
    }
    public void SceneChange()
    {
       SceneManager.LoadScene(0);
        PlayerPrefX.SetBool("NextLevel", true);
    }
}
