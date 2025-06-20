using Drakkar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class riodespla : MonoBehaviour
{
   
    public Vector2 speed = new Vector2();
    private float tiempo;
    Renderer rend; 
    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        tiempo += Time.deltaTime;
           rend.material.SetTextureOffset("_MainTex",speed * tiempo);
    }
}
