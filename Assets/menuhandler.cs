using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class menuhandler : MonoBehaviour {
    public GameObject engine;
    public double duration;
	// Use this for initialization
	void Start () {
		//duration = GetComponent<VideoPlayer>().clip.length - 3;
        duration = GetComponent<VideoPlayer>().frameCount;
    }

    // Update is called once per frame
    void Update () {
        if (GetComponent<VideoPlayer>().frame > duration - 60)
        {
            engine.GetComponent<engineClient>().flag = 4;
            engine.GetComponent<engineClient>().GoCinema();
        }
	}
}
