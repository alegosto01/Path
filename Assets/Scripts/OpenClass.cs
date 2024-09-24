using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenClass : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public static IEnumerator Wait(float tempo)
    {
        print("ciao");
        while(tempo < 0)
        {
            tempo -= Time.deltaTime;
            
            yield return null;
        }
    }
}
