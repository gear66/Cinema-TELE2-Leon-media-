using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyer : MonoBehaviour {

    public float t;
    public float time;

	// Use this for initialization
	void Start () {
        t = 0;
	}
	
	// Update is called once per frame
	void Update () {
        t += Time.deltaTime;
        if (t > time)
        {
            t = 0;
            gameObject.SetActive(false);
            //Destroy(gameObject);
        }
    }
}
