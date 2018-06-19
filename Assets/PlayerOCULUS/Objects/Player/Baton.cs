using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Baton : MonoBehaviour {

    //public int buttonIndex;
    public GameObject destenationObj;
    //public GameObject misc;
    public bool exit;
    public int i;

    public bool interactable { get; internal set; }
    public object onClick { get; internal set; }
    public ColorBlock colors { get; internal set; }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
	}
    //public void but()
    //{
    //        misc.SetActive(false);
    //}
}
