using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ostacolo : MonoBehaviour
{
    float i = 0;
    float x;
    float y;
    void Start()
    {
        if (transform.position.y < 50)
        {
            x = transform.localScale.x;
            y = transform.localScale.y;
            transform.localScale = new Vector3(0, 0, 1);
            FadeIn();
        }
    }

    // Update is called once per frame
    void Update()
    {
        

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "LimiteDX")
        {
            transform.localPosition = new Vector3(transform.localPosition.x - 65.3436f, 0, 0);
            print("Collisione");
        }
        else if(other.tag == "LimiteSX")
        {
            transform.localPosition = new Vector3(transform.localPosition.x + 65.3436f,0, 0);

        }
    }

    public void Sparizione()
    {
        DestroyImmediate(this);
    }
    public void FadeIn()
    {
        while(i <= 0.2f)
        {
            transform.localScale += new Vector3(x * 5f, y * 5f, 0) * Time.deltaTime;
            i += Time.deltaTime;
        }
        
    }
    
}
