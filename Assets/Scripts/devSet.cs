using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using DeadMosquito.AndroidGoodies;

public class devSet : MonoBehaviour {

    public Text battery;
    public Text connection;
    public Text info;
    public Text total;

    public string status;
    public string wifiInfo;

    public GameObject setts;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        battery.text = ("Battery level: " + AGBattery.GetBatteryChargeLevel());
        if (AGNetwork.IsInternetAvailable())
        {
            if (AGNetwork.IsWifiEnabled())
            {
                if (AGNetwork.IsWifiConnected())
                {
                    status = "Wi-Fi ";
                    wifiInfo = ("Speed: " + AGNetwork.GetWifiConnectionInfo().LinkSpeed + "Signal: " + AGNetwork.GetWifiSignalLevel() + "/100");
                }
                else
                {
                    if (AGNetwork.IsMobileConnected())
                    {
                        status = "4G";
                        wifiInfo = ("Speed: null");
                    }
                }
            }
            connection.text = ("Тип соединения: " + status + wifiInfo);
        }
        else
        {
            connection.text = ("Соединение не установлено.");
        }
        total.text = ("Батарея: " + AGBattery.GetBatteryChargeLevel() + " " + status + " Этап: null");
	}
}
