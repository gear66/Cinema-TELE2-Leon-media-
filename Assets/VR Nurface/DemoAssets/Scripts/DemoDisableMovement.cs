using UnityEngine;
using System.Collections;

public class DemoDisableMovement : MonoBehaviour {

    private DemoAutowalk playerAutowalk;

	// Use this for initialization
	void Start () {
        playerAutowalk = GameObject.FindObjectOfType<DemoAutowalk>().GetComponent<DemoAutowalk>();
        playerAutowalk.allowMovement = false;
	}
	
	// Update is called once per frame
	void Update () {
	}
}
