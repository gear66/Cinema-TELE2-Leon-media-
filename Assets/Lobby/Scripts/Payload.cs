using Newtonsoft.Json;

public class Payload
{
    [JsonProperty("stateData")]
    public string stateData { get; set; }

    [JsonProperty("user")]
    public User user { get; set; }

    [JsonProperty("target")]
    public string target { get; set; }

    [JsonProperty("onlineVid")]
    public bool onlineVid { get; set; }

    [JsonProperty("lobby")]
    public string lobby { get; set; }

    [JsonProperty("speedTest")]
    public float speedTest { get; set; }
}
