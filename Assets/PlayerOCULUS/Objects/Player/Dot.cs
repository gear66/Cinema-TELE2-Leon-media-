using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dot : MonoBehaviour {

    public Transform cam;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        float scale = Vector3.Distance(cam.transform.position, transform.position) * 0.02f;
        transform.localScale = new Vector3(scale, scale, scale);

    }
}
