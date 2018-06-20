using Newtonsoft.Json;

public class Message
{
    [JsonProperty("payload")]
    public Payload payload { get; set; }

    [JsonProperty("command")]
    public string command { get; set; }

    [JsonProperty("message")]
    public string message { get; set; }

    [JsonProperty("error")]
    public string error { get; set; }

    [JsonProperty("success")]
    public bool success { get; set; }

    [JsonProperty("isResponse")]
    public bool isResponse { get; set; }

    [JsonProperty("isRequest")]
    public bool isRequest { get; set; }
}
