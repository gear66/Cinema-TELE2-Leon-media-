using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playOndestroy : MonoBehaviour {

    public GameObject splash;
    public int flag;

	// Use this for initialization
	void Start () {
        flag = 1;
	}
	
	// Update is called once per frame
	void Update () {
		if (!splash.activeSelf & flag == 1)
        {
            GetComponent<MediaPlayerCtrl>().Play();
            flag = 2;
        }
        if(flag == 2)
        {
            //GetComponent<MediaPlayerCtrl>().OnEnd.
        }
	}

    public void Sas()
    {
        //
    }
}
