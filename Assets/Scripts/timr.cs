using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timr : MonoBehaviour {

    public float t;
    public bool isactive = false;
    public GameObject browser;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        t += Time.deltaTime;
        if (isactive)
        {
            if (t > 30f)
            {
                //StartCoroutine(SpeedTestt());
                //StartCoroutine(CallWebPage());
                //UnityEngine.Debug.Log("T started");
                browser.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
    public void Detroyy()
    {
        isactive = true;
    }
}
