using UnityEngine;
using System.Collections;

public class ScaleSlerp : MonoBehaviour {
	public GameObject Scene;
	public GameObject SceneStart;
	public Vector3 velicity;
	public float t;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		t = Mathf.Clamp (t, 0f, 1f);
		Scene.transform.position = Vector3.SmoothDamp (Scene.transform.position, SceneStart.transform.position,  ref velicity, 1f);
		Scene.transform.rotation = Quaternion.Slerp(Scene.transform.rotation, SceneStart.transform.rotation, t/50f);
	}
}
