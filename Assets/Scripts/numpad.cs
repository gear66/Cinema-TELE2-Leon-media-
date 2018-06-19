using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class numpad : MonoBehaviour
{

    public InputField inp;
    public InputField namee;
    public InputField min;
    public InputField max;
    public GameObject speedoMeter;
    public string txt;
    public bool isSetip;

    // Use this for initialization
    void Start()
    {
        //isSetip = true;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PresNum(string num)
    {
        if (isSetip)
        {
            inp.text += num;
        }
        else
        {
            namee.text += num;
        }
    }

    public void Cleaar()
    {
        inp.text = "";
    }
    public void ClearName()
    {
        namee.text = "Client";
    }

    public void Sosat()
    {
        Debug.Log("sosat");
    }

    public void PlusMin()
    {
        speedoMeter.GetComponent<speedtest>().speedMin++;
        min.text = speedoMeter.GetComponent<speedtest>().speedMin.ToString();
    }
    public void PlusMax()
    {
        speedoMeter.GetComponent<speedtest>().speedMax++;
        max.text = speedoMeter.GetComponent<speedtest>().speedMax.ToString();
    }

    public void MinusMin()
    {
        speedoMeter.GetComponent<speedtest>().speedMin--;
        min.text = speedoMeter.GetComponent<speedtest>().speedMin.ToString();
    }
    public void MinusMax()
    {
        speedoMeter.GetComponent<speedtest>().speedMax--;
        max.text = speedoMeter.GetComponent<speedtest>().speedMax.ToString();
    }
}
