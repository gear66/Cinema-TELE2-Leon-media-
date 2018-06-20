using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DeadMosquito.AndroidGoodies;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using UnityEngine.Networking.Types;
using UnityEngine.Networking.Match;
using Prototype.NetworkLobby;

public class devSett : MonoBehaviour
{

    public string output;
    string status;
    string wifiInfo;
    public float t;
    public float tt;
    public float check;
    public bool isActive = true;
    public bool isServer;
    public InputField playerData;
    public GameObject engine;
    public float duration;
    public string vidStat;
    public string currentState;

    void Start()
    {
        playerData.text = ("Статус:");
    }

    void Update()
    {
        t += Time.deltaTime;
        tt += Time.deltaTime;
        if (isActive)
        {
            duration = engine.GetComponent<engineClient>().duration;
            if (tt > 1)
            {
                Payload payload = new Payload();
                payload.duration = duration;
                LobbyManager.requests["sendDuration"](payload);
                currentState = engine.GetComponent<engineClient>().currentState;
                tt = 0;
            }
            if (t > 3)
            {
                check = engine.GetComponent<engineClient>().flag;
                if (AGNetwork.IsInternetAvailable())
                {
                    if (AGNetwork.IsWifiEnabled())
                    {
                        if (AGNetwork.IsWifiConnected())
                        {
                            status = "Wi-Fi";
                            wifiInfo = (" Speed: " + AGNetwork.GetWifiConnectionInfo().LinkSpeed + " Signal: " + AGNetwork.GetWifiSignalLevel() + "/100");
                        }
                        else
                        {
                            if (AGNetwork.IsMobileConnected())
                            {
                                status = "4G";
                                wifiInfo = (" Speed: ...");
                            }
                            else
                            {
                                status = "---";
                            }
                        }
                    }
                    //connection.text = ("Тип соединения: " + status + wifiInfo);
                }
                else
                {
                    status = "Wi-Fi";
                    //connection.text = ("Соединение не установлено.");
                }
                if (!isServer)
                {
                    if (engine.GetComponent<engineClient>().isReal)
                    {
                        vidStat = "Online";
                    }
                    else
                    {
                        vidStat = "Offline";
                    }
                    vidStat = engine.GetComponent<engineClient>().isReal.ToString();

                    output = ("Battery: " + AGBattery.GetBatteryChargeLevel() + " | " + status + " | " + currentState + check.ToString() + "c " + vidStat + " " + duration.ToString());
                    playerData.text = output;
                    Debug.Log("Calling refresh data " + output);

                    Payload payload = new Payload();
                    payload.stateData = playerData.text;

                    LobbyManager.requests["refreshData"](payload);
                }

                if (check > 9)
                {
                    check = 0;
                }
                t = 0;
            }
        }
    }
}
