using Newtonsoft.Json;

public class Payload
{
    [JsonProperty("stateData")]
    public string stateData { get; set; }

    [JsonProperty("duration")]
    public float duration { get; set; }

    [JsonProperty("user")]
    public User user { get; set; }

    [JsonProperty("target")]
    public string target { get; set; }

    [JsonProperty("onlineVideo")]
    public bool onlineVideo { get; set; }

    [JsonProperty("lobbyName")]
    public string lobbyName { get; set; }

    [JsonProperty("speedTest")]
    public float speedTest { get; set; }
}
