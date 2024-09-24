using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    public float rotazioneX;
    public float rotazioneY;
    public float rotazioneZ;

    void Start()
    {
        
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(rotazioneX, rotazioneY, rotazioneZ));
    }   
}
