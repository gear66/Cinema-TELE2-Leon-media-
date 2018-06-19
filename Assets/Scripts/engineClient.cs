using Prototype.NetworkLobby;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class engineClient : MonoBehaviour {

    public InputField ipData;
    public InputField nameData;
    public bool is4g = true;
    public bool isInCinema = false;
    public bool isWatchEnded = false;
    public bool isReal;
    bool isAudioPlayed = false;
    public int flag = 0;
    public int set;
    public int setVid;
    public float duration;
    public float t = 5;
    bool ok;
    int vidNum;

    [Header("Videos:")]
    public GameObject[] videoss;
    public GameObject[] videossOnline;
    public GameObject onlineVid;
    public GameObject offlineVid;
    public GameObject currentVideo;
    public GameObject currentVideoLocal;
    GameObject oldVideo;
    public GameObject vidPrefab;
    public GameObject splash;

    [Header("Player:")]
    public GameObject gazee;
    public GameObject pointer;
    public GameObject player;
    public GameObject cinemaEnv;

    [Header("Left banner:")]
    public MeshRenderer leftScrMat;
    public MeshRenderer leftScrMatL;
    public MeshRenderer prerollMat;
    public Material g3;
    public Material g4;
    public Material g33;
    public Material g44;

    [Header("Audio lines:")]
    public AudioSource audioSrc;
    public AudioClip hello3g;
    public AudioClip hello4g;
    AudioClip hellog;
    public AudioClip watch3g;
    public AudioClip watch4g;
    AudioClip watchg;
    public AudioClip end3g;
    public AudioClip end4g;
    AudioClip endg;

    [Header("UI:")]
    public GameObject browBlock;
    public GameObject browserColl2;
    public GameObject buttons;
    public GameObject indicator;

    [Header("Switchers:")]
    public GameObject buttOnline;
    public GameObject buttOffline;
    public GameObject buttBanner3g;
    public GameObject buttBanner4g;

    [Header("Displayed:")]
    public double curTime;
    public long curFrame;

    // Use this for initialization
    void Start () {
        ok = false;
        flag = 0;
        ipData.text = PlayerPrefs.GetString("IP", "");
        nameData.text = PlayerPrefs.GetString("setName", "Client");
        audioSrc.clip = null;
        hellog = null;
        watchg = null;
        endg = null;
        //indicator = GameObject.Find("indicator");
        set = PlayerPrefs.GetInt("set", 3);
        setVid = PlayerPrefs.GetInt("setVid", 0);
    }
	
	// Update is called once per frame
	void Update () {
        if (!ok)
        {
            if (set == 3)
            {
                ImageSet3g();
            }
            if (set == 4)
            {
                ImageSet4g();
            }
            if (setVid == 0)
            {
                OnlineVideo(false);
            }
            if (setVid == 1)
            {
                OnlineVideo(true);
            }
            Debug.Log("Presets set OK");
            ok = true;
        }

        if (isInCinema)
        {
            t += Time.deltaTime;
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                
                if (t < 2)
                {
                    flag = 5;
                    buttons.SetActive(true);
                    audioSrc.Stop();
                    Debug.Log("Demo skipped vvv");
                }
                else
                {
                    StartDemo();
                }
                t = 0;
            }
            if (Input.GetKeyUp(KeyCode.A))
            {
                flag = 5;
                buttons.SetActive(true);
            }
            if (flag == 1)
            {
                if (audioSrc.time > audioSrc.clip.length - 10f)
                {
                    Debug.Log("speedtest coll deleted");
                    browserColl2.SetActive(false);
                }
                if (audioSrc.time > audioSrc.clip.length - 0.01f)
                {
                    Debug.Log("First audio ended");
                    audioSrc.Stop();
                    //audioSrc.clip.
                    //flag = 2;
                }
            }
            if (flag == 2)
            {
                //if (audioSrc.time > audioSrc.clip.length - 0.01f)
                //{
                    Debug.Log("Second audio started");
                    audioSrc.clip = watchg;
                    audioSrc.Play();
                    flag = 3;
                buttons.SetActive(true);
                //}
            }
            if (flag == 3)
            {
                if (audioSrc.time > audioSrc.clip.length - 0.01f)
                {
                    Debug.Log("Second audio ended");
                    audioSrc.Stop();
                    browBlock.SetActive(false);
                    browserColl2.SetActive(false);
                    //flag = 4;
                }
            }
            if (flag == 4)
            {
                //if (isWatchEnded)
                //{
                    Debug.Log("End audio started");
                    audioSrc.clip = endg;
                    audioSrc.Play();
                    browBlock.SetActive(true);
                    browserColl2.SetActive(true);
                    flag = 0;
                //}
            }
            if (flag == 5)
            {
                browBlock.SetActive(false);
                browserColl2.SetActive(false);
                flag = 0;
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GoCinema();
                Debug.Log("Returned to cinema with flag vvv " + flag);
            }
            if (currentVideo != null)
            {
                duration = (float)currentVideo.GetComponent<VideoPlayer>().time;
            }
            else if (currentVideoLocal != null)
            {
                duration = (float)currentVideoLocal.GetComponent<VideoPlayerInitializer>().totalTime - (float)currentVideoLocal.GetComponent<VideoPlayer>().time;
            }
        }
    }

    public void SaveIP()
    {
        PlayerPrefs.SetString("IP", ipData.text);
        LobbyManager.s_Singleton.LobbyName = ipData.text;
        LobbyManager.s_Singleton.UserName = nameData.text;
    }
    public void SaveName()
    {
        PlayerPrefs.SetString("setName", nameData.text);
        LobbyManager.s_Singleton.LobbyName = ipData.text;
        LobbyManager.s_Singleton.UserName = nameData.text;
    }

    public void SwitchToLocal(long frame, double time)
    {
        Debug.Log("Switched to local video on: vvv " + frame + " " + time);
        curFrame = frame;
        curTime = time;
        //oldVideo = currentVideo;
        //DestroyOnlineVideo();
        //currentVideo = null;
        splash.SetActive(false);
        currentVideoLocal = Instantiate(videoss[vidNum - 1]);
        currentVideoLocal.SetActive(true);
        currentVideoLocal.GetComponent<VideoPlayer>().Play();
        currentVideoLocal.GetComponent<VideoPlayerInitializer>().setFrame = curFrame;
        currentVideoLocal.GetComponent<VideoPlayerInitializer>().SetFrame(curFrame);
        indicator.SetActive(true);
        //splash.SetActive(false);
        Debug.Log("Video " + vidNum + " replaced! vvv " + curFrame + " " + curTime);
    }

    public void DestroyOnlineVideo()
    {
        if (currentVideo != null)
        {
            Destroy(currentVideo);
        }
        else
        {
            Debug.Log("Nothing to destroy!");
        }
    }

    public void GoWatch(int i)
    {
        vidNum = i;
        buttons.SetActive(false);
        browserColl2.SetActive(false);
        isInCinema = false;
        gazee.SetActive(false);
        vidPrefab.SetActive(true);
        cinemaEnv.SetActive(false);
        splash.SetActive(true);
        if (isReal)
        {
            currentVideo = Instantiate(videossOnline[i - 1]);
            currentVideo.SetActive(true);
            //currentVideo.GetComponent<HighQualityPlayback>().Play();
            Debug.Log("Video "+ i +" online instantiated vvv");

            //videossOnline[i - 1].SetActive(t rue);
            //videossOnline[i - 1].GetComponent<HighQualityPlayback>().Play() ;
            //Debug.Log(i + " online selected, play started");
        }
        else
        {
            currentVideoLocal = Instantiate(videoss[i - 1]);
            currentVideoLocal.SetActive(true);
            currentVideoLocal.GetComponent<VideoPlayerInitializer>().setFrame = 0;
            Debug.Log("Video " + i + " offline instantiated vvv");
            //videoss[i - 1].SetActive(true);
            //videoss[i - 1].GetComponent<VideoPlayer>().Play();
            //videoss[i - 1].GetComponent<MediaPlayerCtrl>().Play();
        }
    }

    public void GoCinema()
    {
        indicator.SetActive(false);
        if (currentVideo != null)
        {
            Destroy(currentVideo);
        }
        if (currentVideoLocal != null)
        {
            Destroy(currentVideoLocal);
        }
        splash.SetActive(false);
        foreach (GameObject i in videoss)
        {
            i.SetActive(false);
        }
        foreach (GameObject i in videossOnline)
        {
            //i.GetComponent<HighQualityPlayback>().OnReset();
            //i.GetComponent<VideoController>().Stop();
            i.SetActive(false);
        }
        vidPrefab.SetActive(false);
        cinemaEnv.SetActive(true);
        //rig.SetActive(false);
        //player.SetActive(true);
        isInCinema = true;
        browBlock.SetActive(true);
        browserColl2.SetActive(true);
        gazee.SetActive(true);
        Debug.Log("GoCinema called with flag vvv "+ flag);
        //main.enabled = true;
    }

    public void SetRot(GameObject ga)
    {
        ga.transform.position = pointer.transform.position;
        ga.transform.rotation = pointer.transform.rotation;
    }

    public void StartDemo()
    {
        browBlock.SetActive(true);
        audioSrc.clip = hellog;
        audioSrc.Play();
        isAudioPlayed = true;
        flag = 1;
        Debug.Log("Demo started");
    }

    public void ImageSet3g()
    {
        is4g = false;
        PlayerPrefs.SetInt("set", 3);
        leftScrMat.material = g3;
        leftScrMatL.material = g3;
        prerollMat.material = g33;
        hellog = hello3g;
        watchg = watch3g;
        endg = end3g;
        if (buttBanner3g != null & buttBanner4g != null)
        {
            buttBanner4g.SetActive(false);
            buttBanner3g.SetActive(true);
            Debug.Log("image set 3g applied");
        }
    }

    public void ImageSet4g()
    {
        is4g = true;
        PlayerPrefs.SetInt("set", 4);
        leftScrMat.material = g4;
        leftScrMatL.material = g4;
        prerollMat.material = g44;
        hellog = hello4g;
        watchg = watch4g;
        endg = end4g;
        if (buttBanner3g != null & buttBanner4g != null)
        {
            buttBanner3g.SetActive(false);
            buttBanner4g.SetActive(true);
            Debug.Log("image set 4g applied");
        }
    }

    public void WatchEnded()
    {
        isWatchEnded = true;
        flag = 4;
    }

    public void OnlineVideo(bool set)
    {
        if (set & offlineVid != null & onlineVid != null & buttOffline != null & buttOnline != null)
        {
            PlayerPrefs.SetInt("setVid", 1);
            offlineVid.SetActive(false);
            onlineVid.SetActive(true);
            Debug.Log("Online video choosed");
            isReal = true;
            buttOffline.SetActive(false);
            buttOnline.SetActive(true);
        }
        else if(offlineVid != null & onlineVid != null & buttOffline != null & buttOnline != null)
        {
            PlayerPrefs.SetInt("setVid", 0);
            offlineVid.SetActive(true);
            onlineVid.SetActive(false);
            Debug.Log("Offline video choosed");
            isReal = false;
            buttOnline.SetActive(false);
            buttOffline.SetActive(true);
        }
    }

    public void Ok()
    {
        ok = true;
    }
}
