using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camMov : MonoBehaviour {

    public float speed = 0.1f;
    public float smoothSpeed = 10f;
    public float x;

	void Start () {
	}

    void Update()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            transform.Rotate(new Vector3(0, speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.E))
        {
            transform.Rotate(new Vector3(0, speed * Time.deltaTime, 0));
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(new Vector3(0, 0, speed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.Space))
        {
            transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(new Vector3(0, 0, -speed * Time.deltaTime));
        }
        if (Input.GetKey(KeyCode.C))
        {
            transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            transform.Translate(new Vector3(0, 0, (speed/3 * Time.deltaTime)));
        }
    }
}
