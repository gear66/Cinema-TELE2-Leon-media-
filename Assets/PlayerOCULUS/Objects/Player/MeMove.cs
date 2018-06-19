using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;

public class MeMove : MonoBehaviour {

    public Transform compas;
    public GameObject cam;
    public GameObject misc;
    public float speed;
    public bool needHide = false;

    public GameObject buttonBefore;
    public GameObject dot;
    public GameObject cinema;
    public GameObject vid360off;
    public GameObject vid360on;
    public GameObject browser;
    public GameObject bigscreen;

    public GameObject menu;
    public bool menuIsShown = false;

    public bool flyMode = false;

    public Transform MenuHolder;
    public float delta;

    void SwitchMenu() {

        if (menuIsShown)
        {
            menu.SetActive(false);
        }
        else {
            menu.SetActive(true);
        }
        menuIsShown = !menuIsShown;
    }

    void Start () {
        menu.SetActive(false);
    }

    float Make180Angle(float _angle) {
        if (_angle > 180f)
        {
            _angle = _angle - 360f;
        }
        
        return _angle;
    }

    void Update() {

        if (Input.GetKeyDown(KeyCode.Escape)) {
            SwitchMenu();
        }
        

        delta = Make180Angle(MenuHolder.localEulerAngles.y) - Make180Angle(compas.localEulerAngles.y);
        if (delta > 20f)
        {
            MenuHolder.localEulerAngles += new Vector3(0f, - Time.deltaTime * 60f, 0f);
        }
        if (delta < -20f)
        {
            MenuHolder.localEulerAngles += new Vector3(0f, + Time.deltaTime * 60f, 0f);
        }


        compas.transform.rotation = cam.transform.rotation;
        compas.transform.eulerAngles = new Vector3(0f, compas.transform.eulerAngles.y, 0f);

        if (!menuIsShown && !flyMode) {
            
            if (Input.GetMouseButton(0))
            {
                transform.position += compas.transform.forward * Time.deltaTime * speed;
            }
        }

        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit))
        {
            dot.transform.position = hit.point;
            //print(hit.collider.transform.gameObject.name);
            if (hit.collider.transform.tag == "button")
            {
                
                if (hit.collider.transform.gameObject != buttonBefore)
                {
                    if (buttonBefore)
                    {
                        buttonBefore.transform.localScale = new Vector3(2f, 2f, 2f);
                    }
                    buttonBefore = hit.collider.transform.gameObject;
                    hit.collider.transform.localScale *= 1.1f;
                }
                if (Input.GetMouseButtonDown(0)) {
                    Baton b = hit.collider.transform.gameObject.GetComponent<Baton>();
                    if (hit.collider.transform.gameObject.name == "b1")
                    {
                        //misc.SetActive(false);
                        GoCinema();
                    }
                    if (hit.collider.transform.gameObject.name == "b2")
                    {
                        //misc.SetActive(true);
                        Go360online();
                    }
                    if (hit.collider.transform.gameObject.name == "b3")
                    {
                       // misc.SetActive(true);
                        Go360offline();
                    }
                    if (hit.collider.transform.gameObject.name == "status")
                    {
                        //misc.SetActive(true);
                    }
                    if (hit.collider.transform.gameObject.name == "b4")
                    {
                        // misc.SetActive(true);
                        GoBrowser();
                    }




                    if (b.exit == false) {
                        Teleportate(b.destenationObj, true);
                    } else {
                        Teleportate(b.destenationObj, false);
                    }
                    
                }
            }
            else
            {
                if (buttonBefore)
                {
                    buttonBefore.transform.localScale = new Vector3(2f, 2f, 2f);
                    buttonBefore = null;
                }
            }
        }
        else {
            dot.transform.position = new Vector3(0f, 0f, 0f);
        }


    }

    public void Teleportate(GameObject destenationObj, bool _flyMode) {
        SwitchMenu();
        flyMode = _flyMode;

        transform.position = destenationObj.transform.position;
        transform.rotation = destenationObj.transform.rotation;
        if (_flyMode)
        {
            GetComponent<Rigidbody>().isKinematic = true;
        }
        else
        {
            GetComponent<Rigidbody>().isKinematic = false;
        }
        
    }

    void OnApplicationPause(bool isPaused) {
        UnityEngine.XR.XRSettings.enabled = !isPaused;
    }

    public void Go360online()
    {
        browser.SetActive(false);
        cinema.SetActive(false);
        vid360on.SetActive(true);
        vid360off.SetActive(false);
    }

    public void Go360offline()
    {
        browser.SetActive(false);
        cinema.SetActive(false);
        vid360on.SetActive(false);
        vid360off.SetActive(true);
        browser.SetActive(false);
    }

    public void GoCinema()
    {
        cinema.SetActive(true);
        vid360on.SetActive(false);
        vid360off.SetActive(false);
        browser.SetActive(false);
        bigscreen.SetActive(true);
    }

    public void CheckConnect()
    {

    }
    
    public void GoBrowser()
    {
        vid360on.SetActive(false);
        vid360off.SetActive(false);
        cinema.SetActive(true);
        bigscreen.SetActive(false);
        browser.SetActive(true);
    }
}
