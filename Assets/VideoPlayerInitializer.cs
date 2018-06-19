using System.IO;
using UnityEngine;
using UnityEngine.Video;


[RequireComponent(typeof(VideoPlayer))]
public class VideoPlayerInitializer : MonoBehaviour {

    //public GameObject vPlayer;
    public float t;
    bool flag;
    public int stage; 
    GameObject splash;
    GameObject engine;
    public long frame;
    public long setFrame;
    public double time;
    public int isReadyStage = 0;

    // Use this for initialization
    void Start ()
    {
        flag = true;
        t = 0;
        splash = GameObject.Find("splash");
        var player = GetComponent<VideoPlayer>();
        var fileName = Path.GetFileName(player.url);
        var fixedFileName = Application.persistentDataPath + "/" + fileName;
        engine = GameObject.Find("ENGINE");
        player.url = fixedFileName;
        stage = 0;
        isReadyStage = 0;
    }

    public void SetFrame(long f)
    {
        GetComponent<VideoPlayer>().frame = setFrame;
        Debug.Log("Set frame " + f +" called");
    }

    void Update () {
        t += Time.deltaTime;
        if (GetComponent<VideoPlayer>().isPrepared & isReadyStage == 0)
        {
            //engine.GetComponent<engineClient>().DestroyOnlineVideo();
            SetFrame(setFrame);
            isReadyStage = 1;
        }
        if (GetComponent<VideoPlayer>().isPlaying & isReadyStage == 1 & t > 2f)
        {
            engine.GetComponent<engineClient>().DestroyOnlineVideo();
            isReadyStage = 2;
        }

        if (Input.GetKeyUp(KeyCode.O))
        {
            SetFrame(setFrame);
        }

        frame = GetComponent<VideoPlayer>().frame;
        time = GetComponent<VideoPlayer>().time;
        if (t > 4.5 & flag)
        {
            gameObject.GetComponent<VideoPlayer>().Play();
            if (splash != null)
            {
                splash.SetActive(false);
            }
            flag = false;
            stage = 1;
        }
        if (t>5 & (gameObject.GetComponent<VideoPlayer>().frame >= gameObject.GetComponent<VideoPlayer>().frameCount - 5f) & stage == 1)
        {
            engine.GetComponent<engineClient>().GoCinema();
            engine.GetComponent<engineClient>().WatchEnded();
            stage = 0;
        }
    }
}
