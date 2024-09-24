using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMenu : MonoBehaviour
{
    public Animator animCoinsTXT;
    public Animator animShopBTN;
    public Animator animSpeedBTN;
    public Animator animOptionsBTN;
    public Animator animCoinIMG;
    public Animator animPlayBTN;

    public AudioSource track;

    public Image volumeBTN;
    public Sprite volumeOn;
    public Sprite volumeOff;
    public bool primoClick = false;
    public float timerAnimazioniMenu = 0;

    public MainCamera accMainCamera;
    public Camera camera;
    public UIGioco accUIGioco;
    //public Pallina accPallina;

    public Text moneteTotaliLBL;

    private void Awake()
    {
        if (PlayerPrefX.GetBool("Volume", true))
        {
            volumeBTN.sprite = volumeOn;
            track.volume = 0.5f;

        }
        else
        {
            volumeBTN.sprite = volumeOff;
            track.volume = 0;
            
        }
    }

    void Start()
    {
       
        if (PlayerPrefX.GetBool("NextLevel",false) == true)
        {
            accMainCamera.RidimensionamentoCamera();
            NexlLevelFalse();

            gameObject.SetActive(false);
            print("nextlevel");
        }
        moneteTotaliLBL.text = PlayerPrefs.GetInt("MoneteTotali", 100).ToString();

       
    }

    // Update is called once per frame
    void Update()
    {
        
       
    }
    public void NexlLevelFalse()
    {
        PlayerPrefX.SetBool("NextLevel", false);
    }
    public void Volume()
    {
        if(PlayerPrefX.GetBool("Volume",true))
        {
            PlayerPrefX.SetBool("Volume", false);
            volumeBTN.sprite = volumeOff;
            track.volume = 0;
        }
        else
        {
            PlayerPrefX.SetBool("Volume", true);
            volumeBTN.sprite = volumeOn;
            track.volume = 0.3f;
        }
    }
    public IEnumerator FadeOutMenu()
    {
        animPlayBTN.SetBool("PrimoClick", true);
        yield return new WaitForSeconds(0.01f);

        animCoinsTXT.SetBool("PrimoClick", true);
        yield return new WaitForSeconds(0.1f);

        animCoinIMG.SetBool("PrimoClick", true);
        yield return new WaitForSeconds(0.1f);

        animOptionsBTN.SetBool("PrimoClick", true);
        yield return new WaitForSeconds(0.1f);

        animShopBTN.SetBool("PrimoClick", true);
        yield return new WaitForSeconds(0.1f);

        animSpeedBTN.SetBool("PrimoClick", true);
        yield return new WaitForSeconds(1f);

        accUIGioco.score.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);

        yield return null;


    }
    public IEnumerator AumentoVolume()
    {
        if(PlayerPrefX.GetBool("Volume",true))
        {
            while (track.volume < 1)
            {
                track.volume += Time.deltaTime / 4;
                yield return null;
            }
        }
        
        yield return null;
    }
}
