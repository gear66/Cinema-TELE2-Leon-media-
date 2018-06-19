using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

public class enginee : MonoBehaviour {
     
    public Text ipTextbox;

	// Use this for initialization
	void Start () {
        GetLocalIPAddress();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                ipTextbox.text = ip.ToString();
                //return ip.ToString();
            }
        }
    }
}
