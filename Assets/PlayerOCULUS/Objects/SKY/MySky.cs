using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySky : MonoBehaviour {

    public float t = 0f;

    public float speed = 1f;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        t += Time.deltaTime * speed;



        GetComponent<Renderer>().material.SetTextureOffset("_MainTex", new Vector2(t*0.005f, 0f));
	}
}
