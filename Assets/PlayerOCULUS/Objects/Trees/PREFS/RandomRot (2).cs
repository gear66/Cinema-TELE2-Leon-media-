using UnityEngine;
using System.Collections;


[ExecuteInEditMode]

public class RandomRot : MonoBehaviour {

    public float scaleMin = 0.8f;
    public float scaleMax = 1.2f;

    public bool alreadyRotated = false;

    //public bool iWantToRotateWhenCopy = false;

    // Use this for initialization
    void Start () {
	
	}

    void Awake() {
        //if(iWantToRotateWhenCopy)
            //alreadyRotated = false;
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnValidate() {
        SetRandomRotation();
    }

    void SetRandomRotation() {
        if (!alreadyRotated)
        {
            transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
            float nawRandomScale = Random.Range(scaleMin, scaleMax);
            transform.localScale = new Vector3(nawRandomScale, nawRandomScale, nawRandomScale);
        }

        alreadyRotated = true;
    }
}
